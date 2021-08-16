using RimWorld;
using Verse;

namespace PTEI
{
    public class PTEIPreceptDef_Gendered : PreceptDef
    {
        public Gender gender;
    }

    [DefOf]
    public static class PTEIPreceptDefOf_Gendered
    {
        public static PTEIPreceptDef_Gendered PTEI_Male_Custom;
        public static PTEIPreceptDef_Gendered PTEI_Female_Custom;
    }
}
