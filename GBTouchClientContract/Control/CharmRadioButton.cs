﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientContract.View {
	public class CharmRadioButton : RadioButton {
		private void OnImageSourceChanged(ImageSource oldValue, ImageSource newValue) {
			if (newValue is BitmapSource) {
				int w = (int)(newValue.Width / 2);
				int h = (int)newValue.Height;
				NormalImage = new CroppedBitmap(newValue as BitmapSource, new Int32Rect(0, 0, w, h));
				PressedImage = new CroppedBitmap(newValue as BitmapSource, new Int32Rect(w, 0, w, h));
			}
		}

		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public ImageSource NormalImage {
			get { return (ImageSource)GetValue(NormalImageProperty); }
			set { SetValue(NormalImageProperty, value); }
		}

		public ImageSource PressedImage {
			get { return (ImageSource)GetValue(PressedImageProperty); }
			set { SetValue(PressedImageProperty, value); }
		}

		public static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			(d as CharmRadioButton).OnImageSourceChanged(e.OldValue as ImageSource, e.NewValue as ImageSource);
		}

		static CharmRadioButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CharmRadioButton), new FrameworkPropertyMetadata(typeof(CharmRadioButton)));
		}

		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CharmRadioButton), new UIPropertyMetadata(null, OnImageSourceChanged));

		public static readonly DependencyProperty NormalImageProperty =
			DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(CharmRadioButton), new UIPropertyMetadata(null));

		public static readonly DependencyProperty PressedImageProperty =
			DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(CharmRadioButton), new UIPropertyMetadata(null));
	}
}
