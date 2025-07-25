using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermesProxy.World.Enums
{
    public enum UpdateTypeLegacy
    {
        Values        = 0,
        Movement      = 1,
        CreateObject1 = 2,
        CreateObject2 = 3,
        FarObjects    = 4,
        NearObjects   = 5
    }

    public enum UpdateTypeModern
    {
        Values            = 0,
        CreateObject1     = 1,
        CreateObject2     = 2,
        OutOfRangeObjects = 3,
    }

    public enum ObjectCreateType
    {
        Create1 = 0,
        Create2 = 1
    }

    [Flags]
    public enum UpdateFieldFlag
    {
        None = 0,
        Owner = 0x01,
        PartyMember = 0x02,
        UnitAll = 0x04,
        Empath = 0x08
    }
}
