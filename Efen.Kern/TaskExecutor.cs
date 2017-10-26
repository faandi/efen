using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Efen.Kern
{
	public class TaskExecutor
	{
		private FileWalker walker;
		private IEnumerable<FileFilter> filters;
		private IEnumerable<Task> tasks;

		private Dictionary<string,IEnumerable<Task>> tasksByRefId = new Dictionary<string, IEnumerable<Task>> ();
		private List<string> refIds;

		public TaskExecutor (FileWalker walker, IEnumerable<Task> tasks, IEnumerable<FileFilter> filters = null)
		{
			this.filters = filters ?? new FileFilter[0];
			this.walker = walker;
			this.tasks = tasks;

			this.refIds = this.GetRefIds (this.filters);
			if (!string.IsNullOrEmpty (this.walker.ReadErrorRefId)) {
				this.refIds.Add (this.walker.ReadErrorRefId);
			}
			if (!string.IsNullOrEmpty (this.walker.RefId)) {
				this.refIds.Add (this.walker.RefId);
			}

			foreach (Task t in tasks) {
				foreach (string refid in t.FileListRefs) {
					if (!this.refIds.Contains (refid)) {
						string msg = string.Format ("FileListRef {0} specified in task not found.", refid);
						throw new InvalidOperationException (msg);
					}
				}
			}

			foreach (string rid in this.refIds) {
				IEnumerable<Task> tasksToRun = tasks.Where (t => t.FileListRefs.Contains(rid));
				if (tasks.Count () > 0) {
					this.tasksByRefId.Add (rid, tasksToRun.ToArray());
				}
			}

		}

		private List<string> GetRefIds(IEnumerable<FileFilter> filters) {
			List<string> refids = new List<string>();
			foreach (FileFilter f in filters) {
				if (!string.IsNullOrEmpty(f.RefId) && !refids.Contains (f.RefId)) {
					refids.Add (f.RefId);
				}
				foreach (string subrefid in GetRefIds(f.ChildFilters)) {
					if (!string.IsNullOrEmpty(subrefid) && !refids.Contains (subrefid)) {
						refids.Add (subrefid);
					}
				}
			}
			return refids;
		}

		public void ExecuteAll()
		{
			this.walker.FileReadError += (object sender, FileErrorEventArgs e) => {
				this.ExecuteTasks(this.walker.ReadErrorRefId, e.Path);
			};
			foreach (string path in this.walker.Walk()) {
				FileSystemInfo file = new FileInfo (path);
				this.ExecuteTasks (this.walker.RefId, path);
				foreach (FileFilter filter in this.filters) {
					this.ExcecuteTasks (filter, file);
				}
			}

			foreach (Task t in this.tasks) {
				t.Flush ();
			}
		}

		private void ExcecuteTasks (FileFilter filter, FileSystemInfo file) {
			if (filter.IsMatch (file)) {
				this.ExecuteTasks (filter.RefId, file.FullName);
				if (filter.ChildFilters != null) {
					foreach (FileFilter childfilter in filter.ChildFilters) {
						this.ExcecuteTasks (childfilter, file);
					}
				}
			}
		}

		private void ExecuteTasks(string refid, string path) {
			if (!string.IsNullOrEmpty(refid) && this.tasksByRefId.Keys.Contains (refid)) {
				FileInfo file = new FileInfo (path);
				foreach (Task task in this.tasksByRefId[refid]) {
					task.Execute (file, refid);
				}
			}
		}
	}
}