using Refleksja.Models;
using System;
using System.Net.Mail;
using System.Reflection;


namespace Refleksja
{
    class Program
    {
        static void Main(string[] args)
        {

            OrderItem p = new OrderItem();
            Type t = p.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
                Console.WriteLine("Properties: {0}, {1}", prop.Name, prop.PropertyType );



            Mapper mapper = new Mapper();
            mapper.DeleteTable<OrderItem>();
            mapper.CreateTable<OrderItem>();
            mapper.Save(new OrderItem() { OrderCode=220,OrderDate=DateTime.Now,OrderStatus="e3434" });



          
            
        }
    }
}
