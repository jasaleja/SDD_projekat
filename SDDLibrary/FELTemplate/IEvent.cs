using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDDLibrary.FEL_template
{
    public interface IEvent:IComparable<IEvent>
    {
        double Time { get; }
    }
}
