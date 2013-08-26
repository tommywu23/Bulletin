using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Bulletin.Model {
	public class MwSysDateTimeConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string sDt = "";
			try {
				DateTime dtTmp = (DateTime)value;

				sDt =
					dtTmp.Hour.ToString("00") + ":" +
					dtTmp.Minute.ToString("00");

			} catch { }

			return sDt;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
}
