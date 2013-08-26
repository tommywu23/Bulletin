using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace Bulletin.Model {
	public class WeatherProvider2 : DependencyObject {
		public static WeatherProvider2 Instance { get { return instance; } }

		public TimeSpan Interval1 { get; set; }
		public TimeSpan Interval2 { get; set; }

		private WeatherProvider2() { }

		public void Start() {
			Interval1 = TimeSpan.FromMinutes(1);
			Interval2 = TimeSpan.FromHours(1);
		}

		public WeatherInfo GetInfo(string code, WeatherType type) {
			WeatherInfo result = new WeatherInfo(code, type);

			if (code2info.ContainsKey(result.WeatherCode)) {
				result = code2info[result.WeatherCode];
			} else {
				code2info.Add(result.WeatherCode, result);
				//BeginUpdate();
			}

			return result;
		}

		public void DoWork() {
			foreach (var each in code2info) {
				try {
					DoWork(each.Value);
				} catch (Exception ex) {
					//throw new ProviderTimeOutException(ex);
				}
			}
		}

		private void DoWork(WeatherInfo info) {
			string url = string.Format(@"http://xml.weather.yahoo.com/forecastrss/{0}_{1}.xml", info.WeatherCode.CityCode, info.WeatherCode.WeatherType.ToString().ToLower());
			XmlDocument document = new XmlDocument();
			document.Load(url);
			XmlNodeList nodes = document.GetElementsByTagName("forecast", @"http://xml.weather.yahoo.com/ns/rss/1.0");

			this.Dispatcher.Invoke(new Action(() => {
				// Today
				info.TodayH = nodes[0].Attributes["high"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				info.TodayL = nodes[0].Attributes["low"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				info.TodayI = string.Format("http://l.yimg.com/a/i/us/nws/weather/gr/{0}d.png", nodes[0].Attributes["code"].InnerText);

				//// Tomorrow
				//info.TomorrowH = nodes[1].Attributes["high"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				//info.TomorrowL = nodes[1].Attributes["low"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				//info.TomorrowI = string.Format("http://l.yimg.com/a/i/us/nws/weather/gr/{0}d.png", nodes[1].Attributes["code"].InnerText);

				//// DayAfterTomorrow
				//info.DayAfterTomorrowH = nodes[2].Attributes["high"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				//info.DayAfterTomorrowL = nodes[2].Attributes["low"].InnerText + "°" + info.WeatherCode.WeatherType.ToString();
				//info.DayAfterTomorrowI = string.Format("http://l.yimg.com/a/i/us/nws/weather/gr/{0}d.png", nodes[2].Attributes["code"].InnerText);
			}));
		}

		private static WeatherProvider2 instance = new WeatherProvider2();
		private Dictionary<WeatherCode, WeatherInfo> code2info = new Dictionary<WeatherCode, WeatherInfo>();
	}
}
