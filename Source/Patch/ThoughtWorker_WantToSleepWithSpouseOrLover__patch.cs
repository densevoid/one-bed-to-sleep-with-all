using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
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
    [HarmonyPatch(typeof(ThoughtWorker_WantToSleepWithSpouseOrLover))]
    [HarmonyPatch("CurrentStateInternal")]
    class ThoughtWorker_WantToSleepWithSpouseOrLover__patch
    {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);
            int insertionIndex = -1;

            Label myIfLabel = il.DefineLabel();
            Label nextIfLabel = il.DefineLabel();
            Label miniJump = il.DefineLabel();

            for (int i = 4; i < code.Count - 4; i++)
            {
                if (code[i + 4].opcode == OpCodes.Ldsfld && code[i + 4].operand is FieldInfo field && field == AccessTools.Field(typeof(PawnRelationDefOf), "Spouse"))
                {
                    insertionIndex = i;
                    code[i - 4].operand = myIfLabel;
                    code[i].labels.Add(nextIfLabel);
                    break;
                }
            }

            var instructionsToInsert = new List<CodeInstruction>();

            if (insertionIndex != -1)
            {
                // if have master
                CodeInstruction myIfInstruction = new CodeInstruction(OpCodes.Ldarg_1);
                myIfInstruction.labels.Add(myIfLabel);
                instructionsToInsert.Add(myIfInstruction);

                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "SimpleCheckIsHavePartnersPolygamyBed")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue, miniJump));

                // or itself master
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_1));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PolygamyModeUtility), "CheckIsAPolygamyMaster")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse, nextIfLabel));

                // then return
                CodeInstruction resultInstruction = new CodeInstruction(OpCodes.Ldc_I4_0);
                resultInstruction.labels.Add(miniJump);
                instructionsToInsert.Add(resultInstruction);

                instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThoughtState), "op_Implicit")));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ret));

                code.InsertRange(insertionIndex, instructionsToInsert);
            }

            return code;
        }
    }
}
