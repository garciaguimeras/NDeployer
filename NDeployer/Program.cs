using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer
{
    class Program
    {
		
		private void PrintHelp()
		{
			Console.WriteLine();
			Console.WriteLine("Usage: NDeployer [option]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("    -f <build file>    Runs a build file");
			Console.WriteLine("    -help              Prints this help");
		}

		private void RunBuildFile(string filename)
		{
			Environment environment = Environment.GetEnvironment();

			ScriptFile scriptFile = ScriptFactory.GetScriptForFilename(filename);
			TaskDef rootTaskDef = scriptFile.Parse(filename);

			RootTask rootTask = new RootTask();
			rootTask.ProcessTaskDef(rootTaskDef);
			if (environment.Pipe.Error.Count() > 0)
			{
				environment.Pipe.PrintErrorPipe();
				return;
			}
			rootTask.Execute();
		}

		private void CheckOptions(string[] args)
		{
			int totalParams = args.Count();
			if (totalParams == 0)
			{
				Console.WriteLine("Error: Need an option");
				PrintHelp();
				return;
			}

			string option = args[0];
			switch (option)
			{
				case "-f":
					if (totalParams < 2)
					{
						Console.WriteLine("Error: Needs a build file name");
						PrintHelp();
						return;
					}
					RunBuildFile(args[1]);
					break;
				
				case "-help":
					PrintHelp();
					break;
				
				default:
					Console.WriteLine("Error: Bad option '{0}'", option);
					PrintHelp();
					break;
			}
		}

		public void Execute(string[] args)
		{
			Console.WriteLine("NDeployer v0.1");
			CheckOptions(args);
		}

        static void Main(string[] args)
        {
			Program program = new Program();
			program.Execute(args);
        }
    }
}
