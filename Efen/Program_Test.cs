using System;
using System.Collections.Generic;
using Efen.Kern;
using Efen.Kern.Tasks;
using System.IO;
using Efen.Kern.Filters;

namespace Efen
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//string path = "/tmp/testfileex";
			string path = "/tmp";
			FileWalker walker = new FileWalker (path) {
				RefId = "filelist",
				ReadErrorRefId =  "filelisterrors"
			};

			string tmpfileOk = Path.Combine(Path.GetTempPath (), "efen_files_ok.txt");
			string tmpfileErr = Path.Combine(Path.GetTempPath (), "efen_files_err.txt");

			// FileExistsFileFilter
			using (ConsoleWriterTask cw = new ConsoleWriterTask ())
			{ 
				//cw.FileListRefs.AddRange(new string [] {"filelist", "filelisterrors", "filelist_jpgs", "filelist_jpgs2", "filelist_exists"});
				cw.FileListRefs.AddRange(new string []{"filelist"});
				cw.OutStream = ConsoleWriterTask.OutStreamType.StdOut;
				cw.FormatString = "{listrefid},{fullname},{filename},{filesize}";
				cw.Buffer = 1;

				Task[] tasks = new Task[] { cw };

				SearchPatternFileFilter jpg2Filter = new SearchPatternFileFilter ("*0.jpg");
				jpg2Filter.RefId = "filelist_jpgs2";

				SearchPatternFileFilter jpgFilter = new SearchPatternFileFilter ("*.jpg");
				jpgFilter.RefId = "filelist_jpgs";
				jpgFilter.ChildFilters.Add (jpg2Filter);

				FileExistsFileFilter fileExistsFilter = new FileExistsFileFilter ("{path}/{name}.txt");
				fileExistsFilter.RefId = "filelist_exists";

				AttributeFileFilter attributeFilter = new AttributeFileFilter ();
				attributeFilter.RefId = "filelist_attr";
				attributeFilter.IsDirectory = true;

				FileFilter[] filters = new FileFilter[] {
					jpgFilter,
					fileExistsFilter,
					attributeFilter
				};

				TaskExecutor exec = new TaskExecutor (walker, tasks, filters);

				exec.ExecuteAll ();
			}
		}
	}
}
