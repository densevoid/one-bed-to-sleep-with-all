using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace OneBedToSleepWithAll.Patch
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    class Pawn__patching
    {
        static void Prefix(Pawn __instance)
        {
            PolygamyModeUtility.CleanAllNeighborClaims(__instance);
        }
    }
}
