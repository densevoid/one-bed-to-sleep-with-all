using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace OneBedToSleepWithAll.Patch
{

    /*
    [HarmonyPatch(typeof(MainMenuDrawer))]
    [HarmonyPatch("Init")]

    class MainMenuDrawer__Init
    {
        static void Postfix()
        {
            Log.Message("INIT!");

            MethodInfo info = AccessTools.Method(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn) });
            if (info != null)
            {
                Log.Message("Method: " + info.ToString());
                Patches patches = Harmony.GetPatchInfo(info);
                Log.Message("- Prefixes: ");
                foreach (HarmonyLib.Patch prefix in patches.Prefixes)
                {
                    Log.Message("--- " + prefix.owner + " " + prefix.priority);
                }
                Log.Message("- Transpilers: ");
                foreach (HarmonyLib.Patch transpilers in patches.Transpilers)
                {
                    Log.Message("--- " + transpilers.owner + " " + transpilers.priority);
                }
                Log.Message("- Postfixes: ");
                foreach (HarmonyLib.Patch postfix in patches.Postfixes)
                {
                    Log.Message("--- " + postfix.owner + " " + postfix.priority);
                }
            }

            MethodInfo info2 = AccessTools.Method(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) });
            if (info2 != null)
            {
                Log.Message("Method: " + info2.ToString());
                Patches patches = Harmony.GetPatchInfo(info2);
                Log.Message("- Prefixes: ");
                foreach (HarmonyLib.Patch prefix in patches.Prefixes)
                {
                    Log.Message("--- " + prefix.owner + " " + prefix.priority);
                }
                Log.Message("- Transpilers: ");
                foreach (HarmonyLib.Patch transpilers in patches.Transpilers)
                {
                    Log.Message("--- " + transpilers.owner + " " + transpilers.priority);
                }
                Log.Message("- Postfixes: ");
                foreach (HarmonyLib.Patch postfix in patches.Postfixes)
                {
                    Log.Message("--- " + postfix.owner + " " + postfix.priority);
                }
            }
        }
    }
    */
    
    [HarmonyPatch(typeof(RestUtility))]
    [HarmonyPatch("FindBedFor")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) })]
    [HarmonyAfter(new string[] { "KeepBedOwnership" })]

    class RestUtility__FindBedFor
    {
        static void Postfix(ref Building_Bed __result, Pawn sleeper, Pawn traveler, bool checkSocialProperness, bool ignoreOtherReservations, GuestStatus? guestStatus)
        {
            Building_Bed sleeper_bed = sleeper.ownership.OwnedBed;
            if (sleeper_bed != null)
            {
                CompPolygamyMode polygamyComp = sleeper_bed.GetComp<CompPolygamyMode>();
                if (polygamyComp != null && polygamyComp.isPolygamy && polygamyComp.Master == sleeper)
                {
                    return;
                }
            }

            Building_Bed partnersPolygamyBed = PolygamyModeUtility.CheckIsHavePartnersPolygamyBed(sleeper, traveler, checkSocialProperness, ignoreOtherReservations, guestStatus);
            if (partnersPolygamyBed != null)
                __result = partnersPolygamyBed;
        }
    }
}
