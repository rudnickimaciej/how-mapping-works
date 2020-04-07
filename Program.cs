using System;
using System.Net.Mail;
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



            Mapper mapper = new Mapper();
            mapper.DeleteTable<Person>();
            mapper.CreateTable<Person>();
            mapper.Save(new Person() { Age = 23, Name = "Maciek" });
            mapper.Save(new Person() { Age = 24, Name = "Piotrek" });

            Person p2 = mapper.Get<Person>(1);


          
            
        }
    }
}
