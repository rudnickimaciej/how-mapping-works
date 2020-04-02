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
            PropertyInfo[] fi = t.GetProperties();
            foreach (PropertyInfo field in fi)
                Console.WriteLine("Field: {0}", field.Name);


            DatabaseAbstraction db = new DatabaseAbstraction();
            db.CreateTable<Person>();
            Person p2 = db.GetPerson(1);
        }
    }
}
