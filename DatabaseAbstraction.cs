using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;

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

        bool tableExists(string tableName,SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(@"IF EXISTS(
            SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_NAME = @tableName) 
            SELECT 1 ELSE SELECT 0", connection);

            cmd.Parameters.Add("@tableName", SqlDbType.NVarChar).Value = tableName;

            int exists = (int)cmd.ExecuteScalar();
            if (exists == 1)
                return true;
            return false;
        }
        public void CreateTable<T>()
        {

            Dictionary<Type, SqlDbType> dict = new Dictionary<Type, SqlDbType>
            {
                { typeof(Int32), SqlDbType.Int},
                { typeof(string), SqlDbType.Text}

            };
        
           using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                if (tableExists(getTypeName(typeof(Person).ToString()), con)) return;

                try
                {
                    PropertyInfo[] properties = GetProperties<Person>();
                    string query = "create table " + getTypeName(typeof(Person).ToString()) + "(";
                    for(int i = 0; i < properties.Length; i++)
                    {
                        query += properties[i].Name + " " + dict[properties[i].PropertyType];
                        if (properties[i].Name == "Id" || properties[i].Name== "id") query += " primary key identity not null ";
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

                con.Close();
            }
        }
        public void SavePerson(Person person)
        {
            using(SqlConnection con = new SqlConnection(_connString))
            {
             
                con.Open();
                PropertyInfo[] properties = GetProperties<Person>();
                StringBuilder  queryBuilder = new StringBuilder("insert into  " + getTypeName(typeof(Person).ToString()) + " VALUES(");
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == "Id") continue;

                    string value = properties[i].GetValue(person).ToString();

                    Console.WriteLine(properties[i].GetType().ToString());
                    if (properties[i].PropertyType.IsEquivalentTo(typeof(System.String))){
                        value = "'" + value + "'";
                    }
                    queryBuilder.Append(value+",");
                    
                }
                queryBuilder.Remove(queryBuilder.Length - 1, 1);
                queryBuilder.Append(")");

                Console.WriteLine(queryBuilder.ToString());

                try
                {
                    using (SqlCommand command = new SqlCommand(queryBuilder.ToString(),con))
                    {
                        int reader = command.ExecuteNonQuery();
                    }
                }

                catch (Exception e)
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
                      "SELECT * FROM  Person where id='"+id +"'", con))
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
