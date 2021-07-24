using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.AI;

namespace PreceptTraitEnforcer
{
    [StaticConstructorOnStartup]
    static class PTEHarmonyPatches
    {
        internal static FieldInfo _pawn;

        static PTEHarmonyPatches()
        {
            var harmony = new Harmony("PreceptTraitEnforcer");
            MethodInfo rwmethod = AccessTools.Method(typeof(Pawn_IdeoTracker), "SetIdeo", new[] { typeof(Ideo) });
            MethodInfo pmethod = typeof(PTEHarmonyPatches).GetMethod("SetIdeoPatch");

            if(rwmethod != null && pmethod != null)
            {
                var hmethod = new HarmonyMethod(pmethod);
                harmony.Patch(rwmethod, null, hmethod);
                Log.Message("PreceptTraitEnforcer: Patched SetIdeo()");
            }
        }

        public static void SetIdeoPatch(Pawn_IdeoTracker __instance, ref Ideo ideo)
        {
            if(Faction.OfPlayerSilentFail == null || !Faction.OfPlayer.ideos.IsPrimary(ideo))
            {
                return;
            }

            Pawn pawn = __instance.GetPawn();

            if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Asexual))
            {
                TraitResetLoveInterest(pawn, TraitDefOf.Asexual);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Bisexual))
            {
                TraitResetLoveInterest(pawn, TraitDefOf.Bisexual);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Gay))
            {
                TraitResetLoveInterest(pawn, TraitDefOf.Gay);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Straight))
            {
                TraitResetLoveInterest(pawn, null);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Cannibal))
            {
                TraitResetWithConflicts(pawn, TraitDefOf.Cannibal);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Nudist))
            {
                TraitResetWithConflicts(pawn, TraitDefOf.Nudist);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Pyromaniac))
            {
                TraitResetWithConflicts(pawn, TraitDefOf.Pyromaniac);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Undergrounder))
            {
                TraitResetWithConflicts(pawn, TraitDefOf.Undergrounder);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_ChemicalFascination))
            {
                TraitResetDegree(pawn, TraitDefOf.DrugDesire, 2);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_ChemicalInterest))
            {
                TraitResetDegree(pawn, TraitDefOf.DrugDesire, 1);
            }
            else if(ideo.HasPrecept(PTEPreceptDefOf.TraitEnforcer_Teetotaler))
            {
                TraitResetDegree(pawn, TraitDefOf.DrugDesire, -1);
            }

        }

        private static Pawn GetPawn(this Pawn_IdeoTracker _this)
        {
            var flag = _pawn == null;
            if(!flag)
            {
                return (Pawn)_pawn.GetValue(_this);
            }

            _pawn = typeof(Pawn_IdeoTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            var flag2 = _pawn == null;
            if(flag2)
            {
                Log.ErrorOnce("Unable to reflect Pawn_IdeoTracker.pawn", 1874595483);
            }

            return (Pawn)_pawn?.GetValue(_this);
        }

        private static void TraitResetLoveInterest(Pawn pawn, TraitDef trait)
        {
            // Disallow overriding asexual
            if (pawn.story.traits.HasTrait(TraitDefOf.Asexual) || (trait != null && pawn.story.traits.HasTrait(trait)))
            {
                return;
            }

            foreach(TraitDef _trait in new[] { TraitDefOf.Bisexual, TraitDefOf.Gay })
            {
                for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
                {
                    if (pawn.story.traits.allTraits[i].def == _trait)
                    {
                        pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
                        break;
                    }
                }
            }

            if(trait != null) {
                pawn.story.traits.GainTrait(new Trait(trait));
            }

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
            }*/
        }

        private static void TraitResetWithConflicts(Pawn pawn, TraitDef trait)
        {
            if (pawn.story.traits.HasTrait(trait))
            {
                return;
            }

            pawn.story.traits.GainTrait(new Trait(trait));
        }

        private static void TraitResetDegree(Pawn pawn, TraitDef trait, int degree)
        {
            if (pawn.story.traits.HasTrait(TraitDefOf.DrugDesire))
            {
                for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
                {
                    if (pawn.story.traits.allTraits[i].def == TraitDefOf.DrugDesire)
                    {
                        pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
                        break;
                    }
                }
            }

            pawn.story.traits.GainTrait(new Trait(trait, degree));
        }
    }

    [DefOf]
    public static class PTEPreceptDefOf
    {
        public static PreceptDef TraitEnforcer_Asexual;
        public static PreceptDef TraitEnforcer_Bisexual;
        public static PreceptDef TraitEnforcer_Cannibal;
        public static PreceptDef TraitEnforcer_ChemicalFascination;
        public static PreceptDef TraitEnforcer_ChemicalInterest;
        public static PreceptDef TraitEnforcer_Gay;
        public static PreceptDef TraitEnforcer_Nudist;
        public static PreceptDef TraitEnforcer_Pyromaniac;
        public static PreceptDef TraitEnforcer_Straight;
        public static PreceptDef TraitEnforcer_Teetotaler;
        public static PreceptDef TraitEnforcer_Undergrounder;
    }
}
