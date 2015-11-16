﻿//
// Styles.cs
//
// Author:
//       Vsevolod Kukol <sevo@sevo.org>
//
// Copyright (c) 2015 Xamarin, Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Windows.Media;
using MonoDevelop.Ide;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsPlatform
{
	public static class Styles
	{
		static Brush mainToolbarBackgroundBrush;
		static Brush mainToolbarForegroundBrush;
		static Brush mainToolbarShadowBrush;
		static Brush mainToolbarSeparatorBrush;

		static Brush menuBackgroundBrush;
		static Brush menuForegroundBrush;
		static Brush menuBorderBrush;
		static Brush menuHighlightBackgroundBrush;
		static Brush menuHighlightBorderBrush;
		static Brush menuSelectedBackgroundBrush;
		static Brush menuSelectedBorderBrush;
		static Brush menuDisabledForegroundBrush;
		static Brush menuSeparatorBrush;

		static Brush statusBarBackgroundBrush;
		static Brush statusBarTextBrush;
		static Brush statusBarErrorTextBrush;
		static Brush statusBarWarningTextBrush;
		static Brush statusBarReadyTextBrush;
		static Brush searchBarBorderBrush;
		static Brush searchBarBackgroundBrush;
		static Brush searchBarTextBrush;

		public static Brush MainToolbarBackgroundBrush {
			get { return mainToolbarBackgroundBrush; }
			private set { mainToolbarBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MainToolbarForegroundBrush {
			get { return mainToolbarForegroundBrush; }
			private set { mainToolbarForegroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MainToolbarShadowBrush {
			get { return mainToolbarShadowBrush; }
			private set { mainToolbarShadowBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MainToolbarSeparatorBrush {
			get { return mainToolbarSeparatorBrush; }
			private set { mainToolbarSeparatorBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuBackgroundBrush {
			get { return menuBackgroundBrush; }
			private set { menuBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuForegroundBrush {
			get { return menuForegroundBrush; }
			private set { menuForegroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuBorderBrush {
			get { return menuBorderBrush; }
			private set { menuBorderBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuHighlightBackgroundBrush {
			get { return menuHighlightBackgroundBrush; }
			private set { menuHighlightBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuHighlightBorderBrush {
			get { return menuHighlightBorderBrush; }
			private set { menuHighlightBorderBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuSelectedBackgroundBrush {
			get { return menuSelectedBackgroundBrush; }
			private set { menuSelectedBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuSelectedBorderBrush {
			get { return menuSelectedBorderBrush; }
			private set { menuSelectedBorderBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuDisabledForegroundBrush {
			get { return menuDisabledForegroundBrush; }
			private set { menuDisabledForegroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush MenuSeparatorBrush {
			get { return menuSeparatorBrush; }
			set { menuSeparatorBrush = value; RaisePropertyChanged (); }
		}

		public static Brush StatusBarBackgroundBrush {
			get { return statusBarBackgroundBrush; }
			private set { statusBarBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush StatusBarTextBrush {
			get { return statusBarTextBrush; }
			private set { statusBarTextBrush = value; RaisePropertyChanged (); }
		}

		public static Brush StatusBarErrorTextBrush {
			get { return statusBarErrorTextBrush; }
			private set { statusBarErrorTextBrush = value; RaisePropertyChanged (); }
		}

		public static Brush StatusBarWarningTextBrush {
			get { return statusBarWarningTextBrush; }
			private set { statusBarWarningTextBrush = value; RaisePropertyChanged (); }
		}

		public static Brush StatusBarReadyTextBrush {
			get { return statusBarReadyTextBrush; }
			private set { statusBarReadyTextBrush = value; RaisePropertyChanged (); }
		}

		public static Brush SearchBarBorderBrush {
			get { return searchBarBorderBrush; }
			private set { searchBarBorderBrush = value; RaisePropertyChanged (); }
		}

		public static Brush SearchBarBackgroundBrush {
			get { return searchBarBackgroundBrush; }
			private set { searchBarBackgroundBrush = value; RaisePropertyChanged (); }
		}

		public static Brush SearchBarTextBrush {
			get { return searchBarTextBrush; }
			private set { searchBarTextBrush = value; RaisePropertyChanged (); }
		}

		static Styles ()
		{
			Xwt.Drawing.Context.RegisterStyles ("hover", "pressed", "disabled");
			LoadStyles ();
			MonoDevelop.Ide.Gui.Styles.Changed += (o, e) => LoadStyles ();
		}

		public static void LoadStyles ()
		{
			if (IdeApp.Preferences.UserInterfaceSkin == Skin.Light) {
				MainToolbarBackgroundBrush = Brushes.Transparent;
				MainToolbarForegroundBrush = Brushes.Black;
				MainToolbarShadowBrush = Brushes.Gray;
				MainToolbarSeparatorBrush = new SolidColorBrush (new Color { A = 0xFF, R = 0x7d, G = 0x7d, B = 0x7d, });

				MenuBackgroundBrush = SystemColors.MenuBarBrush;
				MenuForegroundBrush = SystemColors.MenuTextBrush;
				MenuBorderBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x99, G = 0x99, B = 0x99});
				MenuSeparatorBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0xD7, G = 0xD7, B = 0xD7});
				MenuHighlightBackgroundBrush = new SolidColorBrush (new Color {A = 0x3D, R = 0x26, G = 0xA0, B = 0xDA});
				MenuHighlightBorderBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x26, G = 0xA0, B = 0xDA});
				MenuSelectedBackgroundBrush = new SolidColorBrush (new Color {A = 0x3D, R = 0x26, G = 0xA0, B = 0xDA});
				MenuSelectedBorderBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x26, G = 0xA0, B = 0xDA});
				MenuDisabledForegroundBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x70, G = 0x70, B = 0x70});

				StatusBarBackgroundBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0xE5, G = 0xE5, B = 0xE5});
				StatusBarTextBrush = MainToolbarForegroundBrush;
				StatusBarErrorTextBrush = Brushes.Red;
				StatusBarWarningTextBrush = Brushes.Orange;
				StatusBarReadyTextBrush = Brushes.Gray;
				SearchBarBorderBrush = Brushes.LightGray;
				SearchBarBackgroundBrush = Brushes.White;
				SearchBarTextBrush = MainToolbarForegroundBrush;
			} else {
				MainToolbarBackgroundBrush = new SolidColorBrush (new Color {A=0xFF, R=0x33, G=0x33, B=0x33});
				MainToolbarForegroundBrush = Brushes.White;
				MainToolbarShadowBrush = Brushes.Gray;
				MainToolbarSeparatorBrush = new SolidColorBrush (new Color { A = 0xFF, R = 0x7d, G = 0x7d, B = 0x7d, });

				MenuBackgroundBrush = MainToolbarBackgroundBrush;
				MenuForegroundBrush = MainToolbarForegroundBrush;
				MenuBorderBrush = Brushes.LightGray;
				MenuSeparatorBrush = Brushes.DimGray;
				MenuHighlightBackgroundBrush = new SolidColorBrush (new Color {A=0x3D, R=0xD3, G=0xD3, B=0xD3});
				MenuHighlightBorderBrush = Brushes.LightGray;
				MenuSelectedBackgroundBrush = new SolidColorBrush (new Color {A=0x3D, R=0xD3, G=0xD3, B=0xD3});
				MenuSelectedBorderBrush = Brushes.LightGray;
				MenuDisabledForegroundBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x70, G = 0x70, B = 0x70});

				StatusBarBackgroundBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x22, G = 0x22, B = 0x22});
				StatusBarTextBrush = MainToolbarForegroundBrush;
				StatusBarErrorTextBrush = Brushes.Red;
				StatusBarWarningTextBrush = Brushes.Orange;
				StatusBarReadyTextBrush = Brushes.LightGray;
				SearchBarBorderBrush = Brushes.Black;
				SearchBarBackgroundBrush = new SolidColorBrush (new Color {A = 0xFF, R = 0x22, G = 0x22, B = 0x22});
				SearchBarTextBrush = MainToolbarForegroundBrush;
			}
		}

		public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

		static void RaisePropertyChanged ([CallerMemberName] string propName = null)
		{
			if (StaticPropertyChanged != null)
				StaticPropertyChanged (null, new PropertyChangedEventArgs (propName));
		}
	}
}

