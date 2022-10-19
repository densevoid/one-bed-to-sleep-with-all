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
using UnityEngine;

namespace OneBedToSleepWithAll.Patch
{
    [HarmonyPatch(typeof(Dialog_AssignBuildingOwner), "DoWindowContents")]
    class Dialog_AssignBuildingOwner__DoWindowContents
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            int insertionIndex = -1;

            Label nextIfLabel = il.DefineLabel();
            Label exitIfLabel = il.DefineLabel();
            Label miniJump = il.DefineLabel();
            Label returnLable = il.DefineLabel();

            /*
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Call)
                {
                    Log.Message(i + " : " + code[i].opcode.Name + "   " + code[i].operand.GetType() + "   " + (AccessTools.Method(typeof(Widgets), "ThingIcon") == ((System.Reflection.MethodInfo)code[i].operand)));
                }
            }
            */            

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldstr && code[i].operand.Equals("BuildingUnassign"))
                {
                    insertionIndex = i - 5;

                    code[insertionIndex].labels.Add(nextIfLabel);

                    int j = insertionIndex + 1;
                    while (j < code.Count) {
                        j++;

                        if (code[j].opcode == OpCodes.Brfalse_S)
                        {
                            //Log.Message("Found FALSE on line number: " + j);
                            exitIfLabel = (Label)code[j].operand;
                            break;
                        }
                    }

                    while (j < code.Count)
                    {
                        j++;

                        if (code[j].opcode == OpCodes.Leave)
                        {
                            //Log.Message("Found RETURN on line number: " + j);
                            returnLable = (Label)code[j].operand;
                            break;
                        }
                    }

                    break;
                }
            }

            var instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {
                int pawnID = 19;
                int rectID = 20;

                // CheckIsCurrentPolygamyPartner(pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, pawnID));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "CheckIsCurrentPolygamyPartner")));

                // if false go to next if
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, nextIfLabel));

                // AddMakeMasterButton(rect, pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, rectID));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, pawnID));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "AddMakeMasterButton")));                

                // if true then return
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, miniJump));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Leave, returnLable));

                // go to end of if-else
                CodeInstruction finalInstruction = new CodeInstruction(OpCodes.Br_S, exitIfLabel);
                finalInstruction.labels.Add(miniJump);
                instructionsToInsert.Add(finalInstruction);
                
                code.InsertRange(insertionIndex, instructionsToInsert);
            }

            
            insertionIndex = -1;

            Label miniJump3 = il.DefineLabel();
            Label nextIf = il.DefineLabel();
            Label exitIf = il.DefineLabel();

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i + 4].opcode == OpCodes.Callvirt && code[i + 4].operand is MethodInfo method && method == AccessTools.Method(typeof(CompAssignableToPawn), "IdeoligionForbids"))
                {
                    insertionIndex = i;
                    code[i].labels.Add(nextIf);

                    int j = insertionIndex + 1;
                    while (j < code.Count)
                    {
                        j++;

                        if (code[j].opcode == OpCodes.Br)
                        {
                            //Log.Message("Found EXITIF on line number: " + j);
                            exitIf = (Label)code[j].operand;
                            break;
                        }
                    }
                    break;
                }
            }


            instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {

                // CheckIsPolygamyBed(this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "CheckIsPolygamyBed")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, nextIf));

                int pawnID = 25;
                int rectID = 29;

                // AddMakeMasterButton(rect, pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, rectID));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, pawnID));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "AddMakeMasterButton")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Br_S, exitIf));

                code.InsertRange(insertionIndex, instructionsToInsert);
            } 

            return code;
        }
    }
}
