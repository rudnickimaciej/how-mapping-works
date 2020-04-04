using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;

namespace Refleksja
{
   public class DatabaseAbstraction
    {

        private const string _connString = "Data Source=DESKTOP-H5FE18K;Initial Catalog=Test;Integrated Security=True";
        public T CreateInstance<T>(params object[] paramArray)
        {
            return (T)Activator.CreateInstance(typeof(T), args: paramArray);
        }

        public PropertyInfo[] GetProperties<T>()
        {
            return typeof(T).GetProperties();
        }

        string getTypeName(string name)
        {
            int index = name.IndexOf('.');
            return name.Substring(++index);
        }
        public void CreateTable<T>()
        {

            Dictionary<Type, string> dict = new Dictionary<Type, string>
            {
                { typeof(Int32),"int" },
                { typeof(string),"varchar(20)"}

            };
        
           using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();

                try
                {
                    PropertyInfo[] properties = GetProperties<Person>();
                    string query = "create table " + getTypeName(typeof(Person).ToString()) + "(";
                    for(int i = 0; i < properties.Length; i++)
                    {
                        query += properties[i].Name + " " + dict[properties[i].PropertyType];
                        if (properties[i].Name == "Id" || properties[i].Name== "id") query += " primary key ";
                        query += ", ";
                    }
                    query += ")";
                     
                    using (SqlCommand command = new SqlCommand(query,con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Error: " + e.Message);

                }
            }
        }
        public  Person GetPerson(int id)
        {
            using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                      "SELECT * FROM  Person2 where id='"+id +"'", con))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                PropertyInfo[] properties = GetProperties<Person>();

                                object[] array = new object[properties.Length];

                                for  (int i=0;i<array.Length;i++)
                                {
                                    array[i] = reader.GetValue(i);
                                };
                                return (Person)Activator.CreateInstance(typeof(Person),array);
                            }
                            return null;
                        }
                        else
                        {
                            throw new Exception("Error: There is no such record in table.");
                        }

                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }
            }
        }
    }
}
