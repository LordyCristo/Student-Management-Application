using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Student_Management_Project
{
    class DatabaseAccess
    {
        public static List<Person> LoadDetails()
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                var output = connection.Query<Person>("SELECT * FROM Student ORDER BY ID ASC", new DynamicParameters());
                return output.ToList();
            }
        }

        private static string GetConnectionString(string con = "Default")
        {
            return ConfigurationManager.ConnectionStrings[con].ConnectionString;
        }

        public static bool SavePerson(Person p)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                //return connection.Execute("INSERT INTO Student(ID, Fname, Lname) VALUES(@ID, @Fname, @Lname)", p) > 0;
                try
                {
                    return connection.Execute("INSERT INTO Student(ID, Fname, Lname) VALUES(@ID, @Fname, @Lname)", p) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool UpdatePerson(Person p, String temp)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                try
                {
                    return connection.Execute($"UPDATE Student SET ID=@ID, Fname=@Fname, Lname=@Lname WHERE ID='{temp}'", p) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool DeletePerson(string id)
        {
            using (SQLiteConnection connection = new SQLiteConnection(GetConnectionString()))
            {
                if(connection.Execute($"DELETE FROM Student WHERE ID='{id}'") > 0)
                    return true;
                return false;
            }
        }
    }
}
