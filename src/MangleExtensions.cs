using IronRuby.Runtime;

namespace MsBuild.IronRuby
{
    public static class MangleExtensions
    {
        public static string Mangle(this string unmangledString)
        {
            return RubyUtils.TryMangleName(unmangledString);
        }

        public static string Unmangle(this string mangledString)
        {
            return RubyUtils.TryUnmangleName(mangledString);
        }
    }
}