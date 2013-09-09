using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace ClientContract.Helper {
	public static class VSDesigner {
		public static bool IsDesignMode {
			get { return isDesignMode; }
		}

		static VSDesigner() {
			var prop = DesignerProperties.IsInDesignModeProperty;
			isDesignMode = (bool)DependencyPropertyDescriptor.
					FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
		}

		private static readonly bool isDesignMode;
	}
}
