using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Gestion_de_fichier_Form
{
    public class Db
    {

        public static string _ConnString = "Data Source=DESKTOP-RF3F46I;Initial Catalog=SystemFichier;Integrated Security=True";
        public static string _FileTable = "[dbo].[File]";
        public static string _DirTable = "[dbo].[Directoty]";
        public static string _FilePermTable = "[dbo].[FilePerm]";
        public static string _DirPermTable = "[dbo].[DirPerm]";
        public static string _UserTable = "[dbo].[User]";



        public static SqlConnection Connection()
        {
            var con = new SqlConnection(_ConnString);
            return con;

        }


        //User Methodes

        public static void FillUser(User us, int id)
        {
            var con = Connection();
            try
            {
                string query = "select * from " + _UserTable + " where userId =" + id;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        us.ID = Convert.ToInt32(reader["userID"]);
                        us.Name = reader["Name"].ToString();
                        us.password = reader["Password"].ToString();
                    }
                    reader.Close();
                }
                con.Close();
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR UserC : " + eror.Message);
            }
            UpdateUserPerm(us);
        }
        public static void UpdateUserPerm(User us)
        {
            var con = Connection();
            us.perms = new List<Perm>();
            try
            {
                con.Open();
                string query = "select * from " + _FilePermTable + " where userId = " + us.ID;
                var cmd2 = new SqlCommand(query, con);
                cmd2.ExecuteNonQuery();
                using (var reader = cmd2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Perm perm = new Perm(Convert.ToInt32(reader["prmID"]), true);
                        us.perms.Add(perm);
                    }
                    reader.Close();
                }
                query = "select * from " + _DirPermTable + " where userId = " + us.ID;
                var cmd3 = new SqlCommand(query, con);
                cmd3.ExecuteNonQuery();
                using (var reader = cmd3.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Perm perm = new Perm(Convert.ToInt32(reader["prmID"]));
                        us.perms.Add(perm);
                    }
                    reader.Close();
                }
                con.Close();

            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR UserC : " + eror.Message);
            }
        }
        public static void SetUserPerm(User us, int id, byte read, byte write, byte exe, string table, string column)
        {
            var con = Connection();
            try
            {
                string query = "Update " + table + " set [read] = " + read + " , write = " + write + " , exe = " + exe + ", update_at = GETDATE() where " + column + " = " + id + " and userID = " + us.ID;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
        }
        public static List<User> ListUser()
        {
            List<User> listUser = new List<User>();
            var con = Connection();
            try
            {
                string query = "Select * from " + _UserTable;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        listUser.Add(new User(Convert.ToInt32(reader["userID"])));
                    }
                }
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            return listUser;
        }


        //FilePerm Methodes

        public static void FillPerm(Perm pr, int prmId, string table, string idTableName)
        {
            try
            {
                var con = Connection();
                string query = "select * from " + table + " where prmId =" + prmId;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pr.prmID = prmId;
                        pr.ID = Convert.ToInt32(reader[idTableName]);
                        pr.read = Convert.ToBoolean(reader["read"]);
                        pr.write = Convert.ToBoolean(reader["write"]);
                        pr.exe = Convert.ToBoolean(reader["exe"]);
                        pr.created_at = Convert.ToDateTime(reader["created_at"]);
                        pr.update_at = (reader.IsDBNull(7)) ? new DateTime() : Convert.ToDateTime(reader["update_at"]);
                        pr.until = (reader.IsDBNull(8)) ? new DateTime() : Convert.ToDateTime(reader["until_at"]);
                        // to check
                    }
                    reader.Close();
                }
                con.Close();
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR filepermC : " + eror.Message);
            }
        }
        public static void AddPerm(int userID, int dirID, byte[] perTable, string table)
        {
            try
            {
                var con = Connection();
                string query = " Insert Into " + table + "(DirID,userID,[read],write,exe,created_at) Values (" + dirID + "," + userID + "," + perTable[0] + "," + perTable[1] + "," + perTable[2] + ",GETDATE())";
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
        }
        public static File getFileById(int id)
        {
            File file;

            try
            {
                string Name = "", type = "";
                string Path = "", exe = "";
                int ParentID = 0;
                long size = 0;

                var con = Connection();
                string query = "select * from " + _FileTable + " where id =" + id;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Name = reader["Name"].ToString();
                        Path = reader["Path"].ToString();
                        ParentID = Convert.ToInt32(reader["parentID"]);
                        size = Convert.ToInt64(reader["Size"]);
                        //data = (byte[])reader["Data"];
                        type = reader["Type"].ToString();
                        exe = reader["Exe"].ToString();

                    }
                    else
                    {
                        return null;
                    }
                    reader.Close();
                }
                con.Close();
                file = new File(id, Name, Path, ParentID, type, exe, size, false);
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR directoryC : " + eror.Message);
                return null;
            }
            return file;
        }



        /// <summary>
        /// "DELETE FROM <paramref name="table"/> where <paramref name="columnName"/> = <paramref name="id"/>"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        public static void Delete(int id, string table, string columnName)
        {
            try
            {
                var con = Connection();
                string query = "Delete from " + table + " where " + columnName + " = " + id;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
        }



        //Directory Methodes

        /// <summary>
        ///   "SELECT FROM [dbo].[Directory] WHERE ID  = <paramref name="dir"/>.ID"
        /// </summary>
        /// <param name="dir"></param>
        public static void FillDirectory(Directory dir)
        {
            try
            {
                var con = Connection();
                string query = "select * from " + _DirTable + " where id =" + dir.ID;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dir.Name = reader["Name"].ToString();
                        dir.Path = reader["Path"].ToString();
                        dir.ParentID = Convert.ToInt32(reader["parentID"]);
                    }
                    reader.Close();
                }
                con.Close();
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR directoryC : " + eror.Message);
            }

        }
        //to use
        public static Directory getDirById(int id)
        {
            Directory dir;

            try
            {
                string Name = "";
                string Path = "";
                int ParentID = 0;

                var con = Connection();
                string query = "select * from " + _DirTable + " where id =" + id;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Name = reader["Name"].ToString();
                        Path = reader["Path"].ToString();
                        ParentID = Convert.ToInt32(reader["parentID"]);
                    }
                    else
                    {
                        return null;
                    }
                    reader.Close();
                }
                con.Close();
                dir = new Directory(id, Name, Path, ParentID, false);
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR directoryC : " + eror.Message);
                return null;
            }
            return dir;
        }
        public static List<Directory> SelectForDirectory(Directory dir, string table, List<Directory> list)
        {

            var connection = Connection();
            bool vide = true;
            try
            {
                string query = "SELECT * FROM " + table + " where parentID = '" + dir.ID + "' and ID <> 1";
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();
                int id;
                while (reader.Read())
                {
                    vide = false;
                    id = Convert.ToInt32(reader["ID"]);
                    list.Add(getDirById(id));

                }
                if (vide)
                {
                    list = null;
                }
                reader.Close();
                connection.Close();
            }
            catch (SqlException eror)
            {

                Console.WriteLine(eror.Message);
            }
            return list;
        }
        public static List<Directory> ListAllDir()
        {
            var connection = Connection();
            List<Directory> list = new List<Directory>();
            bool vide = true;
            try
            {
                string query = "SELECT * FROM " + _DirTable;
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();
                int id;
                while (reader.Read())
                {
                    vide = false;
                    id = Convert.ToInt32(reader["ID"]);
                    list.Add(getDirById(id));

                }
                if (vide)
                {
                    list = null;
                }
                reader.Close();
                connection.Close();
            }
            catch (SqlException eror)
            {

                Console.WriteLine(eror.Message);
            }
            return list;
        }
        public static List<File> SelectForDirectory(Directory dir, string table, List<File> list)
        {

            var connection = Connection();
            bool vide = true;
            try
            {
                string query = "SELECT * FROM " + table + " where parentID = '" + dir.ID + "'";
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                var reader = cmd.ExecuteReader();
                int id;
                while (reader.Read())
                {
                    vide = false;
                    id = Convert.ToInt32(reader["ID"]);
                    list.Add(getFileById(id));

                }
                if (vide)
                {
                    list = null;
                }
                reader.Close();
                connection.Close();
            }
            catch (SqlException eror)
            {

                Console.WriteLine(eror.Message);
            }
            return list;
        }



        // File or Directory Methodes
        public static void fillFile(File file)
        {
            var con = Connection();
            try
            {
                string query = "select * from [dbo].[file] where id =" + file.ID;
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        file.name = reader["Name"].ToString();
                        file.Path = reader["Path"].ToString();
                        file.parentID = Convert.ToInt32(reader["parentID"]);
                        file.size = Convert.ToInt64(reader["Size"]);
                        //file.data = (byte[])reader["Data"];
                        file.type = reader["Type"].ToString();
                        file.exe = reader["Exe"].ToString();
                    }
                    reader.Close();
                }
                con.Close();
            }
            catch (Exception eror)
            {
                Console.WriteLine("EROR fileC : " + eror.Message);
            }
        }
        public static void Update(string table, int parentID, string Path, string Name, int Id)
        {
            try
            {
                

                if (!(table == _FileTable))
                {
                    Path = (Path == "/") ? Path : Path + "/";
                    Path += Name;
                }
                var connection = Db.Connection();
                string query = "UPDATE " + table + " Set ParentID = " + parentID + ", Path = '" + Path  + "', Name ='" + Name + "'  WHERE ID = '" + Id + "'";
                var cmd = new SqlCommand(query, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
        }
        public static Directory InsertDir(string name, int parentId, string path)
        {
            try
            {
                var connection = Connection();
                string query = "insert into " + _DirTable + "(Name,parentID,Path) values('" + name + "'," + parentId + ",'" + path + "')";
                var cmd2 = new SqlCommand(query, connection);
                connection.Open();
                cmd2.ExecuteNonQuery();
                query = "select * from " + _DirTable + " where Path ='" + path+"'";
                var cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return getDirById(Convert.ToInt32(reader["ID"]));
                    }

                    reader.Close();
                }
                connection.Close();
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            return null;
        }
        public static int InsertDir(Directory dir)
        {
            try
            {
                var connection = Connection();
                string query = "insert into " + _DirTable + "(Name,parentID,Path) values('" + dir.Name + "'," + dir.ParentID + ",'" + dir.Path + "')";
                var cmd2 = new SqlCommand(query, connection);
                connection.Open();
                cmd2.ExecuteNonQuery();
                query = "select * from " + _DirTable + " where Path = '" + dir.Path + "' ";
                var cmd1 = new SqlCommand(query, connection);
                cmd1.ExecuteNonQuery();
                using(var reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader["ID"]);
                    }
                }
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            return -1;
        }
        public static int InsertFile(File file)
        {
            try
            {
                var connection = Db.Connection();
                string query = "insert into " + Db._FileTable + "(Name,parentID,Path,Size,Type,Exe) values('" + file.name + "'," + file.parentID + ",'" + file.Path + "'," +
                                            "" + file.size + ",'" + file.type + "','" + file.exe + "')";
                var cmd2 = new SqlCommand(query, connection);
                connection.Open();
                cmd2.ExecuteNonQuery();

                query = "select * from " + _FileTable + " where Path ='" + file.Path + "'";
                var cmd = new SqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader["ID"]);
                    }

                    reader.Close();
                }

            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            return -1;
        }

        public static bool nameExisted(int dirID, int parentID, string name, bool isFile = false,string exe = "")
        {
            var con = Connection();
            bool existed = false;
            try
            {
                if (!isFile)
                {
                    string query = "select * from " + _DirTable + " where ParentID = " + parentID + " and Name = '" + name + "' and ID <> " + dirID;
                    var cmd1 = new SqlCommand(query, con);
                    con.Open();
                    cmd1.ExecuteNonQuery();

                    using (var reader = cmd1.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existed = true;
                        }
                        reader.Close();
                    }
                }
                else
                {
                    string query = "select * from " + _FileTable + " where ParentID = " + parentID + "and Name = '" + name + "'and Exe = '"+exe+"'  and ID <> " + dirID;
                    var cmd1 = new SqlCommand(query, con);
                    con.Open();
                    cmd1.ExecuteNonQuery();

                    using (var reader = cmd1.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existed = true;
                        }
                        reader.Close();
                    }
                }


            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            con.Close();

            return existed;
        }

        public static int? Login(string name , string password)
        {
            var conn = Db.Connection();
            int? id = null;
            conn.Open();
            string query;
            try
            {
                query = "Select * from [dbo].[User] where Name = '" + name + "' and Password = '" + password + "'";
                var cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        id =  Convert.ToInt32(reader["userId"]);

                    }
                    reader.Close();
                }

            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }
            conn.Close();
            return id;
        }


    }
}
