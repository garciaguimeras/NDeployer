using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NDeployer.Tasks
{

    class DeployTask : Task
    {

        string deployDir;

        public DeployTask(Environment environment) : base(environment)
        {
            Name = "deploy";
            deployDir = null;
        }

        public override bool ProcessXml(XElement rootNode)
        {
            deployDir = GetAttribute(rootNode, "todir");
            if (deployDir == null)
                return false;
            return true;
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
			foreach (Dictionary<string, string> data in input)
            {
				if (!data.ContainsKey("filename") || !File.Exists(data["filename"])) 
				{
					string name = data.ContainsKey("filename") ? data["filename"] : "";
					environment.Pipe.AddToErrorPipe("Filename does not exist: {0}", name);
					return;
				}

				string filename = data["filename"];
				string destDir = data.ContainsKey("relativePath") ? data["relativePath"] : ".";

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
