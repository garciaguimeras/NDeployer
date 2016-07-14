﻿using System;
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

                case "copy":
                    return new CopyTask(tag);

				case "flatten":
					return new FlattenTask(tag);

				case "filter":
					return new FilterTask(tag);

				case "push-pipe":
					return new PushPipeTask(tag);

				case "pop-pipe":
					return new PopPipeTask(tag);

				case "clear-pipe":
					return new ClearPipeTask(tag);
            }

            return null;
        }

    }
}
