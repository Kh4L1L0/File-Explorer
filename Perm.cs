using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Gestion_de_fichier_Form
{
    public class Perm
    {
        //Deafault permission types
        public static byte[] ownerDirDefaultPerm = { 1, 1, 1 };
        public static byte[] otherDirDefaultPerm = { 1, 0, 1 };
        public static byte[] ownerFileDefaultPerm = { 1, 1, 0 };
        public static byte[] otherFileDefaultPerm = { 1, 0, 0 };


        public int prmID;
        public int ID;
        public bool read;
        public bool write;
        public bool exe;
        public bool isFile;

        public DateTime created_at;
        public DateTime? update_at { get; set; }
        public DateTime? until { get; set; }

        public Perm(int prmId, bool isFile = false)
        {
            var con = Db.Connection();
            string table = (isFile) ? Db._FilePermTable : Db._DirPermTable;
            string idName = (isFile) ? "fileID" : "dirID";
            this.isFile = isFile;
            Db.FillPerm(this, prmId, table, idName);

        }
        public static void addPerm(int currentUserID, int dirID, bool isFile = false)
        {
            List<int> usersID = User.usersIdList();
            string table = (isFile) ? Db._FilePermTable : Db._DirPermTable;
            byte[] perTable = new byte[3];


            for (int i = 0; i < usersID.Count; i++)
            {
                if (currentUserID == usersID[i] || usersID[i] == 1)
                    perTable = (isFile) ? ownerFileDefaultPerm : ownerDirDefaultPerm;
                else
                    perTable = (isFile) ? otherFileDefaultPerm : otherDirDefaultPerm;

                Db.AddPerm(usersID[i], dirID, perTable, table);
            }
        }
        public static void removePerm(int Id, bool isFile = false)
        {
            string table = (isFile) ? Db._FilePermTable : Db._DirPermTable;
            string nameId = (isFile) ? "fileID" : "dirID";
            Db.Delete(Id, table, nameId);


        }

    }
}
