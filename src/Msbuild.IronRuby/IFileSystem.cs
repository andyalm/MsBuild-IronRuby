using System.IO;

namespace MsBuild.IronRuby
{
    public interface IFileSystem
    {
        string GetFileContent(string path);
    }

    public class FileSystem : IFileSystem
    {
        public string GetFileContent(string path)
        {
            return File.ReadAllText(path);
        }
    }
}