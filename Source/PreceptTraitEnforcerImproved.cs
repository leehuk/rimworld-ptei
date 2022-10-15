using HarmonyLib;
using UnityEngine;
using Verse;

namespace PTEI
{
    public class PTEIMod : Mod
    {
        PTEISettings settings;

        public PTEIMod(ModContentPack content) : base(content)
        {
            this.settings = base.GetSettings<PTEISettings>();

            Log.Message("[PTEI]: Initialising Harmony");
            new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "PTEI".TranslateSimple();
        }
    }
}
