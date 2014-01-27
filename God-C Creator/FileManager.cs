using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace God_C_Creator
{
    class FileManager
    {
        public string AppFolder = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        public FileManager()
        {

        }

        public void DeleteFileAtPath(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException ex)
            {
            }
        }
        public void DeleteFolderAtPath(string path)
        {
            try
            {
                var dir = new DirectoryInfo(@path);
                dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                dir.Delete(true);
            }
            catch (IOException ex)
            {
            }
        }
        public void MoveFilesToHisDirectory(string filename)
        {
            try
            {
                System.IO.File.Move(AppFolder + "\\" + filename + ".godc", AppFolder + "\\" + filename + "\\" + filename + ".godc");
                System.IO.File.Move(AppFolder + "\\" + filename + ".asm", AppFolder + "\\" + filename + "\\" + filename + ".asm");
                System.IO.File.Move(AppFolder + "\\" + filename + ".exe", AppFolder + "\\" + filename + "\\" + filename + ".exe");
                System.IO.File.Move(AppFolder + "\\" + filename + ".obj", AppFolder + "\\" + filename + "\\" + filename + ".obj");
            }
            catch
            {

            }
        }
        public void CreateFolderForCurrentProjectAtPath(string path)
        {
            DeleteFileAtPath(path);
            Directory.CreateDirectory(path);
        }
    }
}
