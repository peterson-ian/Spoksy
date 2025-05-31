using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spoksy.Domain.ValueObjects
{
    public enum ChatStatus
    {
        Active = 1,
        Reported = 2,
        Inactive = 3,
        Closed = 4,
        Archived = 5
    }
}
