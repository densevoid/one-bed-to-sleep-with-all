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
