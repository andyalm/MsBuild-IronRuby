using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;

namespace MsBuild.IronRuby.Extensions
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

        public static ScriptScope ExecuteFile(this ScriptScope scope, string filePath)
        {
            return scope.Engine.ExecuteFile(filePath, scope);
        }

        public static ObjectOperations CreateOperations(this ScriptScope scope)
        {
            return scope.Engine.CreateOperations(scope);
        }

        public static void SetProperty(this ScriptScope scope, object instance, string name, object value)
        {
            var operations = scope.CreateOperations();
            var mangledPropertyName = name.Mangle();
            var unmangledPropertyName = name;

            object property = null;
            if (!operations.TryGetMember(instance, mangledPropertyName, out property))
            {
                if (!operations.TryGetMember(instance, unmangledPropertyName.Mangle(), out property))
                {
                    throw new MemberAccessException("The property '" + name + "' is not defined");
                }
                else
                {
                    operations.SetMember(instance, unmangledPropertyName, value);
                }
            }
            else
            {
                operations.SetMember(instance, mangledPropertyName, value);
            }
        }

        public static object GetProperty(this ScriptScope scope, object instance, string name, Type type)
        {
            var operations = scope.CreateOperations();
            var mangledPropertyName = name.Mangle();
            var unmangledPropertyName = name;

            object property = null;
            if (!operations.TryGetMember(instance, mangledPropertyName, out property))
            {
                if (!operations.TryGetMember(instance, unmangledPropertyName.Mangle(), out property))
                {
                    throw new MemberAccessException("The property '" + name + "' is not defined");
                }
                
                return operations.GetMember(instance, unmangledPropertyName, type);
            }

            return operations.GetMember(instance, mangledPropertyName, type);
        }

        public static object GetMember(this ObjectOperations operations, object instance, string name, Type type)
        {
            var rubyTypedValue = operations.GetMember(instance, name);
            if (type.IsArray)
            {
                var untypedArray = (from item in (IEnumerable<dynamic>) rubyTypedValue
                    select operations.ConvertTo(item, type.GetElementType())).ToArray();

                var typedArray = Array.CreateInstance(type.GetElementType(), untypedArray.Length);
                for (var i = 0; i < untypedArray.Length; i++)
                {
                    typedArray.SetValue(untypedArray[i], i);
                }

                return typedArray;
            }
            
            return operations.ConvertTo(rubyTypedValue, type);
        }
    }
}