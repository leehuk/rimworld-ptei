using System.Collections.Generic;
using Verse;
using static PTEI.PTEISettings;

namespace PTEI
{
    [StaticConstructorOnStartup]
    public static class PTEISettingsStatic
    {
        static PTEISettingsStatic()
        {
            if(TraitsEnabled == null)
            {
                TraitsEnabled = new HashSet<string>();
            }
        }
    }
}
