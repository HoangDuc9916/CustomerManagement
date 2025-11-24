using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.MISAAttribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MISARequired : Attribute
    {
        public string ErrorMessage { get; }
        public MISARequired(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
