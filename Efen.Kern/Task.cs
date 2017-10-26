using System;
using System.IO;
using System.Collections.Generic;

namespace Efen.Kern
{
	public abstract class Task : IDisposable
	{
		protected class FileItem
		{
			public FileSystemInfo File { get; set; }
			public string ListRefId { get; set; }

			public FileItem(FileSystemInfo file, string listRefId) {
				this.File = file;
				this.ListRefId = listRefId;
			}

			public string ToString(string formatstring)
			{
				FileInfo fileInfo = !this.File.Attributes.HasFlag(FileAttributes.Directory) ? this.File as FileInfo : null;
				string txt = formatstring;
				txt = txt.Replace ("{fullname}", this.File.FullName);
				txt = txt.Replace ("{filename}", this.File.Name);
				txt = txt.Replace ("{filesize}", fileInfo != null ? fileInfo.Length.ToString() : "-1");
				txt = txt.Replace ("{listrefid}", this.ListRefId);
				txt = txt.Replace ("{dirname}", Path.GetFileName(Path.GetDirectoryName(this.File.FullName)));
				txt = txt.Replace ("{fulldirname}", Path.GetDirectoryName(this.File.FullName));
				return txt;
			}
		}

		public List<string> FileListRefs { get; private set;}
		public abstract bool RunParallel { get; }

		private int buffer = 100;
		public int Buffer {
			get { return this.buffer; }
			set {
				if (value < 0) {
					throw new InvalidOperationException ("Buffer cannot be lower than 0.");
				}
				this.buffer = value;
			}
		}

		protected Queue<FileItem> queue;

		public Task() {
			this.FileListRefs = new List<string> ();
		}

		public virtual void Execute (FileSystemInfo file, string sourceRefId)
		{  
			if (this.queue == null) {
				if (this.buffer > 0) {
					this.queue = new Queue<FileItem> (this.Buffer);
				} else {
					this.queue = new Queue<FileItem> ();
				}
			}
			if (this.buffer > 0 && this.queue.Count >= this.Buffer) {
				this.Flush ();
			}
			queue.Enqueue (new FileItem(file, sourceRefId));
		}

		public virtual void Flush ()
		{ 
			if (this.queue == null) {
				return;
			}
			this.Flush (this.queue);
		}

		protected abstract void Flush (Queue<FileItem> queue);

		public virtual void Dispose () 
		{

		}
	}
}