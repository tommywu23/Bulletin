using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using BulletinLibrary;
using Bulletin.Model;
using System.Reflection;
using System.Windows.Threading;

namespace Bulletin {
	public partial class App : Application {
		public bool IsBack { get; set; }

		protected override void OnStartup(StartupEventArgs e) {
			if (File.Exists(valider)) {
				var dll = Assembly.LoadFile(valider);
				Type type = dll.GetType("Config.Valider");
				var d = type.InvokeMember("Do", BindingFlags.DeclaredOnly |
							BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
				if(!Convert.ToBoolean(d)){
					MessageBox.Show("程序已过期...");
					App.Current.Shutdown();
				}
			}

			if (!Funtions.CheckfirstInstance()) {
				MessageBox.Show("已经启动了一个程序。");
				return;
			}

			SplashWindow appSplash = new SplashWindow();
			appSplash.Show();

			try {
				XmlSerializer xsi = new XmlSerializer(typeof(List<ItemBase>));
				using (FileStream fs = new FileStream(System.IO.Path.Combine(App.CONFIGPATH, "Resource/Item.xml"), FileMode.Open)) {
					BroadManager.Instance.Items = xsi.Deserialize(fs) as List<ItemBase>;
				}
			} catch (Exception ex) {
				MessageBox.Show(string.Format("加载信息配置文件失败:{0}", ex.ToString()));
				Application.Current.Shutdown();
			}

			//this.Dispatcher.BeginInvoke(new Action(() => {
			//	appSplash.startlogo.Text = "加载信息配置文件";
			//}));

			try {
				BroadManager.Instance.CurrentWeather = Weather.GetWeather();
			} catch (Exception ex) {
				MessageBox.Show(string.Format("加载天气信息失败，请检查网络:{0}", ex.ToString()));
			}

			//this.Dispatcher.BeginInvoke(new Action(() => {
			//	appSplash.startlogo.Text = "加载天气信息";
			//}));

			Thread.Sleep(TimeSpan.FromSeconds(5));

			this.Dispatcher.BeginInvoke(new Action(() => {
				appSplash.startlogo.Text = "已启动延时加载，请等待30秒";
			}));

			Thread.Sleep(TimeSpan.FromSeconds(30));

			BroadManager.Instance.Load();	

			main = new MainWindow();
			Thread.Sleep(100);
			appSplash.Close();
			main.Show();
			base.OnStartup(e);
		}

		private static MainWindow main;
		private static string valider = System.IO.Path.Combine(Environment.CurrentDirectory, @"config.dll");
		public static string CITYCODE = "2137082";
		public static string CONFIGPATH = System.Environment.CurrentDirectory;
	}
}
