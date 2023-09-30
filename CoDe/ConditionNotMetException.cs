using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    class ConditionNotMetException : Exception
    {
        public ConditionNotMetException(string message) : base(message)
        {
        }
    }
}
