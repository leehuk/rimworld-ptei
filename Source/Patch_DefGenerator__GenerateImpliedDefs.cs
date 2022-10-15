using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using static PTEI.PTEISettings;

namespace PTEI
{
    [HarmonyPatch(typeof(DefGenerator))]
    [HarmonyPatch("GenerateImpliedDefs_PreResolve")]
    public static class DefGenerator__GenerateImpliedDefs_PreResolve
    {
        public static void Prefix()
        {
            if (TraitsEnabled == null)
            {
                TraitsEnabled = new HashSet<string>();
            }

            InitTraitsEnabled();
        }

        private static void InitTraitsEnabled()
        {
            foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading)
            {
                foreach (TraitDegreeData degree in trait.degreeDatas)
                {
                    string refname = trait.defName + degree.degree.ToString();

                    if (TraitsEnabled.Contains(refname))
                    {
                        PTEIDebug.DebugLog("InitTraitsEnabled(): Found enabled trait: " + refname);
                        CreatePreceptDef(trait, degree, Gender.Male);
                        CreatePreceptDef(trait, degree, Gender.Female);
                    }
                }
            }

            //DefDatabase<PreceptDef>.ClearCachedData();
        }

        private static void CreatePreceptDef(TraitDef trait, TraitDegreeData degree, Gender gender)
        {
            PTEIDebug.DebugLog("CreatePreceptDef(): CreatePreceptDef");

            string refname = trait.defName + degree.degree.ToString();
            string genderstr = gender == Gender.Male ? "Male" : "Female";

            var precept = new PTEIPreceptDef_Gendered
            {
                defName = "PTEID_" + genderstr + "_" + refname,
                preceptClass = typeof(Precept),

                issue = DefDatabase<IssueDef>.GetNamed("TraitEnforcer" + genderstr),
                label = degree.GetLabelCapFor(gender),
                description = trait.LabelCap,
                impact = PreceptImpact.Medium,
                displayOrderInIssue = 1000,
                displayOrderInImpact = 5000,
                gender = gender,
                visible = true,

                comps = new List<PreceptComp>()
            };

            precept.comps.Add(new PTEIPreceptComp_Standard
            {
                trait = trait,
                degree = degree.degree,
            });

            DefDatabase<PreceptDef>.Add(precept);
        }
    }
}