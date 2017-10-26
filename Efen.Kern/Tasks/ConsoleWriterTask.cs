using System;
using System.Collections.Generic;
using System.IO;

namespace Efen.Kern.Tasks
{
	public class ConsoleWriterTask : Task
	{
		public enum OutStreamType {
			StdErr,
			StdOut
		}

		public override bool RunParallel { get{ return true; } }
		public OutStreamType OutStream { 
			get { 
				if (this.writer == Console.Out) {
					return OutStreamType.StdOut;
				} else if (this.writer == Console.Error) {				
					return OutStreamType.StdErr;
				}
				throw new InvalidOperationException ();
			}
			set { 
				switch (value) {
				case OutStreamType.StdOut:
					this.writer = Console.Out;
					break;
				case OutStreamType.StdErr:
					this.writer = Console.Error;
					break;
				default:
					throw new InvalidOperationException ();
				}
			}
		}

		string formatstring = "{fullname}";
		public string FormatString {
			get{ 
				return this.formatstring;
			}
			set {
				if (string.IsNullOrEmpty (value)) {
					throw new InvalidOperationException ("FormatString cannot be null or empty");
				}
				this.formatstring = value;
			}
		}

		private TextWriter writer = Console.Out;

		protected override void Flush (Queue<FileItem> queue) { 
			while (queue.Count > 0) {
				FileItem f = queue.Dequeue ();
				writer.WriteLine (f.ToString(this.FormatString));
			}
		}
	}
}