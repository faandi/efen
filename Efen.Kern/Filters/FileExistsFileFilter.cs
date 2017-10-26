using System;
using System.IO;
using System.Collections.Generic;

namespace Efen.Kern.Filters
{
	public class FileExistsFileFilter : FileFilter
	{
		public bool IsDirectory { get; set; }
		public List<string> FilesExist { get; private set; }

		public FileExistsFileFilter (params string[] filepaths)
		{
			this.FilesExist = new List<string> (filepaths);
		}

		public FileExistsFileFilter (IEnumerable<string> filepaths)
		{
			this.FilesExist = new List<string> (filepaths);
		}

		public override bool IsMatch (FileSystemInfo file)
		{
			foreach (string p in this.FilesExist) {
				string filepath = p;
				filepath = filepath.Replace ("{path}", Path.GetDirectoryName(file.FullName));
				filepath = filepath.Replace ("{name}", Path.GetFileNameWithoutExtension(file.FullName));
				filepath = filepath.Replace ("{ext}", Path.GetExtension(file.FullName));
				if (file.FullName == filepath && this.FilesExist.Count == 1) {
					return this.Invert;
				} else if (file.FullName == filepath) {
					continue;
				}
				if (this.IsDirectory && !Directory.Exists (filepath)) {
					return this.Invert;
				}
				if (!this.IsDirectory && !File.Exists (filepath)) {
					return this.Invert;
				}
			}
			return !this.Invert;
		}
	}
}