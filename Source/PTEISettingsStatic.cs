using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using RimWorld;
using Verse;
using static PTEI.PTEISettings;

namespace PTEI
{
    [StaticConstructorOnStartup]
    public static class PTEISettingsStatic
    {
        static PTEISettingsStatic()
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

                    if(TraitsEnabled.Contains(refname))
                    {
                        PTEIDebug.DebugLog("PTEISettingsStatic.InitTraitsEnabled(): Found enabled trait: " + refname);
                        CreatePreceptDef(trait, degree, Gender.Male);
                        CreatePreceptDef(trait, degree, Gender.Female);
                    }
                }
            }

            //DefDatabase<PreceptDef>.ClearCachedData();
        }

        private static void CreatePreceptDef(TraitDef trait, TraitDegreeData degree, Gender gender)
        {
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

            MethodInfo shmethod = typeof(ShortHashGiver).GetMethod("GiveShortHash", BindingFlags.NonPublic|BindingFlags.Static);
            if (shmethod != null)
            {
                shmethod.Invoke(null, new object[] { precept, precept.GetType() });
                DefDatabase<PreceptDef>.Add(precept);
            }
            else
            {
                Log.Error("CreatePreceptDef(): Unable to reflect GiveShortHash()");
            }


        }
    }
}
