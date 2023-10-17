using Evergrowth.AspForMarkDigExtension.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AspForDateTimeTypeAttribute : Attribute
{
    private DateTimeOverride _value = DateTimeOverride.AsDate;

    public AspForDateTimeTypeAttribute(DateTimeOverride Value)
    {
        this._value = Value;
    }

    public DateTimeOverride Value
    {
        get
        {
            return _value;
        }
    }
}
