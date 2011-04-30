using System.Xml.Linq;

namespace MsBuild.IronRuby.Extensions
{
    internal class TaskBodyParser
    {
        public static TaskBodyParser Parse(string taskBody)
        {
            return new TaskBodyParser(XElement.Parse(taskBody));
        }

        private readonly XElement _taskElement;

        public TaskBodyParser(XElement taskElement)
        {
            _taskElement = taskElement;
        }

        public string Script
        {
            get { return (string)_taskElement.Attribute("Path"); }
        }
    }
}