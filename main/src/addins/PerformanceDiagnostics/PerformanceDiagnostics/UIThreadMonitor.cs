﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using MonoDevelop.Core;
using System.IO;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceDiagnosticsAddIn
{
	class UIThreadMonitor
	{
		public static UIThreadMonitor Instance { get; } = new UIThreadMonitor ();

		UIThreadMonitor () { }

		Thread tcpLoopThread;
		Thread dumpsReaderThread;
		Thread pumpErrorThread;
		TcpListener listener;
		Process process;

		void TcpLoop (object param)
		{
			var socket = (Socket)param;
			byte [] buffer = new byte [1];
			ManualResetEvent waitUIThread = new ManualResetEvent (false);
			while (true) {
				var readBytes = socket.Receive (buffer, 1, SocketFlags.None);
				if (readBytes != 1)
					return;
				waitUIThread.Reset ();
				Runtime.RunInMainThread (delegate {
					waitUIThread.Set ();
				});
				waitUIThread.WaitOne ();
				socket.Send (buffer);
			}
		}

		TimeSpan forceProfileTime = TimeSpan.Zero;

		public bool ToggleProfilingChecked => sampleProcessPid != -1;

		int sampleProcessPid = -1;
		public void ToggleProfiling ()
		{
			if (sampleProcessPid != -1) {
				Mono.Unix.Native.Syscall.kill (sampleProcessPid, Mono.Unix.Native.Signum.SIGINT);
				sampleProcessPid = -1;
				return;
			}

			var outputFilePath = Path.GetTempFileName ();
			var startInfo = new ProcessStartInfo ("sample");
			startInfo.UseShellExecute = false;
			startInfo.Arguments = $"{Process.GetCurrentProcess ().Id} 10000 -file {outputFilePath}";
			var sampleProcess = Process.Start (startInfo);
			sampleProcess.EnableRaisingEvents = true;
			sampleProcess.Exited += delegate {
				ConvertJITAddressesToMethodNames (outputFilePath, "Profile");
			};
			sampleProcessPid = sampleProcess.Id;
		}

		public void Profile (int seconds)
		{
			var outputFilePath = Path.GetTempFileName ();
			var startInfo = new ProcessStartInfo ("sample");
			startInfo.UseShellExecute = false;
			startInfo.Arguments = $"{Process.GetCurrentProcess ().Id} {seconds} -file {outputFilePath}";
			var sampleProcess = Process.Start (startInfo);
			sampleProcess.EnableRaisingEvents = true;
			sampleProcess.Exited += delegate {
				ConvertJITAddressesToMethodNames (outputFilePath, "Profile");
			};
		}

		public bool IsListening { get; private set; }
		public bool IsSampling { get; private set; }
		public string HangFileName { get; set; }

		public void Start (bool sample)
		{
			if (IsListening) {
				if (IsSampling == sample)
					return;
				Stop ();
			}
			if (sample) {
				if (!(Environment.GetEnvironmentVariable ("MONO_DEBUG")?.Contains ("disable_omit_fp") ?? false)) {
					MessageService.ShowWarning ("Set environment variable",
												$@"It is highly recommended to set environment variable ""MONO_DEBUG"" to ""disable_omit_fp"" and restart {BrandingService.ApplicationName} to have better results.");
				}
			}
			IsListening = true;
			IsSampling = sample;
			//start listening on random port
			listener = new TcpListener (IPAddress.Loopback, 0);
			listener.Start ();
			listener.AcceptSocketAsync ().ContinueWith (t => {
				if (!t.IsFaulted && !t.IsCanceled) {
					tcpLoopThread = new Thread (new ParameterizedThreadStart (TcpLoop));
					tcpLoopThread.IsBackground = true;
					tcpLoopThread.Start (t.Result);
					listener.Stop ();
				}
			});
			//get random port provided by OS
			var port = ((IPEndPoint)listener.LocalEndpoint).Port;
			process = new Process ();
			process.StartInfo.FileName = "mono";
			process.StartInfo.Arguments = GetArguments (port, sample);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;//Ignore it, otherwise it goes to IDE logging
			process.Start ();
			process.StandardError.ReadLine ();
			dumpsReaderThread = new Thread (new ThreadStart (DumpsReader));
			dumpsReaderThread.IsBackground = true;
			dumpsReaderThread.Start ();

			pumpErrorThread = new Thread (new ThreadStart (PumpErrorStream));//We need to read this...
			pumpErrorThread.IsBackground = true;
			pumpErrorThread.Start ();
		}

		string GetArguments (int port, bool sample)
		{
			var arguments = new StringBuilder ();
			arguments.Append ($"{typeof (UIThreadMonitorDaemon.MainClass).Assembly.Location} {port} {Process.GetCurrentProcess ().Id}");

			if (!sample)
				arguments.Append (" --noSample");

			if (!string.IsNullOrEmpty (HangFileName))
				arguments.Append ($" --hangFile:\"{HangFileName}\"");

			return arguments.ToString ();
		}

		[DllImport ("__Internal")]
		extern static string mono_pmip (long offset);
		static Dictionary<long, string> methodsCache = new Dictionary<long, string> ();

		void PumpErrorStream ()
		{
			while (!(process?.HasExited ?? true)) {
				process?.StandardError?.ReadLine ();
			}
		}

		void DumpsReader ()
		{
			while (!(process?.HasExited ?? true)) {
				var fileName = process.StandardOutput.ReadLine ();
				ConvertJITAddressesToMethodNames (fileName, "UIThreadHang");
			}
		}

		public void Stop ()
		{
			if (!IsListening)
				return;
			IsListening = false;
			IsSampling = false;
			listener.Stop ();
			listener = null;
			process.Kill ();
			process = null;
		}

		internal static void ConvertJITAddressesToMethodNames (string fileName, string profilingType)
		{
			var rx = new Regex (@"\?\?\?  \(in <unknown binary>\)  \[0x([0-9a-f]+)\]", RegexOptions.Compiled);
			if (File.Exists (fileName) && new FileInfo (fileName).Length > 0) {
				var outputFilename = Path.Combine (Options.OutputPath, $"{BrandingService.ApplicationName}_{profilingType}_{DateTime.Now:yyyy-MM-dd__HH-mm-ss}.txt");
				using (var sr = new StreamReader (fileName))
				using (var sw = new StreamWriter (outputFilename)) {
					string line;
					while ((line = sr.ReadLine ()) != null) {
						if (rx.IsMatch (line)) {
							var match = rx.Match (line);
							var offset = long.Parse (match.Groups [1].Value, NumberStyles.HexNumber);
							string pmipMethodName;
							if (!methodsCache.TryGetValue (offset, out pmipMethodName)) {
								pmipMethodName = mono_pmip (offset)?.TrimStart ();
								methodsCache.Add (offset, pmipMethodName);
							}
							if (pmipMethodName != null) {
								line = line.Remove (match.Index, match.Length);
								line = line.Insert (match.Index, pmipMethodName);
							}
						}
						sw.WriteLine (line);
					}
				}
			}
		}
	}
}

