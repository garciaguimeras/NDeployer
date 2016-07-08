using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer
{

    class PropertyEvaluatorException : Exception
    {
        public PropertyEvaluatorException(string text) : base(text)
        {}
    }

    class PropertyEvaluator
    {

        private static List<string> GetPropertyReferences(string text)
        {
            Environment environment = Environment.GetEnvironment();
            List<string> references = new List<string>();

            foreach (string key in environment.Properties.Keys)
            {
                string pRef = "${" + environment.Properties[key].Name + "}";
                if (text.Contains(pRef))
                    references.Add(key);
            }

            return references;
        }

        private static void CheckPendingProperties(PropertyItem item)
        {
            int firstIndex = item.EvalValue.IndexOf("${");
            int lastIndex = item.EvalValue.IndexOf("}");
            if (firstIndex != -1 || lastIndex != -1)
            {
                firstIndex = firstIndex == -1 ? 0 : firstIndex;
                lastIndex = lastIndex == -1 ? item.EvalValue.Count() : lastIndex;
                int length = lastIndex - firstIndex + 1;
                Logger.warning("Property {0} could not be completely evaluated. There could be some missing values at '{1}'", item.Name, item.EvalValue.Substring(firstIndex, length));
            }
        }

        public static PropertyItem EvalPropertyItem(PropertyItem item, Stack<string> evalStack)
        {
            Environment environment = Environment.GetEnvironment();

            item.EvalValue = item.Value;
            List<string> references = GetPropertyReferences(item.Value);

            // No references, no need to eval
            if (references.Count == 0)
            {
                // Check pending properties
                CheckPendingProperties(item);
                return item;
            }

            // Ok, let's start the recursive thing...
            foreach (string key in references)
            {
                // Circular reference found!
                if (evalStack.Contains(key))
                    throw new PropertyEvaluatorException(string.Format("Could not evaluate property {0}. Circular reference found between properties {0} and {1}", item.Name, key));

                evalStack.Push(item.Name);
                PropertyItem pItem = EvalProperty(key, evalStack);
                evalStack.Pop();

                item.EvalValue = item.EvalValue.Replace("${" + environment.Properties[key].Name + "}", pItem.EvalValue);
            }

            // Check pending properties
            CheckPendingProperties(item);
            return item;
        }

        public static PropertyItem EvalProperty(string name, Stack<string> evalStack)
        {
            Environment environment = Environment.GetEnvironment();
            PropertyItem item = environment.GetProperty(name);

            // Property not found
            if (item == null)
                return null;

            // Property already evaluated
            if (item.EvalValue != null)
                return item;

            return EvalPropertyItem(item, evalStack);
        }

        public static bool EvalAllProperties()
        {
            Environment environment = Environment.GetEnvironment();

            foreach (string key in environment.Properties.Keys)
            {
                try
                {
                    PropertyItem item = EvalProperty(key, new Stack<string>());
                    if (item == null)
                    {
                        Logger.error("Property {0} not found", item.Name);
                        return false;
                    }
                    Logger.info("Property {0} = {1}", item.Name, item.EvalValue);
                }
                catch (PropertyEvaluatorException e)
                {
                    Logger.error(e.Message);
                    return false;
                }
            }
            return true;
        }

        public static string EvalValue(string value)
        {
            Environment environment = Environment.GetEnvironment();

            try
            {
                PropertyItem item = EvalPropertyItem(new PropertyItem { Name = "", Value = value, EvalValue = null }, new Stack<string>());
                Logger.info("Property {0} = {1}", item.Name, item.EvalValue);
                return item.EvalValue;
            }
            catch (PropertyEvaluatorException e)
            {
                Logger.error(e.Message);
            }
            return null;
        }

    }
}
