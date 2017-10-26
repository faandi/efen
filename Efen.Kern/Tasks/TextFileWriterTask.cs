using System;
using System.Collections.Generic;
using System.IO;

namespace Efen.Kern.Tasks
{
	public class TextFileWriterTask : Task
	{
		public override bool RunParallel { get{ return true; } }
		private string filePath;
		public string FilePath { 
			get { 
				return this.filePath;
			}
			set { 
				if (string.IsNullOrEmpty(value)) {
					throw new InvalidOperationException ();
				}
				this.filePath = value;
			}
		}

		string formatstring = "{fullname}";
		public string FormatString {
			get{ 
				return this.formatstring;
			}
			set {
				if (string.IsNullOrEmpty (value)) {
					throw new InvalidOperationException ("FormatString cannot be null or empty");
				}
				this.formatstring = value;
			}
		}

		public TextFileWriterTask(string filepath, bool append = false) {
			this.FilePath = filepath;
			if (!append && File.Exists (filepath)) {
				throw new ArgumentException ("File already exists");
			}
			using (FileStream s = File.Create (filepath)) {
			};
		}

		protected override void Flush (Queue<FileItem> queue) { 
			using (TextWriter writer = File.AppendText (this.FilePath)) {
				while (queue.Count > 0) {
					FileItem f = queue.Dequeue ();
					writer.WriteLine (f.ToString(this.FormatString));
				}
			}
		}
	}
}