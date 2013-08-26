using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using BulletinLibrary;

namespace ConfigApp.Model {
	public class RadioConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int result = -1;
			var obj = ConfigManager.Instance.Items.FirstOrDefault(p => p.Index == (int)value);
			if(!string.IsNullOrEmpty(obj.ExePath)) result = 0;
			if (!string.IsNullOrEmpty(obj.PhotoPath)) result = 1;
			if (!string.IsNullOrEmpty(obj.Url)) result = 2;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}

	public class WebRadioConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility result = Visibility.Hidden;
			if ((int)value == 2) result = Visibility.Visible;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}

	public class PhotoRadioConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility result = Visibility.Hidden;
			if ((int)value == 1) result = Visibility.Visible;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}

	public class ExeRadioConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Visibility result = Visibility.Hidden;
			if ((int)value == 0) result = Visibility.Visible;
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
