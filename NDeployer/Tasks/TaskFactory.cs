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
                case "property":
                    return new PropertyTask(tag);

                case "file":
                    return new FileTask(tag);

                case "deploy":
                    return new DeployTask(tag);

				case "flatten":
					return new FlattenTask(tag);

				case "filter":
					return new FilterTask(tag);
            }

            return null;
        }

    }
}
