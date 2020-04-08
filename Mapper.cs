using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using Refleksja.PropertyMapper;

namespace Refleksja
{
   public class Mapper
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
                { typeof(string), SqlDbType.Text},
                {typeof(DateTime),SqlDbType.DateTime }

            };
        
           using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                if (tableExists(getTypeName(typeof(T).ToString()), con)) return;

                try
                {
                    PropertyInfo[] properties = GetProperties<T>();
                    string query = "create table " + getTypeName(typeof(T).ToString()) + "(";
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

        public void DeleteTable<T>()
        {

            using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                if (tableExists(getTypeName(typeof(T).ToString()), con))
                {
                    {
                        SqlCommand command = new SqlCommand("DROP TABLE " + getTypeName(typeof(T).ToString()), con);
                        command.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
        }

 
        public void Save<T>(T obj)
        {
            using(SqlConnection con = new SqlConnection(_connString))
            {
             
                con.Open();
                PropertyInfo[] properties = GetProperties<T>();
                StringBuilder  queryBuilder = new StringBuilder("insert into  " + getTypeName(typeof(T).ToString()) + " VALUES(");

                for (int i = 0; i < properties.Length; i++)
                { 
                    if (properties[i].Name == "Id") continue;

                    Type propType = properties[i].PropertyType;
                    object propValue = properties[i].GetValue(obj);

                    string value = (string)typeof(PropertyMapperSwitch)
                        .GetMethod("Map")
                        .MakeGenericMethod(propType)
                        .Invoke(null, new[] { propValue });
                 
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
                        con.Close();

                    }
                }

                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }

            }
        }
        

        public  T Get<T>(int id)
        {
            using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                      "SELECT * FROM " + getTypeName(typeof(T).ToString()) +" where id='"+id +"'", con))
                    {
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                PropertyInfo[] properties = GetProperties<T>();

                                object[] array = new object[properties.Length];

                                for  (int i=0;i<array.Length;i++)
                                {
                                    array[i] = reader.GetValue(i);
                                };
                                return (T)Activator.CreateInstance(typeof(T),array);
                            }
                            return default(T);
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
