﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace PTEI
{
    public class PTEIPreceptDef_Gendered : PreceptDef
    {
        public Gender gender;
    }

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
                Log.Message("PreceptTraitEnforcerImproved: Patched SetIdeo()");
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

                    for (int i = 0; i < def.comps.Count; i++)
                    {
                        if(def.comps[i].GetType() == typeof(PTEIPreceptComp_Standard))
                        {
                            PTEIPreceptComp_Standard pcomp = (PTEIPreceptComp_Standard)def.comps[i];
                            pcomp.ApplyPTEI(pawn);
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

        /*
        private static void TraitResetLoveInterest(Pawn pawn, TraitDef trait)
        {
            /*
            if(trait == null || trait == TraitDefOf.Asexual || trait == TraitDefOf.Gay)
            {
                List<DirectPawnRelation> removeRelations = new List<DirectPawnRelation>();

                foreach(DirectPawnRelation relation in pawn.relations.DirectRelations)
                {
                    if(relation.def == PawnRelationDefOf.Fiance || relation.def == PawnRelationDefOf.Lover || relation.def == PawnRelationDefOf.Spouse)
                    {
                        if(trait == TraitDefOf.Asexual || relation.otherPawn.gender != pawn.gender)
                        {
                            removeRelations.Add(relation);
                        }
                    }
                }

                foreach(DirectPawnRelation relation in removeRelations)
                {
                    pawn.relations.RemoveDirectRelation(relation);
                }
            }
        }*/
    }
}
