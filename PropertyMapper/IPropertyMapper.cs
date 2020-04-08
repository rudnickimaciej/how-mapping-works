using System;
using System.Collections.Generic;
using System.Text;

namespace Refleksja.PropertyMapper
{
    public interface IPropertyMapper
    {   
        string Map(object property);
    }
}
