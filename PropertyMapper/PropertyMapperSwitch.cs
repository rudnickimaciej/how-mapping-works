using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public class PropertyMapperSwitch
    {
        public static string Map<T>(object value) 
        {

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return new IntPropertyMapper().Map(value);

                case TypeCode.String:
                    return new StringPropertyMapper().Map(value);

                case TypeCode.DateTime:
                    return new DateTimePropertyMapper().Map(value);

            }
            throw new Exception("Nie zdefiniowano mappera dla tego typu.");
        }
    }
}
