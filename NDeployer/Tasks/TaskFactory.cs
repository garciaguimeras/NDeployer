using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer.Tasks
{
    class TaskFactory
    {

        public static Task CreateTaskForTag(string tag)
        {
            switch (tag)
            {
				case "meta-attr":
					return new MetaAttributeTask(tag);

                case "property":
                    return new PropertyTask(tag);

                case "file":
                    return new FileTask(tag);

				case "new-file":
					return new NewFileTask(tag);

				case "read-line":
					return new ReadLineTask(tag);

				case "copy":
					return new CopyTask(tag);

				case "flatten":
					return new FlattenTask(tag);

				case "change-relative-dir":
					return new ChangeRelativeDirTask(tag);

				case "filter":
					return new FilterTask(tag);

				case "push-pipe":
					return new PushPipeTask(tag);

				case "pop-pipe":
					return new PopPipeTask(tag);

				case "clear-pipe":
					return new ClearPipeTask(tag);

				case "with":
					return new WithTask(tag);

				case "unzip":
					return new UnzipTask(tag);

				case "print":
					return new PrintTask(tag);
            }

            return null;
        }

    }
}
