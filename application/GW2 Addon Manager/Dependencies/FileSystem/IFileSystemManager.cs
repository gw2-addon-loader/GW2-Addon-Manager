namespace GW2_Addon_Manager.Dependencies.FileSystem
{
    public interface IFileSystemManager
    {
        bool DirectoryExists(string path);
        void DirectoryDelete(string path, bool recursive);

        void FileDelete(string path);

        string PathCombine(string path1, string path2);
    }
}