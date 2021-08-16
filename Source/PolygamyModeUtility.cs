using OneBedToSleepWithAll.Patch;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace OneBedToSleepWithAll
{
    public class PolygamyModeUtility
    {
        public static Building_Bed CheckIsHavePartnersPolygamyBed(Pawn sleeper, bool checkSocialProperness, bool ignoreOtherReservations = false, GuestStatus? guestStatus = null)
        {
            if (sleeper == null) return null;

            Building_Bed sleeper_bed = sleeper.ownership.OwnedBed;
            if (sleeper_bed != null)
            {
                CompPolygamyMode polygamyComp = sleeper_bed.GetComp<CompPolygamyMode>();
                if (polygamyComp != null && polygamyComp.isPolygamy && polygamyComp.Master == sleeper)
                {
                    return null;
                }
            }

            foreach (DirectPawnRelation relation in sleeper.GetLoveRelations(false))
            {
                Building_Bed otherBed = relation.otherPawn.ownership.OwnedBed;
                if (otherBed == null) continue;

                CompPolygamyMode polygamyComp = otherBed.GetComp<CompPolygamyMode>();
                if (polygamyComp == null) continue;

                if (polygamyComp.CurrentNeighbor != null && polygamyComp.CurrentNeighbor == sleeper && RestUtility.IsValidBedFor(otherBed, sleeper, sleeper, checkSocialProperness, false, ignoreOtherReservations, guestStatus))
                {
                    return otherBed;
                }
            }

            return null;
        }

        public static bool SimpleCheckIsHavePartnersPolygamyBed(Pawn pawn)
        {
            Log.Message("checking 2" + pawn.Name.ToStringShort);
            if (pawn == null) return false;

            foreach (DirectPawnRelation relation in pawn.GetLoveRelations(false))
            {
                Log.Message("1");
                Building_Bed otherBed = relation.otherPawn.ownership.OwnedBed;
                if (otherBed == null) continue;
                Log.Message("2");

                CompPolygamyMode polygamyComp = otherBed.GetComp<CompPolygamyMode>();
                if (polygamyComp == null || !polygamyComp.isPolygamy) continue;
                Log.Message("3");

                return true;
            }
            Log.Message("4");

            return false;
        }


        public static bool CheckIsAPolygamyMaster(Pawn pawn)
        {
            if (pawn == null) return false;
            Log.Message("checking 1" + pawn.Name.ToStringShort);

            Building_Bed bed = pawn.ownership.OwnedBed;
            if (bed == null) return false;

            CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();
            if (polygamyComp == null || !polygamyComp.isPolygamy || polygamyComp.Master != pawn) return false;

            return true;
        }


        internal static void CleanAllNeighborClaims(Pawn pawn)
        {
            if (pawn == null || pawn.NonHumanlikeOrWildMan()) return;


            foreach(DirectPawnRelation relation in pawn.GetLoveRelations(false))
            {
                if (relation.otherPawn == null || relation.otherPawn.ownership == null || relation.otherPawn.ownership.OwnedBed == null) continue;

                Building_Bed bed = relation.otherPawn.ownership.OwnedBed;

                CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();
                if (polygamyComp == null) continue;

                if (polygamyComp.isPolygamy && polygamyComp.CurrentNeighbor == pawn)
                {
                    polygamyComp.CurrentNeighbor = null;
                }
            }
        }

        public static bool IsItPolygamyNeighbor(Pawn pawn, Building_Bed bed)
        {
            CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();

            if (polygamyComp != null && polygamyComp.isPolygamy && polygamyComp.CurrentNeighbor == pawn)
            {
                return true;
            }

            return false;
        }

        public static bool CheckIsCurrentPolygamyPartner(Pawn pawn, ThingWithComps parent)
        {
            Building_Bed bed = parent as Building_Bed;
            if (bed != null)
            {
                CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();
                if (polygamyComp != null && polygamyComp.isPolygamy)
                {
                    if (polygamyComp.CurrentNeighbor == pawn)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AddMakeMasterButton(Rect rect, Pawn pawn, ThingWithComps parent)
        {
            Building_Bed bed = parent as Building_Bed;
            
            if (bed == null)
            {
                Log.Error("Can't find bed for MakeMasterButton");
                return false;
            }

            CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();

            if (polygamyComp == null)
            {
                Log.Error("Can't find polygamyComp for MakeMasterButton");
                return false;
            }

            AcceptanceReport acceptanceReport = bed.CompAssignableToPawn.CanAssignTo(pawn);
            bool accepted = acceptanceReport.Accepted;

            if (Widgets.ButtonText(rect, "polygamyMode_MakeAMaster".Translate(), true, true, accepted))
            {
                if (polygamyComp.Master != null)
                    bed.GetComp<CompAssignableToPawn>().TryUnassignPawn(polygamyComp.Master);

                bed.GetComp<CompAssignableToPawn>().TryAssignPawn(pawn);

                SoundDefOf.Click.PlayOneShotOnCamera(null);
                return true;
            }

            return false;
        }

        public static bool CheckIsPolygamyBed(ThingWithComps parent)
        {

            Building_Bed bed = parent as Building_Bed;

            if (bed == null)
            {
                return false;
            }

            CompPolygamyMode polygamyComp = bed.GetComp<CompPolygamyMode>();

            if (polygamyComp == null)
            {
                return false;
            }

            if (polygamyComp.isPolygamy)
                return true;
            else
                return false;
        }
    }
}
