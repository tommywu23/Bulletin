using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientContract {
	public class GTAppMessage {
		// spring > app
		public const int OpenApp = 0x9000;
		public const int CloseApp = 0x9001;
		public const int Update = 0x9002;

		// app > spring
		public const int AppClosed = 0x9002;
	}
}
