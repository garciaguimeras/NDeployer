using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using NDeployer.Util;

namespace NDeployer.Tasks
{

    class FileTask : Task
    {

        string filename;

		public FileTask(string name) : base(name)
        {
            filename = null;
        }

        public override bool ProcessXml(XElement rootNode)
        {
            filename = GetAttribute(rootNode, "name");
            if (filename == null)
                return false;
            return true;
        }

		public void AddToOutputPipe(string filename, string relativePath)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("filename", filename);
			data.Add("relativePath", relativePath);
			environment.Pipe.AddToOuputPipe(data);
		}

        private void ReadDirectory(string path, string relativePath)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
                // Add filename + relativePath
				AddToOutputPipe(f, relativePath);

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string dName = Path.GetFileName(dir);
                string dRelativePath = relativePath + Path.DirectorySeparatorChar + dName;
                ReadDirectory(dir, dRelativePath);
            }
        }

        public override void Execute()
        {
			// Evaluate filename property
			filename = PropertyEvaluator.EvalValue(filename);
			if (filename == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			// Keep older files 
			foreach (Dictionary<string, string> data in environment.Pipe.Input) 
			{
				environment.Pipe.AddToOuputPipe(data);
			}

			// Add unique file...
            if (File.Exists(filename))
            {
                // Add filename + relativePath
				AddToOutputPipe(filename, ".");
                return;
            }

			// ...or add a whole directory...
            if (Directory.Exists(filename))
            {
                ReadDirectory(filename, ".");
                return;
            }

			// ...or add an error :(
            environment.Pipe.AddToErrorPipe("File or directory does not exist: {0}", filename);
        }

    }
}
