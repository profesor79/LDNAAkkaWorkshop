namespace Profesor79.Merge.Domain.Wrappers
{
    using Profesor79.Merge.Contracts;

    /// <summary>The file system wrapper.</summary>
    public class FileSystemWrapper : IFileSystemWrapper
    {
        public bool CheckIfFileExists(string fileName)
        {
            var result = System.IO.File.Exists(fileName);
            return result;
        }

        public string ReadFirstLine(string fileName)
        {
            return string.Empty;
        }

        public string ReadAll(string fileName)
        {
            return string.Empty;
        }

        public bool CreateFile(string fileName) { return false; }
        public bool AppendToFile(string fileName) { return false; }
        public bool WriteFile(string fileName) { return false; }


    }
}
