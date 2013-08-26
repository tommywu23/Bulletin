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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BulletinLibrary;

namespace Bulletin.View {
	public partial class GalleryView : Window {
		public GalleryView() {
			InitializeComponent();
			this.WindowStyle = System.Windows.WindowStyle.None;
			this.AllowsTransparency = true;
			view1.ToBack += view1_ToBack;
		}

		void view1_ToBack() {
			view1.DataContext = null;
			view1.Index = 0;
			view1.Visibility = System.Windows.Visibility.Hidden;
			gallery1.Visibility = System.Windows.Visibility.Visible;
		}

		private void Grid_MouseUp(object sender, MouseButtonEventArgs e) {
			gallery1.Visibility = System.Windows.Visibility.Hidden;
			view1.Photos = ((sender as Grid).Tag as Album).Photos;
			view1.currentImage.Source = view1.Photos[view1.Index].Source;
			view1.previous.Visibility = System.Windows.Visibility.Hidden;
			view1.Visibility = System.Windows.Visibility.Visible;
		}
	}
}
