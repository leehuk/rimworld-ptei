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
            Log.Message("[PTEI]: Initialising Harmony");
            new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();

            this.settings = base.GetSettings<PTEISettings>();
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
