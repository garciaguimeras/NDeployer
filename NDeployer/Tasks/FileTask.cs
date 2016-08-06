using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{

    class FileTask : GeneratorTask
    {

        string filename;

		public FileTask(TaskDef rootNode) : base(rootNode)
        {
            filename = null;
        }

		public override bool IsValidTaskDef()
        {
            filename = GetAttribute(RootNode, "name");
			if (filename == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
            return true;
        }

		private void AddToStandardPipe(string filename, string relativePath)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			data.Add("filename", filename);
			data.Add("relativePath", relativePath);
			environment.Pipe.AddToStandardPipe(data);
		}
			
        private void ReadDirectory(string path)
        {
			IEnumerable<string> files = FileUtil.ReadDirectoryRecursively(path);
            foreach (string f in files)
			{
                // Add filename + relativePath
				string relativePath = FileUtil.GetRelativePath(f, path);
				AddToStandardPipe(f, relativePath);
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
                ReadDirectory(filename);

			// Execute tasks in context
			LoadMetaAttributes(RootNode.Children);
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
        }

    }
}
