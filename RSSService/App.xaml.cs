using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace RSSService {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (File.Exists(valider))
            {
                var dll = Assembly.LoadFile(valider);
                Type type = dll.GetType("Config.Valider");
                var d = type.InvokeMember("Do", BindingFlags.DeclaredOnly |
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, null);
                if (!Convert.ToBoolean(d))
                {
                    MessageBox.Show("程序已过期，请联系开发公司...");
                    App.Current.Shutdown();
                }
            }
            base.OnStartup(e);
        }

        private static string valider = System.IO.Path.Combine(Environment.CurrentDirectory, @"config.dll");
	}
}
