using System;
using System.Collections.Generic;

namespace Efen.Kern
{
	public class Configuration
	{
		public string ConfigPath { get; private set; }

		public Configuration (string configpath)
		{
			this.ConfigPath = configpath;
		}

		public List<Task> LoadTasks()
		{
			throw new NotImplementedException ();
		}
	}
}