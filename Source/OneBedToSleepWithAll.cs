using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            new Harmony("densevoid.hui.personalworkcat").PatchAll(Assembly.GetExecutingAssembly());
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
        public bool debugMode = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref requiredSleepHours, "requiredSleepHours");
            Scribe_Values.Look(ref debugMode, "debugMode");
            base.ExposeData();
        }

        internal void DoWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("polygamyMode_requiredHours".Translate() + ": " + requiredSleepHours);
            requiredSleepHours = (float)Math.Round(listingStandard.Slider(requiredSleepHours, 1f, 24f), 1);
            if (Prefs.DevMode && listingStandard.ButtonTextLabeled("Debug info", "Get to log"))
            {
                GetDebugInfo();
            };
            //listingStandard.CheckboxLabeled("Debug", ref debugMode);
            listingStandard.End();
        }


        private void GetDebugInfo()
        {
            String res = "One bed to sleep with all :: DEBUG INFO\n!!! CLICK HERE !!!\n";

            List<MethodInfo> patchedMethods = new List<MethodInfo>();

            patchedMethods.Add(AccessTools.Method(typeof(Building_Bed), "GetInspectString"));
            patchedMethods.Add(AccessTools.Method(typeof(CompAssignableToPawn_Bed), "TryAssignPawn"));
            patchedMethods.Add(AccessTools.Method(typeof(CompAssignableToPawn_Bed), "TryUnassignPawn"));
            patchedMethods.Add(AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PostResolve"));
            patchedMethods.Add(AccessTools.Method(typeof(Dialog_AssignBuildingOwner), "DoWindowContents"));
            patchedMethods.Add(AccessTools.Method(typeof(Pawn), "Destroy"));
            patchedMethods.Add(AccessTools.Method(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn) }));
            patchedMethods.Add(AccessTools.Method(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) }));
            patchedMethods.Add(AccessTools.Method(typeof(ThoughtWorker_WantToSleepWithSpouseOrLover), "CurrentStateInternal"));
            patchedMethods.Add(AccessTools.Method(typeof(Pawn_Ownership), "ClaimBedIfNonMedical"));

            int allConflictsCount = 0;
            String conflictsRes = "";

            foreach (MethodInfo method in patchedMethods)
            {
                String methodRes = "";

                if (method == null) continue;
                Patches patches = Harmony.GetPatchInfo(method);
                if (patches == null) continue;

                methodRes += "Method: " + method.FullDescription()+"\n";
                int thisMethodConflictsCount = 0;

                methodRes += getConflictsInfo("Prefixes", patches.Prefixes, ref thisMethodConflictsCount);
                methodRes += getConflictsInfo("Transpilers", patches.Transpilers, ref thisMethodConflictsCount);
                methodRes += getConflictsInfo("Postfixes", patches.Postfixes, ref thisMethodConflictsCount);

                if (thisMethodConflictsCount > 0)
                {
                    conflictsRes += methodRes;
                    allConflictsCount += thisMethodConflictsCount;
                }
            }

            if (allConflictsCount > 0)
            {
                res += "Possible conflicts:\n";
                res += conflictsRes;
            }
            else
            {
                res += "There are no conflicts here\n";
            }

            Log.Message(res);


            String getConflictsInfo(String label, ReadOnlyCollection<HarmonyLib.Patch> patches, ref int out_count)
            {
                String patchesInfo = "";
                int count = 0;
                foreach (HarmonyLib.Patch patch in patches)
                {
                    if (patch.owner.Equals("densevoid.onebedtosleepwithall")) continue;
                    count++;
                    patchesInfo += "--- " + patch.owner + " " + patch.priority + "\n";
                }

                if (count > 0)
                {
                    String finalString = label + ":\n";
                    finalString += patchesInfo;
                    out_count += count;
                    return finalString;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
