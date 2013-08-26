using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;


namespace BulletinLibrary {
	public class ItemBase : IDataErrorInfo {
		public string Name { get; set; }
		public int Index { get; set; }
		public string Url { get; set; }
		public string PhotoPath { get; set; }
		public string ExePath { get; set; }
		public string ButtonPath { get; set; }
		public string PlayPath { get; set; }
		public string IsBack { get; set; }
		public string Marquee { get; set; }

		public string Error {
			get { throw new NotImplementedException(); }
		}

		public string this[string columnName] {
			get {
				string result = null;

				if (columnName == "Url") {
					if (IsUrl(Url)) result = "";
					else result = "请正确输入网址";
				}
				return result;
			}
		}

		public static bool IsUrl(string url) {
			return Regex.IsMatch(url, @"([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
		}
	}

	public class Weather : DependencyObject {
		public string Date {
			get { return (string)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		public string Day {
			get { return (string)GetValue(DayProperty); }
			set { SetValue(DayProperty, value); }
		}

		public string WeatherText {
			get { return (string)GetValue(WeatherProperty); }
			set { SetValue(WeatherProperty, value); }
		}

		public string Temp {
			get { return (string)GetValue(TempProperty); }
			set { SetValue(TempProperty, value); }
		}

		public string WeatherImg {
			get { return (string)GetValue(WeatherImgProperty); }
			set { SetValue(WeatherImgProperty, value); }
		}

		public string High {
			get { return (string)GetValue(HighProperty); }
			set { SetValue(HighProperty, value); }
		}

		public string Low {
			get { return (string)GetValue(LowProperty); }
			set { SetValue(LowProperty, value); }
		}

		public string GetDay() {
			string result = DateTime.Now.DayOfWeek.ToString();
			switch (result) {
				case "Monday": result = "星期一"; break;
				case "Tuesday": result = "星期二"; break;
				case "Wednesday": result = "星期三"; break;
				case "Thursday": result = "星期四"; break;
				case "Friday": result = "星期五"; break;
				case "Saturday": result = "星期六"; break;
				case "Sunday": result = "星期日"; break;
			}

			return result;
		}

		public static Weather GetWeather() {
			Weather result = new Weather();

			XmlDocument document = new XmlDocument();
			document.Load("http://xml.weather.yahoo.com/forecastrss?w=2137082&u=c");
			XmlNodeList nodes = document.GetElementsByTagName("forecast", @"http://xml.weather.yahoo.com/ns/rss/1.0");
			result.Date = string.Format("{0:D}", DateTime.Now);
			result.Day = result.GetDay();
			result.WeatherText = nodes[0].Attributes["text"].InnerText;
			result.WeatherImg = string.Format("http://l.yimg.com/a/i/us/nws/weather/gr/{0}d.png", nodes[0].Attributes["code"].InnerText);
			result.Low = string.Format("{0}", nodes[0].Attributes["low"].InnerText);
			result.High = string.Format("{0}", nodes[0].Attributes["high"].InnerText);

			return result;
		}

		public static readonly DependencyProperty TempProperty = DependencyProperty.Register("Temp", typeof(string), typeof(Weather));
		public static readonly DependencyProperty LowProperty = DependencyProperty.Register("Low", typeof(string), typeof(Weather));
		public static readonly DependencyProperty HighProperty = DependencyProperty.Register("High", typeof(string), typeof(Weather));
		public static readonly DependencyProperty WeatherImgProperty = DependencyProperty.Register("WeatherImg", typeof(string), typeof(Weather));
		public static readonly DependencyProperty WeatherProperty = DependencyProperty.Register("Weather", typeof(string), typeof(Weather));
		public static readonly DependencyProperty DayProperty = DependencyProperty.Register("Day", typeof(string), typeof(Weather));
		public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(string), typeof(Weather));
	}

	public class ExeFiles : List<ExeFile> {
		public string Name { get; set; }
	}

	public class ExeFile : DependencyObject {
		public string FileName { get; set; }
		public string Path { get; set; }
		public Point Location { get; set; }
	}

	public class Album {
		public List<Photo> Photos { get; set; }

		public string AlbumName { get; set; }

		public Album() {
			this.Photos = new List<Photo>();
		}
	}

	public class Albums : List<Album> {
		public int Index { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
	}

	public class Photo {
		public int PhotoNumber { get; set; }
		public string ImagePath { get; set; }
		public Album BelongsTo { get; set; }

		public ImageSource Source { get; set; }

		public Photo() { }
		public Photo(string path) {
			var bitmap = new BitmapImage();
			ImagePath = path;
			using (var input = new FileStream(path, FileMode.Open)) {
				bitmap.BeginInit();
				bitmap.StreamSource = input;
				//bitmap.DecodePixelWidth = 961;
				bitmap.DecodePixelHeight = 497;
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.EndInit();
				bitmap.Freeze();
			}

			this.Source = bitmap;
		}
	}
}
