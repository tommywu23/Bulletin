using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BulletinStart {
	public class Program {
		static void Main(string[] args) {
			Thread.Sleep(TimeSpan.FromSeconds(75));
			var pro = Process.GetProcesses().Where(t => t.ProcessName == Path.GetFileNameWithoutExtension("数字校园平台")).FirstOrDefault();
			if (pro != null) {
				IntPtr handle = ProcessEx.FindWindow(null, "BuulletinApp");
				ProcessEx.SetWindowPos(handle);
			} else {
				Console.WriteLine("数字校园平台不在运行，请双击数字校园平台.exe启动程序");
			}
			//Console.WriteLine("Press enter key to close");
			//Console.ReadLine();
			return;			
		}
	}

	public static class ProcessEx {
		private static class NativeMethods {
			internal const uint GW_OWNER = 4;

			internal delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

			[DllImport("User32.dll", CharSet = CharSet.Auto)]
			internal static extern bool IsWindowVisible(IntPtr hWnd);

			[DllImport("User32.dll", EntryPoint = "SetWindowPos")]
			internal static extern int SetWindowPos(IntPtr win_handle, int hWndInsertAfter, int x, int y, int width, int height, uint flags);

			[DllImport("user32.dll", SetLastError = true)]
			internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		}

		public static IntPtr FindWindow(string lpClassName, string lpWindowName) {

			return NativeMethods.FindWindow(lpClassName, lpWindowName);
		}

		public static IntPtr GetWindow(IntPtr handle, uint cmd) {

			return NativeMethods.GetWindow(handle, cmd);
		}

		public static void SetWindowPos(IntPtr handle) {
			NativeMethods.SetWindowPos(handle, -1, 0, 0, 1366, 768, 0x0040);
		}

		public static IntPtr GetHandle(int processId) {
			IntPtr MainWindowHandle = IntPtr.Zero;

			NativeMethods.EnumWindows(new NativeMethods.EnumWindowsProc((hWnd, lParam) => {
				IntPtr PID;
				NativeMethods.GetWindowThreadProcessId(hWnd, out PID);

				if (PID == lParam &&
					NativeMethods.IsWindowVisible(hWnd) &&
					NativeMethods.GetWindow(hWnd, NativeMethods.GW_OWNER) == IntPtr.Zero) {
					MainWindowHandle = hWnd;
					return false;
				}

				return true;

			}), new IntPtr(processId));

			return MainWindowHandle;
		}
	}
}
