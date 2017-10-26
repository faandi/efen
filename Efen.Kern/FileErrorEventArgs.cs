using System;

namespace Efen.Kern
{
	public class FileErrorEventArgs : EventArgs
	{
		public string Path { get; private set;}
		public Exception Exception { get; private set;}
		public bool Continue { get; set;}

		public FileErrorEventArgs(string path, Exception ex)
		{
			this.Path = path;
			this.Exception = ex;
			this.Continue = true;
		}
	}
}