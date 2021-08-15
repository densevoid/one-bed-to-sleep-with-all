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
    public class CompAssignableToPawn__patching
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
}
