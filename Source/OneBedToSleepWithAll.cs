using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace OneBedToSleepWithAll
{
    public class OneBedToSleepWithAll : Mod
    {
        public static OneBedToSleepWithAll__settings Settings;

        public OneBedToSleepWithAll(ModContentPack content) : base(content)
        {
            Settings = GetSettings<OneBedToSleepWithAll__settings>();
            new Harmony("densevoid.onebedtosleepwithall").PatchAll(Assembly.GetExecutingAssembly());
        }
        public override string SettingsCategory()
        {
            return "polygamyMode_modName".Translate();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
        }
    }

    public class OneBedToSleepWithAll__settings : ModSettings
    {
        public float requiredSleepHours = 4f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref requiredSleepHours, "requiredSleepHours");
            base.ExposeData();
        }

        internal void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("polygamyMode_requiredHours".Translate() + ": " + requiredSleepHours);
            requiredSleepHours = (float)Math.Round(listingStandard.Slider(requiredSleepHours, 1f, 24f), 1);
            listingStandard.End();
        }

    }
}
