using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace OneBedToSleepWithAll.Patch
{
    [HarmonyPatch(typeof(RestUtility))]
    [HarmonyPatch("FindBedFor")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) })]

    class RestUtility__FindBedFor
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);
            int insertionIndex = -1;

            Label returnLabel = il.DefineLabel();

            for (int i = 0; i < code.Count - 3; i++)
            {
                if (code[i].opcode == OpCodes.Ldloc_0 && code[i + 1].opcode == OpCodes.Ldfld && code[i + 3].opcode == OpCodes.Brfalse_S)
                {
                    insertionIndex = i;
                    code[i].labels.Add(returnLabel);
                    break;
                }
            }

            var instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_0));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_2));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg, 3));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg, 4));

                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "CheckIsHavePartnersPolygamyBed")));

                LocalBuilder partnersPolygamyBed = il.DeclareLocal(typeof(Building_Bed));
                partnersPolygamyBed.SetLocalSymInfo("partnersPolygamyBed");

                instructionsToInsert.Add(new CodeInstruction(OpCodes.Stloc, partnersPolygamyBed.LocalIndex));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc, partnersPolygamyBed.LocalIndex));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse, returnLabel));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldloc, partnersPolygamyBed.LocalIndex));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ret));

                code.InsertRange(insertionIndex, instructionsToInsert);
            }
            return code;
        }
    }
}
