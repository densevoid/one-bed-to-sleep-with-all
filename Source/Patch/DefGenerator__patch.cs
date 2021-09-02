using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace OneBedToSleepWithAll.Patch
{
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PostResolve")]
    class DefGenerator__GenerateImpliedDefs_PostResolve
    {
        static void Postfix()
        {
            foreach (ThingDef item in DefDatabase<ThingDef>.AllDefs)
            {
                if (item.thingClass == typeof(Building_Bed) || typeof(Building_Bed).AllSubclassesNonAbstract().Contains(item.thingClass))
                {
                    int slotCount = BedUtility.GetSleepingSlotsCount(item.size);

                    if (slotCount > 1 && item.comps != null && !item.comps.Any(x => x is CompProperties_PolygamyMode))
                    {
                        item.comps.Add(new CompProperties_PolygamyMode());

                        if (item.tickerType != TickerType.Normal)
                            item.tickerType = TickerType.Rare;
                    }
                }
            }
        }
    }
}
