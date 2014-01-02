using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib
{
    /// <summary>
    /// AT command response code
    /// </summary>
    public enum ATResponseCode
    {
        AT_OK,
        AT_CONNECT,
        AT_RING,
        AT_NOCARRIER,
        AT_ERROR,
        AT_NODIALTONE,
        AT_BUSY,
        AT_NOANSWER,
        AT_OTHER,            //UNKNOW RESPONSE STRING
        AT_NOTHING           //empty response string
    };

    
}
