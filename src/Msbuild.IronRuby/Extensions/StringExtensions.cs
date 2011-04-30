using System.IO;

using IronRuby.Runtime;

namespace MsBuild.IronRuby.Extensions
{
    public static class StringExtensions
    {
        public static string Mangle(this string unmangledString)
        {
            return RubyUtils.TryMangleName(unmangledString);
        }

        public static string Unmangle(this string mangledString)
        {
            return RubyUtils.TryUnmangleName(mangledString);
        }

        public static string ToFullPath(this string relativePath)
        {
            return Path.GetFullPath(relativePath);
        }

        public static string CombinePath(this string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }
    }
}