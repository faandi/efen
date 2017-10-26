using System;
using System.Collections.Generic;
using System.IO;
using Efen.Kern;

namespace Efen.Kern
{
	public class FileWalker
	{
		public string BaseDir { get; private set;}
		//public int Buffer { get; private set;}
		public string ReadErrorRefId { get; set;}
		public string RefId { get; set; }

		public event EventHandler<FileErrorEventArgs> FileReadError;

		public FileWalker (string basedir)
		{
			this.BaseDir = basedir;
		}

		public IEnumerable<string> Walk() {
			foreach (string path in this.GetFiles(this.BaseDir, true)) {
				yield return path;
			}
		}

		// http://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
		private IEnumerable<string> GetFiles (string path, bool returndirs = false) {
			Queue<string> queue = new Queue<string> ();
			queue.Enqueue (path);
			while (queue.Count > 0) {
				path = queue.Dequeue ();
				try {
					foreach (string subDir in Directory.GetDirectories(path)) {
						queue.Enqueue (subDir);
					}
				} catch (Exception ex) {
					this.OnFileReadError (path, ex);
					//Console.Error.WriteLine (ex);
				}
				if (returndirs) {
					yield return path;
				}
				string[] files = null;
				try {
					files = Directory.GetFiles (path);
				} catch (Exception ex) {
					this.OnFileReadError (path, ex);
					//Console.Error.WriteLine (ex);
				}
				if (files != null) {
					for (int i = 0; i < files.Length; i++) {
						yield return files [i];
					}
				}
			}
		}

		private void OnFileReadError(string path, Exception exception) {
			FileErrorEventArgs eventArgs = new FileErrorEventArgs (path, exception);
			EventHandler<FileErrorEventArgs> handler = this.FileReadError; 
			if (handler != null) {
				//List<Exception> thrownEx = new List<Exception> ();
				Delegate[] eventHandlers = handler.GetInvocationList();
				foreach (Delegate currentHandler in eventHandlers) {
					EventHandler<FileErrorEventArgs> currentSubscriber = (EventHandler<FileErrorEventArgs>)currentHandler;
					try {
						currentSubscriber(this, eventArgs);
					}
					catch (Exception ex) {
						//thrownEx.Add (ex);
						throw;
					}					 
				}
				//if (thrownEx.Count > 0) {
				//	throw thrownEx [0];
				//}
			}
		}

	}
}