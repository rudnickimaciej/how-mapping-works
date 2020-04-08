using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja
{
    public class Person
    {
        public Person()
        {

        }
        public Person(int id,string name,int age)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }

    }
}
