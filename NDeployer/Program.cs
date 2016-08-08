using System;
using System.IO;
using System.Linq;
using System.Reflection;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer
{
	class ProgramInfo
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public string Copyright { get; set; }
	}

	enum ProgramFlag
	{
		EXECUTE,
		INFO
	}

    class Program
    {

		const string LICENSE = "Released under GNU General Public License";

		private ProgramInfo GetAssemblyInfo()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = assembly.GetName();
			object[] description = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
			object[] copyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			return new ProgramInfo
			{
				Name = assemblyName.Name,
				Version = assemblyName.Version.ToString(),
				Description = description.Length > 0 ? (description[0] as AssemblyDescriptionAttribute).Description : "",
				Copyright = copyright.Length > 0 ? (copyright[0] as AssemblyCopyrightAttribute).Copyright : "",
			};
		}

		private void PrintHelp()
		{
			Console.WriteLine("Usage: NDeployer [option] [arg-list]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("    -f <build file> [flag]   Runs a build file");
			Console.WriteLine("    -help                    Prints this help");
			Console.WriteLine("    -info                    Prints app version and copyright information");
			Console.WriteLine();
			Console.WriteLine("Flags:");
			Console.WriteLine("    -i                       Print meta attributes defined on build file");
			Console.WriteLine();
			Console.WriteLine("Argument List:");
			Console.WriteLine("    arg-list                 A list of arguments separated by spaces");
		}

		private void PrintMetaAttributes()
		{
			Environment environment = Environment.GetEnvironment();
			if (environment.MetaAttributes.Keys.Count == 0)
				Console.WriteLine("No meta attributes found");
			else
			{
				foreach (string k in environment.MetaAttributes.Keys)
				{
					Console.WriteLine("{0}: {1}", k, environment.MetaAttributes[k]);
				}
			}
			Console.WriteLine();
		}

		private string[] GetBuildFileArgs(string[] args, int startingPos)
		{
			if (startingPos >= args.Length)
				return new string[] { };

			string[] copy = new string[args.Length - startingPos];
			for (int i = startingPos; i < args.Length; i++)
				copy[i - startingPos] = args[i];
			return copy;
		}

		private void RunBuildFile(string filename, ProgramFlag flag, params string[] argList)
		{
			Environment environment = Environment.GetEnvironment();

			if (!File.Exists(filename))
			{
				Console.WriteLine("Error: File not found {0}", filename);
				return;
			}
			ScriptFile scriptFile = ScriptFactory.GetScriptForFilename(filename);
			if (scriptFile == null)
			{
				Console.WriteLine("Error: Invalid file type or extension {0}", filename);
				return;
			}

			TaskDef rootTaskDef = scriptFile.Parse(filename);
			if (rootTaskDef == null)
			{
				Console.WriteLine("Error: Could not parse file {0}", filename);
				return;
			}

			RootTask rootTask = new RootTask(rootTaskDef, argList);
			rootTask.IsValidTaskDef();
			if (environment.Errors.Count() > 0)
			{
				environment.PrintErrorList();
				return;
			}
				
			if (flag == ProgramFlag.INFO)
			{
				rootTask.LoadMetaAttributes(rootTaskDef.Children);
				PrintMetaAttributes();
				return;
			}

			rootTask.LoadArguments();
			rootTask.Execute();
			if (environment.Errors.Count() > 0)
				environment.PrintErrorList();
		}

		private void CheckOptions(string[] args, ProgramInfo programInfo)
		{
			int totalParams = args.Count();
			if (totalParams == 0)
			{
				Console.WriteLine("Error: Missing option");
				Console.WriteLine();
				PrintHelp();
				return;
			}

			string option = args[0];
			switch (option)
			{
				case "-f":
					if (totalParams < 2)
					{
						Console.WriteLine("Error: Missing build file name");
						Console.WriteLine();
						PrintHelp();
						return;
					}

					ProgramFlag flag = ProgramFlag.EXECUTE;
					if (totalParams > 2)
					{
						switch (args[2])
						{
							case "-i":
								flag = ProgramFlag.INFO;
								break;
						}
					}

					RunBuildFile(args[1], flag, GetBuildFileArgs(args, 2));
					break;
				
				case "-help":
					PrintHelp();
					break;

				case "-info":
					Console.WriteLine(programInfo.Copyright);
					Console.WriteLine(LICENSE);
					Console.WriteLine();
					break;
				
				default:
					Console.WriteLine("Error: Unrecognized option '{0}'", option);
					PrintHelp();
					break;
			}
		}

		public void Execute(string[] args)
		{
			ProgramInfo programInfo = GetAssemblyInfo();
			Console.WriteLine("{0} v{1}", programInfo.Name, programInfo.Version);
			Console.WriteLine(programInfo.Description);
			Console.WriteLine();
			CheckOptions(args, programInfo);
		}

        static void Main(string[] args)
        {
			Program program = new Program();
			program.Execute(args);
        }
    }
}
