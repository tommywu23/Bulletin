using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Bulletin.Model {
	public class WeatherInfo : DependencyObject {
		public WeatherCode WeatherCode { get; set; }
		public string TodayH {
			get { return (string)GetValue(TodayHProperty); }
			set { SetValue(TodayHProperty, value); }
		}
		public string TodayL {
			get { return (string)GetValue(TodayLProperty); }
			set { SetValue(TodayLProperty, value); }
		}
		public string TodayI {
			get { return (string)GetValue(TodayIProperty); }
			set { SetValue(TodayIProperty, value); }
		}
		public string TomorrowH {
			get { return (string)GetValue(TomorrowHProperty); }
			set { SetValue(TomorrowHProperty, value); }
		}
		public string TomorrowL {
			get { return (string)GetValue(TomorrowLProperty); }
			set { SetValue(TomorrowLProperty, value); }
		}
		public string TomorrowI {
			get { return (string)GetValue(TomorrowIProperty); }
			set { SetValue(TomorrowIProperty, value); }
		}
		public string DayAfterTomorrowH {
			get { return (string)GetValue(DayAfterTomorrowHProperty); }
			set { SetValue(DayAfterTomorrowHProperty, value); }
		}
		public string DayAfterTomorrowL {
			get { return (string)GetValue(DayAfterTomorrowLProperty); }
			set { SetValue(DayAfterTomorrowLProperty, value); }
		}
		public string DayAfterTomorrowI {
			get { return (string)GetValue(DayAfterTomorrowIProperty); }
			set { SetValue(DayAfterTomorrowIProperty, value); }
		}

		public WeatherInfo(string code, WeatherType type) {
			this.WeatherCode = new WeatherCode(code, type);
		}

		public static readonly DependencyProperty TodayHProperty =
			DependencyProperty.Register("TodayH", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty TodayLProperty =
			DependencyProperty.Register("TodayL", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty TodayIProperty =
			DependencyProperty.Register("TodayI", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty TomorrowHProperty =
			DependencyProperty.Register("TomorrowH", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty TomorrowLProperty =
			DependencyProperty.Register("TomorrowL", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty TomorrowIProperty =
			DependencyProperty.Register("TomorrowI", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty DayAfterTomorrowHProperty =
			DependencyProperty.Register("DayAfterTomorrowH", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty DayAfterTomorrowLProperty =
			DependencyProperty.Register("DayAfterTomorrowL", typeof(string), typeof(WeatherInfo));

		public static readonly DependencyProperty DayAfterTomorrowIProperty =
			DependencyProperty.Register("DayAfterTomorrowI", typeof(string), typeof(WeatherInfo));
	}

	public class WeatherCode {
		public string CityCode { get; set; }
		public WeatherType WeatherType { get; set; }

		public WeatherCode(string code, WeatherType type) {
			this.CityCode = code;
			this.WeatherType = type;
		}

		public override bool Equals(object arg) {
			bool result = false;

			if (arg is WeatherCode) {
				WeatherCode code = arg as WeatherCode;
				result = (string.Compare(code.CityCode, this.CityCode, true) == 0) && (code.WeatherType == this.WeatherType);
			} else {
				result = base.Equals(arg);
			}

			return result;
		}

		public static bool operator ==(WeatherCode arg1, WeatherCode arg2) {
			return object.Equals(arg1, arg2);
		}

		public static bool operator !=(WeatherCode arg1, WeatherCode arg2) {
			return !(arg1 == arg2);
		}
	}

	public enum WeatherType {
		C = 0,
		F
	}
}
