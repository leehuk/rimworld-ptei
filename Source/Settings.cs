using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace PreceptTraitEnforcer
{
    public class PTESettings : ModSettings
    {
        public enum GenderMode
        {
            All,
            Meme,
            Female,
            Male
        }
        public static string GenderSetting = GenderMode.All.ToString();
        public static List<FloatMenuOption> GenderOptions;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref GenderSetting, "GenderSetting");
            base.ExposeData();
        }

        public class PTEMod : Mod
        {
            PTESettings settings;

            public PTEMod(ModContentPack content) : base(content)
            {
                this.settings = GetSettings<PTESettings>();
            }

            public override void DoSettingsWindowContents(Rect inRect)
            {
                GenderOptions = Enum.GetNames(typeof(GenderMode)).Select(g => new FloatMenuOption($"setting_pte_genderMode_{g}".TranslateSimple(), () => GenderSetting = g)).ToList();

                var styleLabel = new Rect(0f, 0f, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight);
                var styleDesc = new Rect(0f, Text.LineHeight, Mathf.CeilToInt(inRect.width * 0.7f), Text.LineHeight*5);
                var styleField = new Rect(styleLabel.width + 5f, 0f, inRect.width - styleLabel.width - 5f, Text.LineHeight);

                GUI.BeginGroup(inRect);
                Widgets.Label(styleLabel, "setting_pte_genderMode_label".TranslateSimple());

                if (Widgets.ButtonText(styleField, $"setting_pte_genderMode_{GenderSetting}".TranslateSimple()))
                {
                    Find.WindowStack.Add(new FloatMenu(GenderOptions));
                }

                Widgets.Label(styleDesc, "setting_pte_genderMode_desc".TranslateSimple());

                GUI.EndGroup();

                /*
                Listing_Standard listingStandard = new Listing_Standard();
                listingStandard.Begin(inRect);
                listingStandard.End();*/
                base.DoSettingsWindowContents(inRect);
            }
            public override string SettingsCategory()
            {
                return "PreceptTraitEnforcer".TranslateSimple();
            }
        }
    }
}