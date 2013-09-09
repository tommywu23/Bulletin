using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ClientContract {
	public partial class GTAppBase : Application {
		protected override void OnStartup(StartupEventArgs e) {
			string appName = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).FileDescription;
			if (!SingleApplication.Open(appName)) {
				Application.Current.Shutdown();
				return;
			} else {
				base.OnStartup(e);
			}
		}
		protected override void OnExit(ExitEventArgs e) {
			SingleApplication.Close();
			base.OnExit(e);
		}
	}

	public static class SingleApplication {
		public static bool Open(string appName) {
			bool isFirstInstance;
			mutex = new Mutex(true, appName, out isFirstInstance);
			return isFirstInstance;
		}
		public static void Close() {
			if (mutex != null) mutex.Close();
		}

		private static Mutex mutex = null;
	}
}
