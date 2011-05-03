using System;

using Microsoft.Build.Framework;

namespace MsBuild.IronRuby
{
    public interface IParameterTypeDeserializer
    {
        Type DeserializeType(string typeName);
    }

    public class ParameterTypeDeserializer : IParameterTypeDeserializer
    {
        public Type DeserializeType(string typeName)
        {
            if (typeName.EndsWith("[]"))
            {
                var singularType = DeserializeType(typeName.Substring(0, typeName.Length - 2));
                return Type.GetType(singularType.FullName + "[]," + singularType.Assembly.FullName, throwOnError: true);
            }

            switch (typeName.ToLowerInvariant())
            {
                case "itaskitem":
                case "taskitem":
                    return typeof(ITaskItem);
                case "int":
                    return typeof(int);
                case "string":
                    return typeof(String);
                default:
                    var type = Type.GetType(typeName, throwOnError : false);
                    if(type == null)
                        throw new NotSupportedException("The parameter type '" + typeName + "' is not recognized");
                    return type;
            }
        }
    }
}