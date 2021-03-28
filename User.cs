using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;

namespace Gestion_de_fichier_Form
{
    public class User
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string password { get; set; }
        public List<Perm> perms = new List<Perm>();


        public User(int id)
        {
            Db.FillUser(this, id);
        }

        public void updateListPerm()
        {
            Db.UpdateUserPerm(this);
        }
        public bool? checkRead(int id, bool isFile = false)
        {
            for (int i = 0; i < perms.Count; i++)
            {
                if (id == perms[i].ID && perms[i].isFile == isFile)
                {
                    return perms[i].read;
                }
            }
            return null;
        }
        public bool? checkWrite(int id, bool isFile = false)
        {
            for (int i = 0; i < perms.Count; i++)
            {
                if (id == perms[i].ID && perms[i].isFile == isFile)
                {
                    return perms[i].write;
                }
            }
            return null;
        }
        public bool? checkExe(int id, bool isFile = false)
        {
            for (int i = 0; i < perms.Count; i++)
            {
                if (id == perms[i].ID && perms[i].isFile == isFile)
                {
                    return perms[i].exe;
                }
            }
            return null;
        }

        public void setPerm(int id, byte read, byte write, byte exe, bool isFile = false)
        {
            string table = (isFile) ? "[dbo].[FilePerm]" : "[dbo].[DirPerm]";
            string column = (isFile) ? "fileID" : "dirId";
            Db.SetUserPerm(this, id, read, write, exe, table, column);
            updateListPerm();

        }

        public static User userIdByName(string name)
        {
            List<User> userByName = (from user in Db.ListUser()
                                     where user.Name.ToLower() == name.ToLower()
                                     select user).ToList();
            return userByName[0];
        }
        public static List<int> usersIdList()
        {
            return (from users in Db.ListUser()
                    select users.ID).ToList();
        }
        /*public static int addUser()
        {
            Console.WriteLine(" --- Application de gestion des fichiers --- ");
            Console.Write("\nName (Guest):");
            string name = Console.ReadLine();
            Console.Write("Password :");
            string password = "";
            Console.WriteLine();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    password = (password != "") ? password.Substring(0, password.Length - 1) : "";
                }
                else
                {
                    password += key.KeyChar;
                }
            }
            var con = Db.Connection();
            try
            {
                string query = "Select * from [dbo].[User] Where Name =" + name;
                var cmd2 = new SqlCommand(query, con);
                con.Open();
                cmd2.ExecuteNonQuery();
                using (var reader = cmd2.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine(" User Name Already Exist");
                        return 0;
                    }
                    reader.Close();
                }

                con.Close();
            }
            catch (SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }


            try
            {
                
                string query = "Insert Into [dbo].[User] (Name,Password) values ('" + name + "','" + password + "');" +
                               "Insert into [dbo].[Directory] (Name,Path,ParentID) values ('" +name+ "','/user/" + name + "',3)";
                var cmd1 = new SqlCommand(query, con);
                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();

            }catch(SqlException eror)
            {
                Console.WriteLine(eror.Message);
            }


        }


        private void newUserPerm(int id)
        {
            var con = Db.Connection();

            try
            {
                string query = "Select * from [dbo].[Directory]";
                var cmd = new SqlCommand(query, con);
                con.Open();
                cmd.ExecuteNonQuery();

                using(var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                    }

                    reader.Close();
                }
            }
        }*/


    }

}
