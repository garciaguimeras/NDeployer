using NDeployer.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer
{

    class PropertyItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string EvalValue { get; set; }
    }

    class Environment
    {

        private static Environment instance = null;

        Dictionary<string, Task> tasks;
        Dictionary<string, PropertyItem> properties;

        public Dictionary<string, Task> Tasks { get { return tasks; } }
        public Dictionary<string, PropertyItem> Properties { get { return properties;  } }

        public static Environment GetEnvironment()
        {
            if (instance == null)
                instance = new Environment();
            return instance;
        }

        private Environment()
        {
            tasks = new Dictionary<string, Task>();
            properties = new Dictionary<string, PropertyItem>();

            InitTasks();
        }

        private void InitTasks()
        {
            AddTask(new DeployTask());
            AddTask(new FileTask());
        }

        public void AddTask(Task task)
        {
            tasks.Add(task.Name, task);
        }

        public Task GetTask(string name)
        {
            try
            {
                return tasks[name];
            }
            catch (KeyNotFoundException e)
            { }
            return null;
        }

        public void AddProperty(string name, string value)
        {
            properties.Add(name, new PropertyItem { Name = name, Value = value, EvalValue = null });
        }

        public PropertyItem GetProperty(string name)
        {
            try
            {
                return properties[name];
            }
            catch (KeyNotFoundException e)
            { }
            return null;
        }
    }
}
