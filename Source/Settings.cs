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
        public static bool DebugLogging = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TraitSettingMale, "TraitSettingMale");
            Scribe_Values.Look(ref TraitSettingFemale, "TraitSettingFemale");
            Scribe_Values.Look(ref TraitDegreeMale, "TraitDegreeMale");
            Scribe_Values.Look(ref TraitDegreeFemale, "TraitDegreeFemale");
            Scribe_Values.Look(ref DebugLogging, "DebugLogging");
            base.ExposeData();
        }

        public class PTEIMod : Mod
        {
            PTEISettings settings;

            public PTEIMod(ModContentPack content) : base(content)
            {
                this.settings = GetSettings<PTEISettings>();
            }

            public override void DoSettingsWindowContents(Rect inRect)
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

                var styleLabelMale = new Rect(0f, 0f, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
                var styleFieldMale = new Rect(styleLabelMale.width + 5f, 0f, inRect.width - styleLabelMale.width - 5f, Text.LineHeight);

                var styleLabelFemale = new Rect(0f, Text.LineHeight * 2, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
                var styleFieldFemale = new Rect(styleLabelFemale.width + 5f, Text.LineHeight * 2, inRect.width - styleLabelFemale.width - 5f, Text.LineHeight);

                var styleLabelDebug = new Rect(0f, Text.LineHeight * 7, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
                var styleFieldDebug = new Rect(styleLabelOverride.width + 5f, Text.LineHeight * 7, inRect.width - styleLabelOverride.width - 5f, Text.LineHeight);

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

                Widgets.Label(styleLabelDebug, "setting_pte_debug_label".TranslateSimple());
                Widgets.CheckboxLabeled(styleFieldDebug, "", ref DebugLogging);

                GUI.EndGroup();

                base.DoSettingsWindowContents(inRect);
            }

            public override string SettingsCategory()
            {
                return "PTEI".TranslateSimple();
            }
        }
    }
}