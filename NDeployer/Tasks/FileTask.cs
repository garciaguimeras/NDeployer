using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using NDeployer.Util;

namespace NDeployer.Tasks
{

    class FileTask : GeneratorTask
    {

        string filename;
		XElement root;

		public FileTask(string name) : base(name)
        {
            filename = null;
        }

        public override bool ProcessXml(XElement rootNode)
        {
			root = rootNode;
            filename = GetAttribute(rootNode, "name");
			if (filename == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
            return true;
        }

		public void AddToStandardPipe(string filename, string relativePath)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("filename", filename);
			data.Add("relativePath", relativePath);
			environment.Pipe.AddToStandardPipe(data);
		}

        private void ReadDirectory(string path, string relativePath)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
                // Add filename + relativePath
				AddToStandardPipe(f, relativePath);

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string dName = Path.GetFileName(dir);
                string dRelativePath = relativePath + Path.DirectorySeparatorChar + dName;
                ReadDirectory(dir, dRelativePath);
            }
        }

        public override void ExecuteGenerator()
        {
			// Evaluate filename property
			filename = PropertyEvaluator.EvalValue(filename);
			if (filename == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			if (!File.Exists(filename) && !Directory.Exists(filename))
			{
				// Add an error :(
				environment.Pipe.AddToErrorPipe("File or directory does not exist: {0}", filename);
				return;
			}

			// Add unique file...
            if (File.Exists(filename))
				AddToStandardPipe(filename, ".");
    
			// ...or add a whole directory
            if (Directory.Exists(filename))
                ReadDirectory(filename, ".");

			// Execute tasks in context
			ExecuteContext(root.Elements());
        }

    }
}
