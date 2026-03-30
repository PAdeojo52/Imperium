using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium.Engine.Exceptions
{
    public class CalenderException: Exception
    {

        public CalenderException()
        {
            throw new Exception("A calendar-related error occurred.");
        }

        public CalenderException(string message)
        {
            throw new CalenderException(message);
        }

       
    }
}
