﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OneBedToSleepWithAll
{
    public class CompPolygamyMode : ThingComp
    {
        public int QueueTime
        {
            get { return Mathf.RoundToInt(2500 * OneBedToSleepWithAll.Settings.requiredSleepHours); }
        }

        public bool isPolygamy;

        private Pawn master;

        public Pawn Master
        {
            get { return master; }
        }

        private Pawn currentNeighbor;
        public Pawn CurrentNeighbor
        {
            get { return currentNeighbor;  }

            set {
                if (value != currentNeighbor)
                {
                    Building_Bed bed = parent as Building_Bed;

                    if (bed != null)
                    {
                        if (currentNeighbor != null)
                            bed.CompAssignableToPawn.ForceRemovePawn(currentNeighbor);

                        if (value != null)
                            bed.CompAssignableToPawn.ForceAddPawn(value);
                    }

                    currentNeighbor = value;
                }
            }
        }

        private List<DirectPawnRelation> loveRelations;
        public List<DirectPawnRelation> LoveRelations
        {
            get
            {
                if (loveRelations == null)
                {
                    updateLoveRelations();
                }

                return loveRelations;
            }
        }

        private void updateLoveRelations()
        {
            Building_Bed bed = parent as Building_Bed;

            if (bed == null || Master == null)
            {
                loveRelations = null;
                return;
            }

            loveRelations = Master.GetLoveRelations(false).ListFullCopy();

            loveRelations.RemoveAll(relation =>
            {
                bool result = !BedUtility.WillingToShareBed(master, relation.otherPawn) || relation.otherPawn.Map != bed.Map;
                return result;
            });

        }

        public int timeForNextNeighbor;

        public bool IsOwnerInBed
        {
            get { return isOwnerInBed; }
        }

        private bool isOwnerInBed;

        public override void Initialize(CompProperties props)
        {
            this.isPolygamy = false;
            this.timeForNextNeighbor = QueueTime;
            this.isOwnerInBed = false;
        }

        internal void AssignesUpdated(Pawn pawn)
        {
            Building_Bed bed = parent as Building_Bed;

            if (bed.OwnersForReading.Contains(pawn) && master != null && master != pawn)
            {
                foreach (Pawn owner in bed.OwnersForReading.ListFullCopy())
                {
                    if (owner != pawn) owner.ownership.UnclaimBed();
                }
            }

            DefineMaster();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.isPolygamy, "isPolygamy", false, false);
            Scribe_Values.Look<int>(ref this.timeForNextNeighbor, "timeForNextNeighbor", QueueTime, false);
            Scribe_References.Look<Pawn>(ref this.master, "polygamyOwner");
            Scribe_References.Look<Pawn>(ref this.currentNeighbor, "currentNeighbor");
            Scribe_Values.Look<bool>(ref this.isOwnerInBed, "isOwnerInBed", false, false);
        }

        public void DefineMaster()
        {
            Building_Bed bed = parent as Building_Bed;

            if (bed != null)
            {
                ResetLight();

                foreach (Pawn pawn in bed.OwnersForReading.ListFullCopy())
                {
                    if (master != null)
                    {
                        pawn.ownership.UnclaimBed();
                        continue;
                    }

                    master = pawn;
                }

                UpdateCondition();
            }
        }

        public void UpdateCondition()
        {
            Building_Bed bed = parent as Building_Bed;

            if (isPolygamy)
            {
                if (Master == null && bed.OwnersForReading.Count > 1)
                {
                    DefineMaster();
                    return;
                }
                if (bed.OwnersForReading.Contains(Master))
                {
                    updateLoveRelations();

                    if (CurrentNeighbor == null)
                    {
                        if (LoveRelations.Count > 0)
                        {
                            CurrentNeighbor = LoveRelations.First().otherPawn;
                            timeForNextNeighbor = QueueTime;
                        }
                    }

                    else
                    {
                        bool relationConfirmed = false;

                        foreach (DirectPawnRelation relation in LoveRelations)
                        {
                            if (relation.otherPawn == CurrentNeighbor)
                            {
                                relationConfirmed = true;
                                break;
                            }
                        }

                        if (!relationConfirmed)
                        {
                            //TODO: make an honest queue so that in the event of a break in relations, the queue is not reset to zero.
                            //TODO: try to replace this hack with an honest miscalculation of all cases of divorce and relationship breakups
                            CurrentNeighbor = null;
                            UpdateCondition();
                            return;
                        }

                        if (LoveRelations.Count > 1)
                        {
                            isOwnerInBed = ((Building_Bed)parent).CurOccupants.Contains(Master);

                            if (isOwnerInBed)
                            {
                                timeForNextNeighbor -= 250;

                                if (timeForNextNeighbor < 0)
                                    timeForNextNeighbor = 0;
                            }

                            if (timeForNextNeighbor <= 0 && bed.CurOccupants.Count() <= 0)
                            {
                                //next queue
                                int curIndex = -1;

                                foreach (DirectPawnRelation relation in LoveRelations)
                                {
                                    if (relation.otherPawn == CurrentNeighbor)
                                    {
                                        curIndex = LoveRelations.IndexOf(relation);
                                    }
                                }
                                CurrentNeighbor = GetNextNeighbor(LoveRelations, curIndex);
                                timeForNextNeighbor = QueueTime;
                            }
                        }
                    }
                }
                else if (Master != null)
                {
                    ResetLight();
                }
            }
        }


        public static Pawn GetNextNeighbor(List<DirectPawnRelation> list, int prevNeighborIndex)
        {
            int nextIndex = (prevNeighborIndex + 1) % list.Count;
            return list[nextIndex].otherPawn;
        }


        public override void CompTickRare()
        {
            UpdateCondition();
        }


        internal void ResetLight()
        {
            master = null;
            CurrentNeighbor = null;
            timeForNextNeighbor = QueueTime;
            updateLoveRelations();
        }

        internal void ResetAll()
        {
            ResetLight();

            Building_Bed bed = parent as Building_Bed;

            foreach (Pawn pawn in bed.OwnersForReading.ListFullCopy())
            {
                pawn.ownership.UnclaimBed();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Building_Bed bed = parent as Building_Bed;

            if (bed.def.building.bed_humanlike && parent.Faction == Faction.OfPlayer && (bed.ForColonists || bed.ForSlaves) && bed.SleepingSlotsCount > 1)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "polygamyMode_Polygamy".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/PolygamyModeGizmo", false),
                    defaultDesc = "polygamyMode_enablePolygamy".Translate(),
                    isActive = () => isPolygamy,
                    toggleAction = delegate ()
                    {
                        isPolygamy = !isPolygamy;
                        if (isPolygamy) DefineMaster();
                        else PolygamyModeDisabled();
                    },
                };
            }
        }

        private void PolygamyModeDisabled()
        {
            ResetLight();
        }

        public override void PostDeSpawn(Map map)
        {
            ResetLight();
            isPolygamy = false;
        }
    }
}