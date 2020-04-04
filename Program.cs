using System;
using System.Reflection;


namespace Refleksja
{
    class Program
    {
        static void Main(string[] args)
        {

            Person p = new Person();
            Type t = p.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
                Console.WriteLine("Properties: {0}, {1}", prop.Name, prop.PropertyType );


            DatabaseAbstraction db = new DatabaseAbstraction();
            db.CreateTable<Person>();
            Person p2 = db.GetPerson(1);
        }
    }
}
