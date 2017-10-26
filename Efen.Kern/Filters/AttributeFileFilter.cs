using System;
using System.IO;
using System.Collections.Generic;

namespace Efen.Kern.Filters
{
	public class AttributeFileFilter : FileFilter
	{
		// TODO more conditions
		// datecreated, datemodified 

		public bool? IsDirectory { get; set; }
		public int MinFileSize { get; set; }
		public int MaxFileSize { get; set; }

		public AttributeFileFilter ()
		{
		}

		public override bool IsMatch (FileSystemInfo file)
		{
			bool allmatch = true;
			if (this.IsDirectory.HasValue) {
				bool isdir = file.Attributes.HasFlag (FileAttributes.Directory);
				allmatch = allmatch && this.IsDirectory.Value ? isdir : !isdir;
			}
			if (this.MinFileSize > 0 && file is FileInfo) {
				allmatch = allmatch && ((FileInfo)file).Length >= this.MinFileSize;
			}
			if (this.MaxFileSize > 0 && file is FileInfo) {
				allmatch = allmatch && ((FileInfo)file).Length <= this.MaxFileSize;
			}
			return allmatch;
		}
	}
}