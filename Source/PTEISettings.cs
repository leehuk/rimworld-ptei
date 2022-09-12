using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using static PTEI.PTEISettingsStatic;

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
        public static bool TraitFactions = false;
        public static bool DebugLogging = false;

        public static HashSet<string> TraitsEnabled;

        private static Vector2 addTraitsScroller;
        private static int addTraitsLines;

        private static readonly List<string> xmlTraits = new List<string>{
            "Asexual0", "Bisexual0", "Bloodlust0", "Brawler0", "BodyPurist0", "Cannibal0", "DrugDesire-1", "DrugDesire1", "DrugDesire2",
            "Gay0", "Gourmand0", "Jealous0", "Masochist0", "Nudist0", "Psychopath0", "Pyromaniac0", "Transhumanist0", "Undergrounder0"
        };

        public override void ExposeData()
        {
            Scribe_Values.Look(ref TraitSettingMale, "TraitSettingMale");
            Scribe_Values.Look(ref TraitSettingFemale, "TraitSettingFemale");
            Scribe_Values.Look(ref TraitDegreeMale, "TraitDegreeMale");
            Scribe_Values.Look(ref TraitDegreeFemale, "TraitDegreeFemale");
            Scribe_Values.Look(ref TraitChanceMale, "TraitChanceMale", 100, true);
            Scribe_Values.Look(ref TraitChanceFemale, "TraitChanceFemale", 100, true);
            Scribe_Values.Look(ref TraitOverride, "TraitOverride");
            Scribe_Values.Look(ref TraitFactions, "TraitFactions");
            Scribe_Values.Look(ref DebugLogging, "DebugLogging");

            Scribe_Collections.Look(ref TraitsEnabled, "keys", LookMode.Value);

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
            options.CheckboxLabeled("setting_pte_factions_label".TranslateSimple(), ref TraitFactions);
            options.CheckboxLabeled("setting_pte_debug_label".TranslateSimple(), ref DebugLogging);
            options.Gap(Text.LineHeight);

            /*
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

            if (options.ButtonTextLabeled("setting_pte_mtrait_label".TranslateSimple(), TraitSettingMale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsMale));
            }

            if (options.ButtonTextLabeled("setting_pte_ftrait_label".TranslateSimple(), TraitSettingFemale))
            {
                Find.WindowStack.Add(new FloatMenu(TraitOptionsFemale));
            }*/

            options.Label("Additional Traits (Restart Required)");
            options.Gap(Text.LineHeight / 2);

            options.End();

            Rect scrollRect = inRect;
            scrollRect.y += options.CurHeight;
            scrollRect.yMax -= options.CurHeight + Text.LineHeight;

            Rect listRect = new Rect(0f, 0f, inRect.width - 30f, (addTraitsLines + 2) * (Text.LineHeight + options.verticalSpacing));

            Widgets.BeginScrollView(scrollRect, ref addTraitsScroller, listRect);
            options.Begin(listRect);
            this.DoListTraits(options);
            options.End();
            Widgets.EndScrollView();
        }

        private void DoListTraits(Listing_Standard options)
        {
            addTraitsLines = 0;

            SortedList<string, TraitWithDegree> traitlist = new SortedList<string, TraitWithDegree>();

            // we need to order by the degree name, which is inside our trait iterable
            foreach (TraitDef trait in DefDatabase<TraitDef>.AllDefsListForReading)
            {
                foreach (TraitDegreeData degree in trait.degreeDatas)
                {
                    traitlist.Add(degree.GetLabelCapFor(Gender.None), new TraitWithDegree(trait, degree));
                }
            }

            foreach (KeyValuePair<string, TraitWithDegree> kvp in traitlist)
            {
                TraitDef trait = kvp.Value.Trait;
                TraitDegreeData degree = kvp.Value.Degree;

                string refname = trait.defName + degree.degree.ToString();
                bool checkOn = TraitsEnabled?.Contains(refname) ?? false;

                if (xmlTraits.Contains(refname))
                {
                    continue;
                }

                Rect rect = options.GetRect(Text.LineHeight);

                string label = degree.GetLabelCapFor(Gender.None);
                if (!string.IsNullOrEmpty(trait.modContentPack?.Name))
                {
                    label += " (" + trait.modContentPack.Name + ")";
                }

                Widgets.CheckboxLabeled(rect, label, ref checkOn);
                Widgets.DrawHighlightIfMouseover(rect);
                if (addTraitsLines % 2 != 0)
                {
                    Widgets.DrawLightHighlight(rect);
                }

                if (checkOn && !(TraitsEnabled.Contains(refname)))
                {
                    TraitsEnabled.Add(refname);
                }
                else if (!checkOn && TraitsEnabled.Contains(refname))
                {
                    TraitsEnabled.Remove(refname);
                }

                addTraitsLines++;
            }
        }
    }

    internal class TraitWithDegree
    {
        public TraitDef Trait;
        public TraitDegreeData Degree;

        public TraitWithDegree(TraitDef trait, TraitDegreeData degree)
        {
            this.Trait = trait;
            this.Degree = degree;
        }
    }

}