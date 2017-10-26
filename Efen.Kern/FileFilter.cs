using System;
using System.Collections.Generic;
using System.IO;

namespace Efen.Kern
{
	public abstract class FileFilter
	{
		public string RefId { get; set; }
		public List<FileFilter> ChildFilters { get; private set; }
		public bool Invert { get; set; }

		public FileFilter ()
		{
			this.ChildFilters = new List<FileFilter> ();
			this.Invert = false;
		}

		public abstract bool IsMatch (FileSystemInfo file);
	}
}