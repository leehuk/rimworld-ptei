using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace PTEI
{
    [StaticConstructorOnStartup]
    static class PreceptTraitEnforcerImproved
    {
        internal static FieldInfo _pawn;

        static PreceptTraitEnforcerImproved()
        {
            var harmony = new Harmony("PreceptTraitEnforcerImproved");
            MethodInfo rwmethod = AccessTools.Method(typeof(Pawn_IdeoTracker), "SetIdeo", new[] { typeof(Ideo) });
            MethodInfo pmethod = typeof(PreceptTraitEnforcerImproved).GetMethod("SetIdeoPatch");

            if (rwmethod != null && pmethod != null)
            {
                var hmethod = new HarmonyMethod(pmethod);
                harmony.Patch(rwmethod, null, hmethod);
                Log.Message("[PTEI]: Patched SetIdeo()");
            }
        }

        public static void SetIdeoPatch(Pawn_IdeoTracker __instance, ref Ideo ideo)
        {
            if (Faction.OfPlayerSilentFail == null || !Faction.OfPlayer.ideos.IsPrimary(ideo))
            {
                return;
            }

            Pawn pawn = __instance.GetPawn();

            if(pawn == null)
            {
                return;
            }

            foreach (Precept p in ideo.PreceptsListForReading)
            {
                if(p.def.GetType() == typeof(PTEIPreceptDef_Gendered))
                {
                    PTEIPreceptDef_Gendered def = (PTEIPreceptDef_Gendered)p.def;

                    if(pawn.gender != def.gender)
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
                        pcomp.conflictingTraits = pcomp.trait.conflictingTraits;

                        pcomp.ApplyPTEI(pawn);
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
                        pcomp.conflictingTraits = pcomp.trait.conflictingTraits;

                        pcomp.ApplyPTEI(pawn);
                    }
                    else
                    {
                        for (int i = 0; i < def.comps.Count; i++)
                        {
                            if (def.comps[i].GetType() == typeof(PTEIPreceptComp_Standard))
                            {
                                PTEIPreceptComp_Standard pcomp = (PTEIPreceptComp_Standard)def.comps[i];
                                pcomp.ApplyPTEI(pawn);
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
