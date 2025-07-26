using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class TraceActivityAttribute : Attribute
    {
        public string? Name { get; }
        public bool RecordExceptions { get; }

        public TraceActivityAttribute(string? name = null, bool recordExceptions = true)
        {
            Name = name;
            RecordExceptions = recordExceptions;
        }
    }
}
