using System.IO;

namespace GW2_Addon_Manager.Dependencies.FileSystem
{
    class FileSystemManager : IFileSystemManager
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);

        public void DirectoryDelete(string path, bool recursive) => Directory.Delete(path, recursive);
        public void FileDelete(string path) => File.Delete(path);
        public string PathCombine(string path1, string path2) => Path.Combine(path1, path2);
    }
}