using System;
using System.IO;
using System.Collections.Generic;

namespace Efen.Kern.Filters
{
	public class SearchPatternFileFilter : FileFilter
	{
		// http://msdn.microsoft.com/en-us/library/wz42302f%28v=vs.110%29.aspx
		public List<string> Patterns { get; private set; }

		public SearchPatternFileFilter (params string[] patterns)
		{
			this.Patterns = new List<string> (patterns);
		}

		public SearchPatternFileFilter (IEnumerable<string> patterns)
		{
			this.Patterns = new List<string> (patterns);
		}

		public override bool IsMatch (FileSystemInfo file)
		{
			// TODO implement SearchPattern like Directory.GetFiles
			foreach (string p in this.Patterns) {
				if (file.FullName.ToLowerInvariant ().EndsWith (p.Replace ("*", "").ToLowerInvariant ())) {
					return !this.Invert;
				}
			}
			return this.Invert;
		}
	}
}