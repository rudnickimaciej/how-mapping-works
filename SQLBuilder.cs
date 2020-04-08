using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Refleksja
{
    public class SQLBuilder
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
        bool tableExists(string tableName, SqlConnection connection)
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

            Dictionary<Type, SqlDbType> dict = new Dictionary<Type, SqlDbType>
            {
                { typeof(Int32), SqlDbType.Int},
                { typeof(string), SqlDbType.Text},
                {typeof(DateTime),SqlDbType.DateTime }

            };

        public void RecursiveCreateTable<T>()
        {

            using (SqlConnection con = new SqlConnection(_connString))
            {
                con.Open();
                if (tableExists(getTypeName(typeof(T).ToString()), con)) return;

                try
                {

                    PropertyInfo[] properties = GetProperties<T>();
                    string query = "create table " + getTypeName(typeof(T).ToString()) + "(";
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].GetType().IsPrimitive)
                        {

                        }
                        query += properties[i].Name + " " + dict[properties[i].PropertyType];
                        if (properties[i].Name == "Id" || properties[i].Name == "id") query += " primary key identity not null ";
                        query += ", ";
                    }
                    query += ")";

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);

                }

                con.Close();
            }

        }

        public void RecursiveMap()
        {

        }
        public void GenerateQuery()
        {

        }
    }
}
