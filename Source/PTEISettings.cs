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

            float linecounter = 1f;

            var styleLabelMale = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
            var styleFieldMale = new Rect(styleLabelMale.width + 5f, styleLabelMale.y, inRect.width - styleLabelMale.width - 5f, Text.LineHeight);
            linecounter += 1f;

            var styleLabelFemale = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
            var styleFieldFemale = new Rect(styleLabelFemale.width + 5f, styleLabelFemale.y, inRect.width - styleLabelFemale.width - 5f, Text.LineHeight);
            linecounter += 1.5f;

            var styleLabelMaleChance = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.5f), Text.LineHeight);
            var styleFieldMaleChance = new Rect(styleLabelMaleChance.width + 5f, styleLabelMaleChance.y, inRect.width - styleLabelMaleChance.width - 5f, Text.LineHeight);
            linecounter += 1f;

            var styleLabelFemaleChance = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.5f), Text.LineHeight);
            var styleFieldFemaleChance = new Rect(styleLabelFemaleChance.width + 5f, styleLabelFemaleChance.y, inRect.width - styleLabelFemaleChance.width - 5f, Text.LineHeight);
            linecounter += 1.5f;

            var styleLabelOverride = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
            var styleFieldOverride = new Rect(styleLabelOverride.width + 5f, styleLabelOverride.y, inRect.width - styleLabelOverride.width - 5f, Text.LineHeight);
            linecounter += 1f;
            var styleDescOverride = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight * 2);
            linecounter += 2.5f;

            var styleLabelDebug = new Rect(0f, Text.LineHeight * linecounter, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
            var styleFieldDebug = new Rect(styleLabelOverride.width + 5f, styleLabelDebug.y, inRect.width - styleLabelOverride.width - 5f, Text.LineHeight);
            linecounter += 1.5f;

            GUI.BeginGroup(inRect);

            Widgets.Label(styleLabelMale, "setting_pte_mtrait_label".TranslateSimple());
            if (Widgets.ButtonText(styleFieldMale, TraitSettingMale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsMale));
            }

            Widgets.Label(styleLabelFemale, "setting_pte_ftrait_label".TranslateSimple());
            if (Widgets.ButtonText(styleFieldFemale, TraitSettingFemale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsFemale));
            }

            Widgets.Label(styleLabelMaleChance, "setting_pte_mchance_label".TranslateSimple());
            TraitChanceMale = (int)Widgets.HorizontalSlider(styleFieldMaleChance, TraitChanceMale, 0f, 100f);

            Widgets.Label(styleLabelFemaleChance, "setting_pte_fchance_label".TranslateSimple());
            TraitChanceFemale = (int)Widgets.HorizontalSlider(styleFieldFemaleChance, TraitChanceFemale, 0f, 100f);

            Widgets.Label(styleLabelOverride, "setting_pte_override_label".TranslateSimple());
            Widgets.CheckboxLabeled(styleFieldOverride, "", ref TraitOverride);
            Widgets.Label(styleDescOverride, "setting_pte_override_desc".TranslateSimple());

            Widgets.Label(styleLabelDebug, "setting_pte_debug_label".TranslateSimple());
            Widgets.CheckboxLabeled(styleFieldDebug, "", ref DebugLogging);

            GUI.EndGroup();
        }
    }
}