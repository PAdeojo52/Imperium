using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium.Engine.Exceptions
{
    internal class InvalidMonthException: Exception
    {
        public InvalidMonthException()
        {
            throw new CalenderException("An Month related error has occured.");
        }

        public InvalidMonthException(string message)
        {
            throw new CalenderException("message");
        }
    }
}
