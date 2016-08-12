using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NDeployer.Script;

namespace NDeployer.Tasks
{
    class TaskFactory
    {

        public static Task CreateTask(TaskDef taskDef)
        {
            switch (taskDef.Name)
            {
				case "meta-attr":
					return new MetaAttributeTask(taskDef);

                case "property":
					return new PropertyTask(taskDef);

                case "file":
					return new FileTask(taskDef);

				case "new-file":
					return new NewFileTask(taskDef);

				case "copy":
					return new CopyTask(taskDef);

				case "flatten":
					return new FlattenTask(taskDef);

				case "filter":
					return new FilterTask(taskDef);

				case "with":
					return new WithTask(taskDef);

				case "zip":
					return new ZipTask(taskDef);

				case "unzip":
					return new UnzipTask(taskDef);

				case "print":
					return new PrintTask(taskDef);

				case "read-line":
					return new ReadLineTask(taskDef);

				case "function":
					return new FunctionTask(taskDef);

				case "invoke":
					return new InvokeTask(taskDef);

				case "ifdef":
					return new IfDefTask(taskDef);

				case "ifndef":
					return new IfNotDefTask(taskDef);
            }

            return null;
        }

    }
}
