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
                Log.Message("[PTEI] Applying trait: " + this.trait.defName);

                if (this.degree != 0)
                {
                    pawn.story.traits.GainTrait(new Trait(this.trait, this.degree));
                }
                else
                {
                    pawn.story.traits.GainTrait(new Trait(this.trait));
                }
            }
            else
            {
                Log.Message("[PTEI] Applying trait: None");
            }
        }

        private bool Conflicts(Pawn pawn)
        {
            for(int i = 0; i < this.conflictingTraits.Count; i++)
            {
                if(pawn.story.traits.HasTrait(this.conflictingTraits[i]))
                {
                    Log.Message("[PTEI] Trait conflict: " + this.conflictingTraits[i].defName);
                    return true;
                }
            }

            if(this.trait != null && this.trait.requiredWorkTags != WorkTags.None && pawn.WorkTagIsDisabled(this.trait.requiredWorkTags))
            {
                Log.Message("[PTEI] Worktag conflict: " + this.trait.requiredWorkTags.ToString());
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
                    Log.Message("[PTEI] Removing trait: " + pawn.story.traits.allTraits[i].def.defName);
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
            Log.Message("[PTEI] Starting: " + pawn.Name);

            if(this.trait != null && pawn.story.traits.HasTrait(this.trait))
            {
                Log.Message("[PTEI] Skipping trait: has");
                return;
            }

            if (this.Conflicts(pawn))
            {
                Log.Message("[PTEI] Skipping trait: conflict");
                return;
            }

            this.RemoveTraits(pawn);
            this.ApplyTraits(pawn);
        }

        public TraitDef trait;
        public int degree = 0;
        public List<TraitDef> conflictingTraits = new List<TraitDef>();
        public List<TraitDef> removeTraits = new List<TraitDef>();
    }
}
