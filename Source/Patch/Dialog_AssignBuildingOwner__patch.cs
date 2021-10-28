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
                Log.Message(i + " : " + code[i].opcode.Name + "   " + code[i].operand);
            }
            */

            for (int i = 0; i < code.Count - 17; i++)
            {
                if (code[i + 1].opcode == OpCodes.Ldstr && code[i + 1].operand.Equals("BuildingUnassign"))
                {
                    insertionIndex = i;
                    code[i].labels.Add(nextIfLabel);
                    exitIfLabel = (Label)code[i + 8].operand;
                    returnLable = (Label)code[i + 17].operand;
                    break;
                }
            }

            var instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {
                // CheckIsCurrentPolygamyPartner(pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 5));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "CheckIsCurrentPolygamyPartner")));

                // if false go to next if
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, nextIfLabel));

                // AddMakeMasterButton(rect, pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 6));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 5));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
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
            //LocalBuilder isItPolygamyBed = il.DeclareLocal(typeof(bool));
            //isItPolygamyBed.SetLocalSymInfo("isItPolygamyBed");

            for (int i = 0; i < code.Count - 25; i++)
            {
                if (code[i + 4].opcode == OpCodes.Callvirt && code[i + 4].operand is MethodInfo method && method == AccessTools.Method(typeof(CompAssignableToPawn), "IdeoligionForbids"))
                {
                    insertionIndex = i;
                    code[i].labels.Add(nextIf);
                    exitIf = (Label)code[i + 34].operand;
                    //code[i].labels.Add(miniJump);
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
                //instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_S, isItPolygamyBed.LocalIndex));

                //instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, isItPolygamyBed.LocalIndex));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, nextIf));


                // TODO: сделать тут AddMakeMasterButton, а третий блок вставки походу вообще убрать.

                // if (!isItPolygamyBed) 
                
               //instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, isItPolygamyBed.LocalIndex));
                //instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue_S, miniJump3));
                

                // AddMakeMasterButton(rect, pawn, this.assignable.parent);
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 14));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, 7));
                //instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(RimWorld.Dialog_AssignBuildingOwner/ '<>c__DisplayClass8_0'), "p")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Dialog_AssignBuildingOwner), "assignable")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ThingComp), "parent")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "AddMakeMasterButton")));

                // if true then return
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, miniJump3));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Leave, returnLable));

                // go to end of if-else
                CodeInstruction finalInstruction = new CodeInstruction(OpCodes.Br_S, exitIf);
                finalInstruction.labels.Add(miniJump3);
                instructionsToInsert.Add(finalInstruction);

                code.InsertRange(insertionIndex, instructionsToInsert);

            }


            /*
            insertionIndex = -1;

            Label miniJump2 = il.DefineLabel();

            for (int i = 3; i < code.Count; i++)
            {
                if (code[i - 3].opcode == OpCodes.Ldstr && code[i - 3].operand.Equals("BuildingReassign"))
                {
                    insertionIndex = i;
                    code[i].labels.Add(miniJump2);
                    break;
                }
            }

            instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {
                // if (isItPolygamyBed) 
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc_S, isItPolygamyBed.LocalIndex));

                // if true then taggedString = "polygamyMode_MakeAMaster".translate()
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse_S, miniJump2));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldstr, "polygamyMode_MakeAMaster"));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Translator), "Translate", new Type[] { typeof(string) })));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc_S, 15));

                code.InsertRange(insertionIndex, instructionsToInsert);
            }
            */

            return code;
        }
    }
}
