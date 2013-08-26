using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bulletin.View {
	public partial class Marquee : UserControl {
		public string MarqueeContent {
			get { return (string)GetValue(MarqueeContentProperty); }
			set { SetValue(MarqueeContentProperty, value); }
		}

		public static readonly DependencyProperty MarqueeContentProperty =
			DependencyProperty.Register("MarqueeContent", typeof(string), typeof(Marquee));

		private double _marqueeTimeInSeconds;

		public double MarqueeTimeInSeconds {
			get { return _marqueeTimeInSeconds; }
			set { _marqueeTimeInSeconds = value; }
		}


		public Marquee() {
			InitializeComponent();
			this.Loaded += new RoutedEventHandler(Marquee_Loaded);
		}

		void Marquee_Loaded(object sender, RoutedEventArgs e) {
			canMain.Height = this.ActualHeight;
			canMain.Width = this.ActualWidth;
			StartMarqueeing();
		}

		public void StartMarqueeing() {
			double height = canMain.ActualHeight - tbmarquee.ActualHeight;
			tbmarquee.Margin = new Thickness(0, height / 2, 0, 0);
			DoubleAnimation doubleAnimation = new DoubleAnimation();
			doubleAnimation.From = -tbmarquee.ActualWidth;
			doubleAnimation.To = canMain.ActualWidth;
			doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
			doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(_marqueeTimeInSeconds));
			tbmarquee.BeginAnimation(Canvas.RightProperty, doubleAnimation);
		}
	}
}
