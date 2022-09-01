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
        public TraitDef trait;
        public int degree = 0;
        public List<TraitDef> removeTraits = new List<TraitDef>();

        public PTEIPreceptComp_Standard() { }

        public void Apply(Pawn pawn)
        {
            PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply()");

            if(this.trait == null)
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply(): Trait is null");
                return;
            }

            if((this.degree == 0 && pawn.story.traits.HasTrait(this.trait)) || (this.degree != 0 && pawn.story.traits.HasTrait(this.trait, this.degree)))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply(): Pawn -" + pawn.Name + "- has trait: " + this.trait.defName);
                return;
            }

            if(this.Conflicts(pawn))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply(): Pawn -" + pawn.Name + "- has conflicts");
                return;
            }

            Random rnd = new Random();
            int convres = rnd.Next(101);

            if ((pawn.gender == Gender.Male && PTEISettings.TraitChanceMale < 100 && convres > PTEISettings.TraitChanceMale) || (pawn.gender == Gender.Female && PTEISettings.TraitChanceFemale < 100 && convres > PTEISettings.TraitChanceFemale))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply(): Pawn -" + pawn.Name + "- chance failed: " + convres.ToString());
                return;
            }

            this.RemoveTraits(pawn);

            PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Apply(): Pawn -" + pawn.Name + "- adding trait: " + this.trait.defName);
            if (this.degree != 0)
            {
                pawn.story.traits.GainTrait(new Trait(this.trait, this.degree));
            }
            else
            {
                pawn.story.traits.GainTrait(new Trait(this.trait));
            }
        }

        private bool Conflicts(Pawn pawn)
        {
            List<TraitDef> conflicts = new List<TraitDef>();

            // A conflicting work type, this is a hard stop
            if (this.trait.requiredWorkTags != WorkTags.None && pawn.WorkTagIsDisabled(this.trait.requiredWorkTags))
            {
                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Pawn -" + pawn.Name + "- worktag conflict: " + this.trait.requiredWorkTags.ToString());
                return true;
            }

            // The same trait but at a different degree
            if (pawn.story.traits.HasTrait(this.trait))
            {
                Trait trait = pawn.story.traits.GetTrait(this.trait);

                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Pawn -" + pawn.Name + "- has trait: " + this.trait.defName + " at degree: " + trait.Degree);
                conflicts.Add(this.trait);
            }

            for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
            {
                Trait trait = pawn.story.traits.allTraits[i];

                if (trait.def.ConflictsWith(this.trait))
                {
                    // Traits marked for removal do not conflict
                    if(this.removeTraits.Contains(trait.def))
                    {
                        continue;
                    }

                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Pawn -" + pawn.Name + "- has conflicting trait: " + this.trait.defName + "//" + trait.def.defName + " at degree: " + trait.Degree);
                    conflicts.Add(trait.def);
                }
            }

            if(conflicts.Count > 0)
            {
                if(PTEISettings.TraitOverride == false)
                {
                    return true;
                }

                PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Applying trait removal");
                // We're overriding traits, so we need to drop any that conflict
                foreach (TraitDef traitdef in conflicts)
                {
                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Pawn -" + pawn.Name + "- removing trait: " + traitdef.defName);

                    Trait trait = pawn.story.traits.GetTrait(traitdef);
                    if (trait == null)
                    {
                        PTEIDebug.DebugLog("PTEIPreceptComp_Standard.Conflicts(): Pawn -" + pawn.Name + "- unable to remove: " + traitdef.defName);
                        continue;
                    }

                    pawn.story.traits.RemoveTrait(trait);
                }
            }

            return false;
        }

        private void RemoveTraits(Pawn pawn)
        {
            foreach (TraitDef traitdef in this.removeTraits)
            {
                Trait trait = pawn.story.traits.GetTrait(traitdef);
                if (trait != null)
                {
                    PTEIDebug.DebugLog("PTEIPreceptComp_Standard.RemoveTraits(): Removing trait: " + traitdef.defName);
                    pawn.story.traits.RemoveTrait(trait);
                }

            }
        }
    }
}
