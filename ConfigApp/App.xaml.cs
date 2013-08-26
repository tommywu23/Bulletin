using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ConfigApp.Model;

namespace ConfigApp {
	public partial class App : Application {
		protected override void OnStartup(StartupEventArgs e) {
			ConfigManager.Instance.Load();
			base.OnStartup(e);
		}
	}
}
