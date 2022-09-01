using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTEI
{
    [HarmonyPatch(typeof(Pawn_IdeoTracker))]
    [HarmonyPatch("SetIdeo", new[] { typeof(Ideo) })]
    public static class Pawn_IdeoTracker__SetIdeo
    {
        internal static FieldInfo _pawn;

        public static void Postfix(Pawn_IdeoTracker __instance, ref Ideo ideo)
        {
            if (Faction.OfPlayerSilentFail == null || !Faction.OfPlayer.ideos.IsPrimary(ideo))
            {
                return;
            }

            Pawn pawn = __instance.GetPawn();

            if (pawn == null)
            {
                return;
            }

            foreach (Precept p in ideo.PreceptsListForReading)
            {
                if (p.def.GetType() == typeof(PTEIPreceptDef_Gendered))
                {
                    PTEIPreceptDef_Gendered def = (PTEIPreceptDef_Gendered)p.def;

                    if (pawn.gender != def.gender)
                    {
                        continue;
                    }

                    if (def == PTEIPreceptDefOf_Gendered.PTEI_Male_Custom)
                    {
                        PTEIPreceptComp_Standard pcomp = new PTEIPreceptComp_Standard();

                        if (DefDatabase<TraitDef>.AllDefsListForReading.FindIndex(t => t.defName == PTEISettings.TraitSettingMale) == -1)
                        {
                            Log.Message("[PTEI]: Unable to locate male custom trait");
                            continue;
                        }

                        pcomp.trait = DefDatabase<TraitDef>.AllDefsListForReading.Find(t => t.defName == PTEISettings.TraitSettingMale);
                        pcomp.degree = PTEISettings.TraitDegreeMale;

                        pcomp.Apply(pawn);
                    }
                    else if (def == PTEIPreceptDefOf_Gendered.PTEI_Female_Custom)
                    {
                        PTEIPreceptComp_Standard pcomp = new PTEIPreceptComp_Standard();

                        if (DefDatabase<TraitDef>.AllDefsListForReading.FindIndex(t => t.defName == PTEISettings.TraitSettingFemale) == -1)
                        {
                            Log.Message("[PTEI]: Unable to locate female custom trait");
                            continue;
                        }

                        pcomp.trait = DefDatabase<TraitDef>.AllDefsListForReading.Find(t => t.defName == PTEISettings.TraitSettingFemale);
                        pcomp.degree = PTEISettings.TraitDegreeFemale;

                        pcomp.Apply(pawn);
                    }
                    else
                    {
                        for (int i = 0; i < def.comps.Count; i++)
                        {
                            if (def.comps[i].GetType() == typeof(PTEIPreceptComp_Standard))
                            {
                                PTEIPreceptComp_Standard pcomp = (PTEIPreceptComp_Standard)def.comps[i];
                                pcomp.Apply(pawn);
                            }
                        }
                    }
                }
            }
        }

        private static Pawn GetPawn(this Pawn_IdeoTracker _this)
        {
            var flag = _pawn == null;
            if (!flag)
            {
                return (Pawn)_pawn.GetValue(_this);
            }

            _pawn = typeof(Pawn_IdeoTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            var flag2 = _pawn == null;
            if (flag2)
            {
                Log.ErrorOnce("Unable to reflect Pawn_IdeoTracker.pawn", 1874595483);
            }

            return (Pawn)_pawn?.GetValue(_this);
        }
    }
}
