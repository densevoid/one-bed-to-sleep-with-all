using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OneBedToSleepWithAll.Patch
{

    [HarmonyPatch(typeof(Building_Bed), "GetInspectString")]
    class Building_Bed__GetInspectString
    { 
        static void Postfix(ref string __result, Building_Bed __instance)
        {
            CompPolygamyMode polygamyComp = __instance.GetComp<CompPolygamyMode>();

            if (polygamyComp != null)
            {
                if (polygamyComp.isPolygamy)
                {
                    Pawn neighbor = null;
                    String masterName = "Nobody".Translate();
                    String neighborName = "Nobody".Translate();

                    if (polygamyComp.Master != null)
                        masterName = polygamyComp.Master.Name.ToStringFull;

                    if (polygamyComp.CurrentNeighbor != null)
                        neighbor = polygamyComp.CurrentNeighbor;

                    if (neighbor != null)
                    {
                        neighborName = neighbor.Name.ToStringFull;
                    }

                    __result += "\n" + "polygamyMode_Master".Translate() + ": " + masterName;

                    if (polygamyComp.Master != null)
                    {
                        __result += " (" + "polygamyMode_partnersCount".Translate() + ": " + polygamyComp.LoveRelations.Count + ")";

                        __result += "\n" + "polygamyMode_currentPartner".Translate() + ": " + neighborName;

                        __result += "\n" + "polygamyMode_status".Translate() + ": ";

                        String status = "polygamyMode_nothingIsHappening".Translate();

                        if (neighbor != null)
                        {
                            if (polygamyComp.LoveRelations.Count == 1)
                            {
                                status = "polygamyMode_permanentPartner".Translate();
                            }
                            else if (polygamyComp.timeForNextNeighbor > 0)
                            {
                                if (!polygamyComp.IsOwnerInBed)
                                    status = "polygamyMode_waitingForMaster".Translate();
                                else
                                    status = Math.Round(GenDate.TicksToDays(polygamyComp.timeForNextNeighbor) * 24, 1) + " " + "polygamyMode_hoursOfSleepLeft".Translate();
                            }
                            else
                                status = "polygamyMode_waitingForWakeUp".Translate();

                            __result += status;
                        }
                    }
                }
            }
        }
    }
}
