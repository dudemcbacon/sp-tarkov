using System.Linq;
using System.Collections.Generic;
using EmuTarkov.Common.Utils.Patching;
using EFT;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using BackendInterface = GInterface23;
using SessionInterface = GInterface24;

namespace EmuTarkov.SinglePlayer.Patches.ScavMode
{
    public class ScavProfileLoadPatch : GenericPatch<ScavProfileLoadPatch>
    {
        public ScavProfileLoadPatch() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(IsTargetMethod);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where backend.Session.getProfile() is called.
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SessionInterface), "get_Profile"));
            int searchIndex = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed.
            if (searchIndex == -1)
            {
                Debug.LogError("Patch " + MethodBase.GetCurrentMethod().DeclaringType.Name + "failed: Could not find reference code.");
                return instructions;
            }

            // Move back by 4. This is the start of this method call.
            // Note that we don't actually want to replace the code at searchIndex (which is a Ldloc0) since there is a branch
            // instruction prior to this instruction that leads to it and we can reuse a Ldloc0 instruction here.
            searchIndex -= 4;

            Label brFalseLabel = generator.DefineLabel();
            Label brLabel = generator.DefineLabel();

            List<CodeInstruction> newCodes = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ClientApplication), "_backEnd")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BackendInterface), "get_Session")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MainApplication), "esideType_0")),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, brFalseLabel),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SessionInterface), "get_Profile")),
                new CodeInstruction(OpCodes.Br, brLabel),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SessionInterface), "get_ProfileOfPet"))
                {
                    labels = { brFalseLabel }
                },
                new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(MainApplication).GetNestedType("Class738", BindingFlags.NonPublic), "profile"))
                {
                    labels = { brLabel }
                }
            };

            codes.RemoveRange(searchIndex + 1, 5);
            codes.InsertRange(searchIndex + 1, newCodes);

            return codes.AsEnumerable();
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 3
                || parameters[0].Name != "location"
                || parameters[1].Name != "timeAndWeather"
                || parameters[2].Name != "entryPoint"
                || parameters[2].ParameterType != typeof(string)
                || methodInfo.ReturnType != typeof(void))
                return false;
            return true;
        }


    }
}
