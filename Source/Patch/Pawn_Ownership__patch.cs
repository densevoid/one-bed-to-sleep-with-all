using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneBedToSleepWithAll.Patch
{
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
        [HarmonyPatch(typeof(Pawn_Ownership))]
        [HarmonyPatch("ClaimBedIfNonMedical")]
        [HarmonyBefore(new string[] { "KeepBedOwnership" })]

        class Pawn_Ownership__ClaimBedIfNonMedical
        {
            static bool Prefix(Building_Bed newBed, Pawn_Ownership __instance, ref Pawn ___pawn)
            {
                CompPolygamyMode compPolygamy = newBed?.GetComp<CompPolygamyMode>();
                if (compPolygamy != null && compPolygamy.isPolygamy == true)
                {
                    if (newBed.OwnersForReading.Contains(___pawn) && compPolygamy.CurrentNeighbor == ___pawn)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

}
