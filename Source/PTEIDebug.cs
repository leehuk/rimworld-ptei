using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTEI
{
    static class PTEIDebug
    {
        static public void DebugLog(string message)
        {
            if(PTEISettings.DebugLogging == true)
            {
                Log.Message("[PTEI] " + message);
            }
        }
    }
}
