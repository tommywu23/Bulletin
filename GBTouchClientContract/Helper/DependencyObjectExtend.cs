using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ClientContract.Helper {
	public static class DependencyObjectExtend {
		public static T FindParent<T>(this DependencyObject element) where T : DependencyObject {
			while (element != null && !(element is T)) {
				element = VisualTreeHelper.GetParent(element);
			}
			return (T)element;
		}

		public static T FindChild<T>(this DependencyObject parent) where T : DependencyObject {
			if (parent == null) return null;
			T childElement = null;
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++) {
				var child = VisualTreeHelper.GetChild(parent, i);

				T childType = child as T;
				if (childType == null) {
					childElement = FindChild<T>(child);
					if (childElement != null) break;
				} else {
					childElement = (T)child; break;
				}
			}

			return childElement;
		}

		public static DependencyObject PrintParent(this DependencyObject element) {
			while (element != null) {
				element = VisualTreeHelper.GetParent(element);
				Console.WriteLine(element.GetType().Name);
			}
			return element;
		}

		public static DependencyObject PrintChild(this DependencyObject parent) {
			if (parent == null) return null;

			DependencyObject childElement = null;
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++) {
				var child = VisualTreeHelper.GetChild(parent, i);
				Console.WriteLine(child.GetType().Name);

				childElement = PrintChild(child);
				if (childElement != null) break;
			}

			return childElement;
		}
	}
}
