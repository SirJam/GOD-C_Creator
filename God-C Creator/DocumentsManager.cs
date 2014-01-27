using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace God_C_Creator
{
    class RecentDocument
    {
        private string _name;
        private string _path;
        public string Name
        {
            get
            {   
                return this._name; 
            }
            set 
            { 
                this._name = value; 
            }
        }
        public string Path
        {
            get
            {   
                return this._path; 
            }
            set 
            { 
                this._path = value; 
            }
        }
    }
    class DocumentsManager
    {
        private ObservableCollection<RecentDocument> _recDocuments = new ObservableCollection<RecentDocument>();
        private string AppFolder = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        public ObservableCollection<RecentDocument> GetDocuments()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(AppFolder);
            FileInfo[] filesInfo = dirInfo.GetFiles("*.godc", SearchOption.AllDirectories);
            this._recDocuments.Clear();

            for (int i = 0; i < filesInfo.Length; i++)
            {
                RecentDocument recDoc = new RecentDocument();
                recDoc.Name = filesInfo[i].Name;
                recDoc.Path = filesInfo[i].FullName;

                this._recDocuments.Add(recDoc);
            }

            return this._recDocuments;
        }
    }
}
