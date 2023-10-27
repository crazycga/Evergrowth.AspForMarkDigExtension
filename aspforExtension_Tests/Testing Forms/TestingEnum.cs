using Evergrowth.AspForMarkDigExtension.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evergrowth.AspForMarkDigExtension_Tests.Testing_Forms;
public enum TestingEnum
{
    [AspForRadioButtonValue("Oneth")]
    First,
    [AspForRadioButtonValue("Twoth")]
    Second,
    [AspForRadioButtonValue("Threeth")]
    Third,
    Fourth
}
