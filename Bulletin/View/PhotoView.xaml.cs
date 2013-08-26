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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletinLibrary;

namespace Bulletin.View {
	public partial class PhotoView : UserControl {
		public event Action ToBack;
		public int Index { get; set; }
		public List<Photo> Photos { get; set; }

		public PhotoView() {
			InitializeComponent();
			Index = 0;
			Photos = new List<Photo>();
		}

		private void OnBack(object sender, RoutedEventArgs e) {
			if (ToBack != null) ToBack();
		}

		private void OnNext(object sender, RoutedEventArgs e) {
			if ((Index + 1) < this.Photos.Count) {
				Index++;
				currentImage.Source = this.Photos[Index].Source;
				if ((this.Photos.Count - 1) == Index) next.Visibility = System.Windows.Visibility.Hidden;
				if (Index > 0) previous.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void OnPrevious(object sender, RoutedEventArgs e) {
			if ((Index - 1) > -1) {
				Index--;
				currentImage.Source = this.Photos[Index].Source;
				if (Index == 0) previous.Visibility = System.Windows.Visibility.Hidden;
				if (Index < this.Photos.Count) next.Visibility = System.Windows.Visibility.Visible;
			}
		}
	}
}
