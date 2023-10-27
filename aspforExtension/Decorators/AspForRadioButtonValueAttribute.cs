using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension.Decorators;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class AspForRadioButtonValueAttribute : Attribute
{
    private string _value;

    public AspForRadioButtonValueAttribute(string value)
    {
        this._value = value;
    }

    public string Value
    {
        get
        {
            return _value;
        }
    }
}
