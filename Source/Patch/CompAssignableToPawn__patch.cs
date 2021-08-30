using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OneBedToSleepWithAll.Patch
{
    [HarmonyPatch]
    public class CompAssignableToPawn__assign_patching
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CompAssignableToPawn_Bed), "TryAssignPawn");
            yield return AccessTools.Method(typeof(CompAssignableToPawn_Bed), "TryUnassignPawn");
        }

        public static void Postfix(CompAssignableToPawn_Bed __instance, Pawn pawn)
        {
            CompPolygamyMode cp = __instance.parent.GetComp<CompPolygamyMode>();
            if (cp != null && cp.isPolygamy)
            {
                cp.AssignesUpdated(pawn);
            }
        }
    }

    /*
    [HarmonyPatch]
    public class CompAssignableToPawn__assign_logging_patching
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CompAssignableToPawn), "TryAssignPawn");
            yield return AccessTools.Method(typeof(CompAssignableToPawn), "ForceAddPawn");
        }
        public static void Prefix(CompAssignableToPawn __instance, Pawn pawn)
        {
            Log.Message("Prefix Try assign: " + pawn.Name.ToStringShort + " Initial owners count: " + __instance.AssignedPawns.Count());
            foreach (Pawn owner in __instance.AssignedPawns)
            {
                Log.Message(owner.Name.ToStringFull);
            }
        }

        public static void Postfix(CompAssignableToPawn __instance, Pawn pawn)
        {
            Log.Message("Postfix Try assign: " + pawn.Name.ToStringShort + " Final owners count: " + __instance.AssignedPawns.Count());
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Pawn_Ownership), "OwnedBed", MethodType.Setter)]
    public class Pawn_Ownership__OwnedBed_patching
    {
        public static void Prefix(Pawn ___pawn, Building_Bed ___intOwnedBed)
        {
            Log.Message("Prefix Pawn_Ownership.OwnedBed: " + ___pawn + " bed: " + (___intOwnedBed != null ? ___intOwnedBed.ToString() : "null"));
        }
        public static void Postfix(Pawn ___pawn, Building_Bed ___intOwnedBed)
        {
            Log.Message("Postfix Pawn_Ownership.OwnedBed: " + ___pawn + " bed: " + (___intOwnedBed != null ? ___intOwnedBed.ToString() : "null"));
        }
    }
    */

    /*
    [HarmonyPatch(typeof(Pawn_Ownership), "ExposeData")]
    public class Pawn_Ownership__ExposeData_patching
    {
        public static void Prefix (Pawn ___pawn, Building_Bed ___intOwnedBed)
        {
            if (___pawn.RaceProps.Humanlike && ___pawn.Faction == Faction.OfPlayer)
                Log.Message(Scribe.mode.ToString() + " Prefix Pawn_Ownership.ExposeData: " + ___pawn + " bed: " + (___intOwnedBed != null ? ___intOwnedBed.ToString() : "null"));
        }
        public static void Postfix(Pawn ___pawn, Building_Bed ___intOwnedBed)
        {
            if (___pawn.RaceProps.Humanlike && ___pawn.Faction == Faction.OfPlayer)
                Log.Message(Scribe.mode.ToString() + " Postfix Pawn_Ownership.ExposeData: " + ___pawn + " bed: " + (___intOwnedBed != null ? ___intOwnedBed.ToString() : "null"));
        }
    }
    */
}
