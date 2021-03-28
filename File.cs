using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Gestion_de_fichier_Form
{
    public class File
    {
        public long size;
        public int ID;
        public string name { get; set; }
        public string Path { get; set; }
        public byte[] data;

        public int parentID { get; set; }
        public string type;
        public string exe;

        /// <summary>
        /// construction de fichier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <param name="exe"></param>
        /// <param name="insert"> ! Volatile</param>
        public File(int id, string name, string path, int parentId, string type, string exe, long size, bool insert = true)
        {
            this.ID = id;
            this.name = name;
            this.Path = path;
            this.parentID = parentId;
            this.type = type;
            this.exe = exe;
            this.size = size;
            if (insert)
            {
                this.ID = Db.InsertFile(this);
            }
        }
        /// <summary>
        /// recuperer un fichier de basse de donné
        /// </summary>
        /// <param name="id"></param>
        public File(int id)
        {
            ID = id;
            Db.fillFile(this);
        }

        public bool remove()
        {
            Db.Delete(ID, Db._FileTable, "ID");
            return true;
        }

        //public void creat()
        //public void open()
    }
}
