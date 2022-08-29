using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace PTEI
{
    public class PTEISettings : ModSettings
    {
        public static List<FloatMenuOption> TraitOptionsMale;
        public static List<FloatMenuOption> TraitOptionsFemale;
        public static string TraitSettingMale = "-";
        public static string TraitSettingFemale = "-";
        public static int TraitDegreeMale = 0;
        public static int TraitDegreeFemale = 0;
        public static int TraitChanceMale = 100;
        public static int TraitChanceFemale = 100;
        public static bool TraitOverride = false;
        public static bool DebugLogging = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TraitSettingMale, "TraitSettingMale");
            Scribe_Values.Look(ref TraitSettingFemale, "TraitSettingFemale");
            Scribe_Values.Look(ref TraitDegreeMale, "TraitDegreeMale");
            Scribe_Values.Look(ref TraitDegreeFemale, "TraitDegreeFemale");
            Scribe_Values.Look(ref TraitChanceMale, "TraitChanceMale", 100, true);
            Scribe_Values.Look(ref TraitChanceFemale, "TraitChanceFemale", 100, true);
            Scribe_Values.Look(ref TraitOverride, "TraitOverride");
            Scribe_Values.Look(ref DebugLogging, "DebugLogging");

            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard options = new Listing_Standard();

            options.Begin(inRect);

            options.Label("setting_pte_mchance_label".TranslateSimple());
            TraitChanceMale = (int)options.Slider(TraitChanceMale, 0f, 100f);
            options.Label("setting_pte_fchance_label".TranslateSimple());
            TraitChanceFemale = (int)options.Slider(TraitChanceFemale, 0f, 100f);
            options.CheckboxLabeled("setting_pte_override_label".TranslateSimple(), ref TraitOverride);
            options.CheckboxLabeled("setting_pte_debug_label".TranslateSimple(), ref DebugLogging);

            TraitOptionsMale = new List<FloatMenuOption>() { new FloatMenuOption("-", () => { TraitSettingMale = "-"; TraitDegreeMale = 0; }) };
            TraitOptionsFemale = new List<FloatMenuOption>() { new FloatMenuOption("-", () => { TraitSettingFemale = "-"; TraitDegreeFemale = 0; }) };

            foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading)
            {
                foreach (TraitDegreeData degree in trait.degreeDatas)
                {
                    TraitOptionsMale.Add(new FloatMenuOption(degree.GetLabelFor(Gender.Male), () => { TraitSettingMale = trait.defName; TraitDegreeMale = degree.degree; }));
                    TraitOptionsFemale.Add(new FloatMenuOption(degree.GetLabelFor(Gender.Female), () => { TraitSettingFemale = trait.defName; TraitDegreeFemale = degree.degree; }));
                }
            }

            if(options.ButtonTextLabeled("setting_pte_mtrait_label".TranslateSimple(), TraitSettingMale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsMale));
            }

            if(options.ButtonTextLabeled("setting_pte_ftrait_label".TranslateSimple(), TraitSettingFemale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsFemale));
            }

            options.End();
        }
    }
}