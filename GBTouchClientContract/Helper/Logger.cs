using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientContract.Helper {
	public class Logger {
		public static void Debug(object sender, string msg) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Write(sender.GetType().Name, msg);
			Console.ForegroundColor = defaultConsoleColor;
		}

		public static void Info(object sender, string msg) {
			Write(sender.GetType().Name, msg);
		}

		public static void Warn(object sender, string msg) {
			Console.ForegroundColor = ConsoleColor.Red;
			Write(sender.GetType().Name, msg);
			Console.ForegroundColor = defaultConsoleColor;
		}

		public static void Error(object sender, string msg) {
			Write(sender.GetType().Name, msg);
		}

		public static void Debug(Type sender, string msg) {
			Console.ForegroundColor = ConsoleColor.Yellow;
			Write(sender.Name, msg);
			Console.ForegroundColor = defaultConsoleColor;
		}

		private static void Write(string sender, string msg) {
			Console.WriteLine("{0}: {1}", sender, msg);
		}

		static Logger() {
			defaultConsoleColor = Console.ForegroundColor;
		}

		private static ConsoleColor defaultConsoleColor;
	}
}
