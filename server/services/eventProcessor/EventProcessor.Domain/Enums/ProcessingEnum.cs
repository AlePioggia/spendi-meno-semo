using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Domain.Enums
{
    public enum ProcessingStateEnum
    {
        Unprocessed = 0,
        Received = 1,
        Processing = 2,
        Completed = 3,
        Error = 4
    }
}
