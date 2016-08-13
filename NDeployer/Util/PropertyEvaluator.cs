using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NDeployer.Util
{

    class PropertyEvaluatorException : Exception
    {
        public PropertyEvaluatorException(string text) : base(text)
        {}
    }

    static class PropertyEvaluator
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

        private static bool CheckPendingProperties(PropertyItem item)
        {
            int firstIndex = item.EvalValue.IndexOf("${");
            int lastIndex = item.EvalValue.IndexOf("}");
            if (firstIndex != -1 || lastIndex != -1)
            {
                firstIndex = firstIndex == -1 ? 0 : firstIndex;
                lastIndex = lastIndex == -1 ? item.EvalValue.Count() : lastIndex;
                int length = lastIndex - firstIndex + 1;
                Logger.warning("Property {0} could not be completely evaluated. There could be some missing values at '{1}'", item.Name, item.EvalValue.Substring(firstIndex, length));
				return false;
            }
			return true;
        }

        public static PropertyItem EvalPropertyItem(PropertyItem item, Stack<string> evalStack)
        {
            Environment environment = Environment.GetEnvironment();

            item.EvalValue = item.Value;
            List<string> references = GetPropertyReferences(item.Value);

            // References found, need to eval
			if (references.Count > 0)
			{

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
			}

            // Check pending properties
			if (!CheckPendingProperties(item))
				return null;

			// Fix directory separator
			item.EvalValue = FileUtil.FixDirectorySeparator(item.EvalValue);

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
				bool result = EvalProperty(key);
				if (!result)
					return false;
            }
            return true;
        }

		public static bool EvalProperty(string key)
		{
			Environment environment = Environment.GetEnvironment();

			try
			{
				PropertyItem item = EvalProperty(key, new Stack<string>());
				if (item == null)
					return false;
				// Logger.info("Property {0} = {1}", item.Name, item.EvalValue);
			}
			catch (PropertyEvaluatorException e)
			{
				environment.AddToErrorList(e.Message);
				return false;
			}

			return true;
		}

        public static string EvalValue(string value)
        {
            Environment environment = Environment.GetEnvironment();

            try
            {
                PropertyItem item = EvalPropertyItem(new PropertyItem { Name = "", Value = value, EvalValue = null }, new Stack<string>());
                // Logger.info("Value {0}", item.EvalValue);
				if (item == null)
					return null;
                return item.EvalValue;
            }
            catch (PropertyEvaluatorException e)
            {
                environment.AddToErrorList(e.Message);
            }
            return null;
        }

    }
}
