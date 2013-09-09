using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClientContract.View {
	[TemplatePart(Name = "PART_Image", Type = typeof(Image))]
	public class CharmButton : Button {
		private void OnImageSourceChanged(ImageSource oldValue, ImageSource newValue) {
			if (newValue is BitmapSource) {
				int w = (int)(newValue.Width / 2);
				int h = (int)newValue.Height;
				NormalImage = new CroppedBitmap(newValue as BitmapSource, new Int32Rect(0, 0, w, h));
				PressedImage = new CroppedBitmap(newValue as BitmapSource, new Int32Rect(w, 0, w, h));
			}
		}

		public CharmButton() {
			this.PreviewMouseDown += OnPreviewMouseDown;
			this.PreviewMouseUp += OnPreviewMouseUp;
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (image != null) {
				image.Source = PressedImage;
				this.CaptureMouse();
			}
			e.Handled = true;
		}

		private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e) {
			if (image != null) {
				if (this.IsMouseCaptured) {
					this.ReleaseMouseCapture();
				}

				image.Source = NormalImage;
				this.OnClick();
			}
			e.Handled = true;
		}

		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			image = this.GetTemplateChild("PART_Image") as Image;
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
			(d as CharmButton).OnImageSourceChanged(e.OldValue as ImageSource, e.NewValue as ImageSource);
		}

		static CharmButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CharmButton), new FrameworkPropertyMetadata(typeof(CharmButton)));
		}

		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(CharmButton), new UIPropertyMetadata(null, OnImageSourceChanged));

		public static readonly DependencyProperty NormalImageProperty =
			DependencyProperty.Register("NormalImage", typeof(ImageSource), typeof(CharmButton), new UIPropertyMetadata(null));

		public static readonly DependencyProperty PressedImageProperty =
			DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(CharmButton), new UIPropertyMetadata(null));

		public Image image;
	}
}
