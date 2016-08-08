using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{

    class CopyTask : ContextTask
    {

        string deployDir;

		public CopyTask(TaskDef taskDef) : base(taskDef)
        {
            deployDir = null;
        }

		public override bool IsValidTaskDef()
        {
            deployDir = GetAttribute(RootNode, "todir");
			if (deployDir == null)
			{
				AddAttributeNotFoundError("todir");
				return false;
			}
            return true;
        }

        public override void Execute()
        {
            deployDir = PropertyEvaluator.EvalValue(deployDir);
            if (deployDir == null)
            {
                environment.AddToErrorList("Error evaluating attributes. Execution suspended.");
                return;
            }
            
            // Logger.info("Deploying to directory: {0}", deployDir);
            if (!Directory.Exists(deployDir))
            {
                // Logger.info(2, "Creating new directory: {0}", deployDir);
                Directory.CreateDirectory(deployDir);
            }

			List<Dictionary<string, string>> copied = new List<Dictionary<string, string>>();

			IEnumerable<Dictionary<string, string>> input = environment.Pipe.FilterStandardPipe("include", "exclude");
			foreach (Dictionary<string, string> data in input)
            {
				if (!data.ContainsKey("filename") || !File.Exists(data["filename"])) 
				{
					string name = data.ContainsKey("filename") ? data["filename"] : "";
					environment.AddToErrorList("Filename does not exist: {0}", name);
					continue;
				}

				string filename = data["filename"];

				// Is flatten?
				bool flatten = data.ContainsKey("flatten") && data["flatten"].Equals("");
				string destDir = !flatten && data.ContainsKey("relativePath") ? data["relativePath"] : ".";
				destDir = FileUtil.FixDirectorySeparator(destDir);

				// Change relative dir?
				string cd = data.ContainsKey("changeRelativeDir") ? data["changeRelativeDir"] : "";
				cd = FileUtil.FixDirectorySeparator(cd);
				if (destDir.Equals(cd))
					destDir = "";
				else
				{
					if (destDir.StartsWith(cd + Path.DirectorySeparatorChar))
					{
						destDir = destDir.Substring(cd.Length + 1);
					}
				}

                // Logger.info(2, "Deploying file {0}", filename);

				string fName = Path.GetFileName(filename);
                string fullDestDir = Path.Combine(deployDir, destDir);
				string destFile = Path.Combine(fullDestDir, fName);

				data.Add("copied-to", destDir);
				copied.Add(data);

				if (!Directory.Exists(fullDestDir))
					Directory.CreateDirectory(fullDestDir);
                File.Copy(filename, destFile, true);
            }

			// Execute children tasks
			environment.BeginContext(new Pipe(copied));
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
			environment.EndContext();
        }

    }
}
