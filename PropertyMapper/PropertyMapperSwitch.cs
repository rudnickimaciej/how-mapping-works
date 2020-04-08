using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public class PropertyMapperSwitch
    {
        public  static IPropertyMapper GetPropertyMapper<T>() 
        {

            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return new IntPropertyMapper();

                case TypeCode.String:
                    return new StringPropertyMapper();

                case TypeCode.DateTime:
                    return new DateTimePropertyMapper();

            }
            throw new Exception("Nie zdefiniowano mappera dla tego typu.");
        }
    }
}
