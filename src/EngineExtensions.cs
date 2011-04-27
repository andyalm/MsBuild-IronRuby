using System.Text;
using Microsoft.Scripting.Hosting;

namespace MsBuild.IronRuby
{
    public static class EngineExtensions
    {
        public static dynamic ExecuteEmbeddedScript(this ScriptScope scope, string resourceName)
        {
            var contentProvider = new EmbeddedResourceContentProvider(resourceName, typeof(EngineExtensions).Assembly);
            var scriptSource = scope.Engine.CreateScriptSource(contentProvider, null, Encoding.ASCII);
            
            return scriptSource.Execute(scope);
        }

        public static dynamic Execute(this ScriptScope scope, string expression)
        {
            return scope.Engine.Execute(expression, scope);
        }

        public static T Execute<T>(this ScriptScope scope, string expression)
        {
            return scope.Engine.Execute<T>(expression, scope);
        }

        public static ObjectOperations CreateOperations(this ScriptScope scope)
        {
            return scope.Engine.CreateOperations(scope);
        }
    }
}