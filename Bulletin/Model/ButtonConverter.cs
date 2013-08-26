using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Data;
using System.Windows.Media;

namespace Bulletin.Model {
	public class ButtonConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string result = string.Empty;
			result = Path.Combine("Resource/Images", @"button" + value.ToString() + ".png");
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class ButtonBackConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string result = string.Empty;
			result = Path.Combine("Resource/Images", @"buttonback" + value.ToString() + ".png");
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class BackgroundConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			SolidColorBrush result = null;
			var list = value as List<SolidColorBrush>;
			Random ro = new Random();
			result = list[ro.Next(0,(list.Count - 1))];
			Thread.Sleep(100);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
