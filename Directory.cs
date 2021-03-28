using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Gestion_de_fichier_Form
{
    public class Directory
    {
        public static Directory ROOT = Db.getDirById(1);
        public readonly int ID;
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentID { get; set; }

        /// <summary>
        /// Construir un dossier volatile
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="parentId"></param>
        /// <param name="insert"></param>
        public Directory(int ID, string name, string path, int parentId, bool insert = true)
        {
            this.ID = ID;
            this.Name = name;
            this.Path = path;
            this.ParentID = parentId;
            if (insert)
                this.ID = Db.InsertDir(this);
        }
        /// <summary>
        /// Recuperer un dossier de base donne
        /// </summary>
        /// <param name="id"></param>

        public List<Directory> ListDirectory()
        {
            List<Directory> listD = new List<Directory>();
            listD = Db.SelectForDirectory(this, Db._DirTable, listD);
            return listD;
        }
        public List<File> ListFile()
        {
            List<File> ListF = new List<File>();
            ListF = Db.SelectForDirectory(this, Db._FileTable, ListF);
            return ListF;
        }
        public void Transform(int transferId, string Name, bool isFile)
        {
            string table = (isFile) ? Db._FileTable : Db._DirTable;
            Db.Update(table, this.ID, this.Path, Name, transferId);


        }
        public long getSize()
        {
            long size = 0;
            //somme de taille des fichier
            var listF = ListFile();
            if (listF != null)
            {
                for (int i = 0; i < listF.Count; i++)
                {
                    size += listF[i].size;
                }
            }
            //somme de taille des dossier
            var listD = ListDirectory();
            if (listD != null)
            {
                for (int i = 0; i < listD.Count; i++)
                {
                    size += listD[i].getSize();
                }
            }
            return size;
        }
        public bool remove()
        {
            var listD = ListDirectory();
            var listF = ListFile();
            bool removed = true;
            // verifier si le dossier est vide
            if (listD == null && listF == null)
            {
                //Perm.removePerm(ID);
                Db.Delete(ID, Db._DirTable, "ID");
                return removed;
            }
            else
            {
                if(listD != null)
                {
                    foreach (var Dir in listD)
                    {
                        if (!Dir.remove())
                        {
                            removed = false;
                        }
                    }
                }
                if(listF != null)
                {
                    foreach (var file in listF)
                    {
                        if (!file.remove())
                        {
                            removed = false;
                        }
                    }
                }
                if(removed)
                    Db.Delete(ID, Db._DirTable, "ID");

            }
            return removed;
        }
        public File GetFileChild(int id)
        {
            var listF = ListFile();
            if (listF != null)
            {
                for (int i = 0; i < listF.Count; i++)
                {
                    if (listF[i].ID == id)
                        return listF[i];
                }
                return null;
            }
            return null;
        }
        public File GetFileChild(string name)
        {
            var listF = ListFile();
            if (listF != null)
            {
                for (int i = 0; i < listF.Count; i++)
                {
                    if (listF[i].name == name)
                        return listF[i];
                }

            }
            return null;

        }
        public int IdByPath(string path, bool file = false)
        {
            Directory nextD;
            Directory root = (path[0] == '/') ? Directory.ROOT : this;
            path = (path[0] == '/') ? path.Substring(1) : path;
            if (path == "")
            {
                return 1;
            }
            string[] pathList = path.Split('/');
            int listLength = (file) ? pathList.Length - 1 : pathList.Length;
            for (int i = 0; i < listLength; i++)
            {
                if (pathList[i] != "..")
                {
                    nextD = root.GetDireChild(pathList[i]);
                    if (nextD == null)
                        return -1;
                    else
                        root = nextD;
                }
                else
                {
                    root = Db.getDirById(root.ParentID);
                }
            }
            return root.ID;
        }
        public string GetDirectoryPath(int id)
        {
            string path;
            if (id != 1)
            {
                Directory dire = Db.getDirById(id);
                path = GetDirectoryPath(dire.ParentID) + "/" + dire.Name;
            }
            else
            {
                path = "Root";
            }
            return path;
        }
        public Directory GetDireChild(int id)
        {
            var listD = ListDirectory();
            if (listD != null)
            {
                for (int i = 0; i < listD.Count; i++)
                {
                    if (listD[i].ID == id)
                        return listD[i];
                }
            }
            return null;
        }
        public Directory GetDireChild(string name)
        {
            var listD = ListDirectory();
            if (listD != null)
            {
                for (int i = 0; i < listD.Count; i++)
                {
                    if (listD[i].Name == name)
                        return listD[i];
                }
            }
            return null;
        }
        public Directory creatChildDirectory(string name, string path)
        {
            return Db.InsertDir(name, ID, path);
        }
        public bool ChildNameExist(string name, bool isFile = false, int dirId = 0,string exe = "")
        {
            return Db.nameExisted(dirId, ID, name, isFile,exe);

        }
        public bool Empty()
        {
            if (ListDirectory() != null || ListFile() != null)
                return false;
            else
                return true;
        }
        public List<int> allDirDecendent()
        {
            List<int> listid = new List<int>();

            recerDirDecendent(listid);
            return listid;
        }
        private void recerDirDecendent(List<int> listID)
        {
            var listD = ListDirectory();
            listID.Add(ID);
            if(listD != null)
            {
                foreach (var dir in listD)
                {
                    dir.recerDirDecendent(listID);
                }
            }

        }



    }
}
