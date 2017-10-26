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
			string outPath = args.Length > 1 ? args[1] : "/tmp/efenreport";
			string inPath = args.Length > 1 ? args[0] : "/tmp/GeodataTestFiles";

			FileWalker walker = new FileWalker (inPath) {
				RefId = "allFiles",
				ReadErrorRefId = "fileErrors"
			};
			// get configs
			Config[] configs = new Config[] {
				ErrorConfig(outPath,walker.ReadErrorRefId), 
				EsriShapefileConfig(outPath),
				EsriTilePackage(outPath),
				GeoTif(outPath),
				EsriFileGeodatabase(outPath),
				GpxConfig(outPath),
				EsriCache(outPath),
				DwgDxfConfig(outPath),
				KmlConfig(outPath),
				GmlConfig(outPath)
			};
			// build inputs
			List<Task> tasks = new List<Task> ();
			List<FileFilter> filters = new List<FileFilter> ();
			foreach (Config c in configs) {
				// add tasks and filters
				tasks.AddRange (c.Tasks);
				filters.AddRange (c.Filters);
			}
			// do the work
			TaskExecutor exec = new TaskExecutor (walker, tasks, filters);
			exec.ExecuteAll ();
		}

		private class Config 
		{
			public IEnumerable<FileFilter> Filters {get;set;}
			public IEnumerable<Task> Tasks {get;set;}
			public string Outfile {get;set;}
		}

		private static Config ErrorConfig(string outPath, string refid) {

			string outfile = Path.Combine (outPath, "errors.txt");			
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500,
				FilePath = outfile 
			};
			task.FileListRefs.Add (refid);
			return new Config () { 
				Filters = new FileFilter[] { },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config EsriShapefileConfig(string outPath) {

			string outfile = Path.Combine (outPath, "esriShapefile.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("esriShapefile");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.shp") { 
				RefId = "esriShapefile"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config EsriTilePackage(string outPath) {
			string outfile = Path.Combine (outPath, "esriTilePackage.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("esriTilePackage");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.tpk") { 
				RefId = "esriTilePackage"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config GeoTif(string outPath) {
			string outfile = Path.Combine (outPath, "geoTif.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("geoTif");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.tif");
			FileExistsFileFilter filter2 = new FileExistsFileFilter ("{path}"+Path.DirectorySeparatorChar+"{name}.txt"){
				RefId = "geoTif"
			};
			filter.ChildFilters.Add (filter2);
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config EsriFileGeodatabase(string outPath) {
			string outfile = Path.Combine (outPath, "esriFileGeodatabase.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500,
				FormatString = "{fulldirname}"
			};
			task.FileListRefs.Add ("esriFileGeodatabase");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.gdbtable") { 
				RefId = "esriFileGeodatabase"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config GpxConfig(string outPath) {
			string outfile = Path.Combine (outPath, "gpx.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("gpx");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.gpx") { 
				RefId = "gpx"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config EsriCache(string outPath) {
			string outfile = Path.Combine (outPath, "esriCache.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500,
				FormatString = "{fulldirname}"
			};
			task.FileListRefs.Add ("esriCache");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("conf.xml");
			FileExistsFileFilter filter2 = new FileExistsFileFilter ("{path}"+Path.DirectorySeparatorChar+"_alllayers"){
				RefId = "esriCache",
				IsDirectory = true
			};
			filter.ChildFilters.Add (filter2);
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config DwgDxfConfig(string outPath) {
			string outfile = Path.Combine (outPath, "dwgDxf.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("dwg");
			task.FileListRefs.Add ("dxf");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.dwg") { 
				RefId = "dwg"
			};
			SearchPatternFileFilter filter2 = new SearchPatternFileFilter ("*.dxf") { 
				RefId = "dxf"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter, filter2 },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config KmlConfig(string outPath) {
			string outfile = Path.Combine (outPath, "kml.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("kml");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.kml") { 
				RefId = "kml"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

		private static Config GmlConfig(string outPath) {
			string outfile = Path.Combine (outPath, "gml.txt");
			TextFileWriterTask task = new TextFileWriterTask (outfile, false) {
				Buffer = 500
			};
			task.FileListRefs.Add ("gml");
			SearchPatternFileFilter filter = new SearchPatternFileFilter ("*.gml") { 
				RefId = "gml"
			};
			return new Config () { 
				Filters = new FileFilter[] { filter },
				Tasks = new Task[] { task },
				Outfile = outfile
			};
		}

	}
}
