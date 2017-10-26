using NUnit.Framework;
using System;
using Efen.Kern;

namespace Efen.Tests
{
	[TestFixture ()]
	public class FileWalkerTests
	{
		[Test ()]
		public void TestCase ()
		{
			string path = "/tmp";
			FileWalker walker = new FileWalker (path);
			foreach (string file in walker.Walk()) {
				Console.Write (file);
			}
		}
	}
}

