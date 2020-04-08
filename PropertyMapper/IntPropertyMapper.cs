using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public class IntPropertyMapper : IPropertyMapper
    {

        public string Map(object property)
        {
            return property.ToString();
        }
    }
}
