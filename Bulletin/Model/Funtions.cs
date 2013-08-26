using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Bulletin.Model {
	public class Funtions {
		public static bool CheckfirstInstance() {
			bool isFirstInstance;
			Mutex mutex = new Mutex(true, Application.ProductName, out isFirstInstance);
			if (isFirstInstance == false) {
				return false;
			}
			Application.ApplicationExit += new EventHandler((o, b) => { mutex.ReleaseMutex(); });
			return true;
		}
	}
}
