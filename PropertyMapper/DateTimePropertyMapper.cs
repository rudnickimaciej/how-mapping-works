using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public class DateTimePropertyMapper : IPropertyMapper
    {
        public string Map(object property)
        {

            return "'" + ((DateTime)property).ToString("yyyy-MM-dd") + "'";
            
        }
    }
}
