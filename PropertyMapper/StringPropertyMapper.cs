using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public class StringPropertyMapper : IPropertyMapper
    {
        public string Map(object property)
        {
            return "'" + property.ToString() + "'";
        }
    }
}
