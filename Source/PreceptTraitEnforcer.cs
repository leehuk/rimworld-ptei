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
                if((ideo.HasMeme(MemeDefOf.MaleSupremacy) && pawn.gender != Gender.Male) || (ideo.HasMeme(MemeDefOf.FemaleSupremacy) && pawn.gender != Gender.Female))
                {
                    return;
                }

                TraitResetLoveInterest(pawn, TraitDefOf.Gay);
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
            if (pawn.story.traits.HasTrait(trait) || pawn.story.traits.HasTrait(TraitDefOf.Asexual))
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

            pawn.story.traits.GainTrait(new Trait(trait));

            if(trait == TraitDefOf.Asexual || trait == TraitDefOf.Gay)
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
        }
    }

    [DefOf]
    public static class PTEPreceptDefOf
    {
        public static PreceptDef TraitEnforcer_Asexual;
        public static PreceptDef TraitEnforcer_Bisexual;
        public static PreceptDef TraitEnforcer_Gay;
    }
}
