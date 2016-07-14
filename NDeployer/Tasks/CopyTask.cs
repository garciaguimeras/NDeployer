using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

using NDeployer.Util;

namespace NDeployer.Tasks
{

    class CopyTask : Task
    {

        string deployDir;

		public CopyTask(string name) : base(name)
        {
            deployDir = null;
        }

        public override bool ProcessXml(XElement rootNode)
        {
            deployDir = GetAttribute(rootNode, "todir");
            if (deployDir == null)
                return false;
            return true;
        }

		private List<Dictionary<string, string>> FilterInputPipe(IEnumerable<Dictionary<string, string>> input)
		{
			List<Dictionary<string, string>> notExcluded = new List<Dictionary<string, string>>();
			List<Dictionary<string, string>> included = new List<Dictionary<string, string>>();
			foreach (Dictionary<string, string> data in input)
			{
				if (data.ContainsKey("include") && data["include"].Equals(""))
					included.Add(data);
				if (!data.ContainsKey("exclude") || !data["exclude"].Equals(""))
					notExcluded.Add(data);
			}
			if (included.Count() > 0)
				return included;
			return notExcluded;
		}

        public override void Execute()
        {
            deployDir = PropertyEvaluator.EvalValue(deployDir);
            if (deployDir == null)
            {
                environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
                return;
            }
            
            Logger.info("Deploying to directory: {0}", deployDir);
            if (!Directory.Exists(deployDir))
            {
                Logger.info(2, "Creating new directory: {0}", deployDir);
                Directory.CreateDirectory(deployDir);
            }

			environment.Pipe.KeepPipe();

			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Input;
			input = FilterInputPipe(input);
			foreach (Dictionary<string, string> data in input)
            {
				if (!data.ContainsKey("filename") || !File.Exists(data["filename"])) 
				{
					string name = data.ContainsKey("filename") ? data["filename"] : "";
					environment.Pipe.AddToErrorPipe("Filename does not exist: {0}", name);
					continue;
				}

				string filename = data["filename"];
				bool flatten = data.ContainsKey("flatten") && data["flatten"].Equals("");
				string destDir = !flatten && data.ContainsKey("relativePath") ? data["relativePath"] : ".";

                Logger.info(2, "Deploying file {0}", filename);

				string fName = Path.GetFileName(filename);
                destDir = Path.Combine(deployDir, destDir);
                string destFile = Path.Combine(destDir, fName);

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);
                File.Copy(filename, destFile, true);
            }

        }

    }
}
