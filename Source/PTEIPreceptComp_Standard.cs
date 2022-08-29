using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RimWorld;
using Verse;

namespace PTEI
{
    public class PTEIPreceptComp_Standard : PreceptComp
    {
        public PTEIPreceptComp_Standard() { }

        private void ApplyTraits(Pawn pawn)
        {
            if (this.trait != null)
            {
                if (this.degree != 0)
                {
                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyTraits(): Applying trait/degree: " + this.trait.defName + "/" + this.degree.ToString());
                    pawn.story.traits.GainTrait(new Trait(this.trait, this.degree));
                }
                else
                {
                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyTraits(): Applying trait: " + this.trait.defName);
                    pawn.story.traits.GainTrait(new Trait(this.trait));
                }
            }
            else
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyTraits(): Applying trait: None");
            }
        }

        private bool Conflicts(Pawn pawn)
        {
            if (PTEISettings.TraitOverride == false)
            {
                for (int i = 0; i < this.conflictingTraits.Count; i++)
                {
                    if (pawn.story.traits.HasTrait(this.conflictingTraits[i]))
                    {
                        PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Trait conflict: " + this.conflictingTraits[i].defName);
                        return true;
                    }
                }
            }

            if(this.trait != null && this.trait.requiredWorkTags != WorkTags.None && pawn.WorkTagIsDisabled(this.trait.requiredWorkTags))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Worktag conflict: " + this.trait.requiredWorkTags.ToString());
                return true;
            }

            return false;
        }

        private static void RemoveTrait(Pawn pawn, TraitDef trait)
        {
            for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
            {
                if (pawn.story.traits.allTraits[i].def == trait)
                {
                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.RemoveTrait(): Removing trait " + pawn.story.traits.allTraits[i].def.defName);
                    pawn.story.traits.RemoveTrait(pawn.story.traits.allTraits[i]);
                    return;
                }
            }
        }
        private void RemoveTraits(Pawn pawn)
        {
            foreach (TraitDef _trait in this.removeTraits)
            {
                RemoveTrait(pawn, _trait);
            }

            // This could happen if someone forces e.g. chemfasc to not conflict with cheminterest
            if(this.trait != null)
            {
                RemoveTrait(pawn, this.trait);

                // Double sanity for anyone deciding to make it ignore conflicts, but doesn't setup removeTraits
                bool reloop = false;
                do
                {
                    reloop = false;

                    for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
                    {
                        if (pawn.story.traits.allTraits[i].def.ConflictsWith(this.trait))
                        {
                            RemoveTrait(pawn, pawn.story.traits.allTraits[i].def);
                            reloop = true;
                            break;
                        }
                    }
                } while (reloop == true);
            }
        }

        public void ApplyPTEI(Pawn pawn)
        {
            if(this.trait != null && PTEISettings.TraitOverride == false && pawn.story.traits.HasTrait(this.trait))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyPTEI(): Pawn -" + pawn.Name + "- already has trait");
                return;
            }

            if (this.Conflicts(pawn))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyPTEI(): Pawn -" + pawn.Name + "- conflicts");
                return;
            }

            Random rnd = new Random();
            int convres = rnd.Next(101);

            if((pawn.gender == Gender.Male && PTEISettings.TraitChanceMale < 100 && convres > PTEISettings.TraitChanceMale) || (pawn.gender == Gender.Female && PTEISettings.TraitChanceFemale < 100 && convres > PTEISettings.TraitChanceFemale))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyPTEI(): Pawn -" + pawn.Name + "- chance failed: " + convres.ToString());
                return;
            }

            PTEIDebug.DebugLog("PTEIPreceptComp_Standard.ApplyPTEI(): Pawn -" + pawn.Name + "- adding trait: " + this.trait.defName);
            this.RemoveTraits(pawn);
            this.ApplyTraits(pawn);
        }

        public TraitDef trait;
        public int degree = 0;
        public List<TraitDef> conflictingTraits = new List<TraitDef>();
        public List<TraitDef> removeTraits = new List<TraitDef>();
    }
}
