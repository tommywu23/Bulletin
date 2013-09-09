using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace BulletinLibrary {
	public class DownloadManager : DispatcherObject {
		public event DownloadHandler Completed;
		public event DownloadHandler Failed;
		public event DownloadHandler ProgressChanged;

		public static DownloadManager Instance { get { return instance; } }

		private DownloadManager() {
			Downloader downloader = new Downloader();
			downloader.Completed += (task) => {
				this.BeginInvoke((DownloadHandler)((o) => {
					if (Completed != null) Completed(o);
					DoWork();
				}), task);
			};
			downloader.Failed += (task) => {
				this.BeginInvoke((DownloadHandler)((o) => {
					TaskManager.Instance.Defer(o);
					if (Failed != null) Failed(o);
					DoWork();
				}), task);
			};
			downloader.ProgressChanged += (task) => {
				this.BeginInvoke((DownloadHandler)((o) => {
					if (ProgressChanged != null) ProgressChanged(o);
				}), task);
			};
			downloaders.Add(downloader);
		}

		public void Start() { }

		public void Stop() {
			exit = true;
			downloaders.ForEach(p => {
				p.Stop();
			});
		}

		public void AddTask(DownloadTask task) {
			this.Invoke((Action)(() => {
				TaskManager.Instance.Add(task);
				DoWork();
			}));
		}

		public void AddTasks(List<DownloadTask> tasks) {
			this.Invoke((Action)(() => {
				TaskManager.Instance.Add(tasks);
				DoWork();
			}));
		}

		public void InsertTask(DownloadTask task) {
			this.Invoke((Action)(() => {
				TaskManager.Instance.Enqueue(task);
				DoWork();
			}));
		}

		public void InsertTasks(List<DownloadTask> tasks) {
			this.Invoke((Action)(() => {
				TaskManager.Instance.Enqueue(tasks);
				DoWork();
			}));
		}

		private void DoWork() {
			if (exit) return;
			DownloadTask task = TaskManager.Instance.Peek();
			Downloader downloader = downloaders.FirstOrDefault(p => !p.IsBusy);
			if (task != null && downloader != null) {
				downloader.BeginStart(task);
			}
		}

		private void Invoke(Action action) {
			this.Dispatcher.Invoke(action);
		}

		private void BeginInvoke(DownloadHandler action, params object[] args) {
			this.Dispatcher.BeginInvoke(action, args);
		}

		private static DownloadManager instance = new DownloadManager();
		private List<Downloader> downloaders = new List<Downloader>();
		private const int count = 5;
		private bool exit;
	}

	public class TaskManager {
		public static TaskManager Instance { get { return instance; } }

		private TaskManager() { }

		public void Add(DownloadTask task) {
			tasks.Add(task);
		}

		public void Add(List<DownloadTask> arg) {
			tasks.AddRange(arg);
		}

		public void Enqueue(DownloadTask task) {
			tasks.Insert(0, task);
		}

		public void Enqueue(List<DownloadTask> arg) {
			tasks.InsertRange(0, arg);
		}

		public DownloadTask Peek() {
			return tasks.FirstOrDefault(p => p.Status == TaskStatus.Waitting);
		}

		public void Defer(DownloadTask task) {
			tasks.Remove(task);
			if (task.Defer()) tasks.Add(task);
		}

		private static TaskManager instance = new TaskManager();
		private List<DownloadTask> tasks = new List<DownloadTask>();
	}
}
