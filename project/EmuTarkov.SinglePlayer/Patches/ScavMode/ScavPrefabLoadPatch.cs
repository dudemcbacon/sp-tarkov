﻿using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EmuTarkov.Common.Utils.Patching;
using EFT;
using UnityEngine;
using HarmonyLib;
using BackendInterface = GInterface23;
using SessionInterface = GInterface24;

namespace EmuTarkov.SinglePlayer.Patches.ScavMode
{
    public class ScavPrefabLoadPatch : GenericPatch<ScavPrefabLoadPatch>
    {
        public ScavPrefabLoadPatch() : base(transpiler: nameof(PatchTranspile)) {}

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication).GetNestedType("Struct129", BindingFlags.NonPublic)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(x => x.Name == "MoveNext");
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

            // Move back by 3. This is the start of IL chain that we're interested in.
            searchIndex -= 3;

            Label brFalseLabel = generator.DefineLabel();
            Label brLabel= generator.DefineLabel();

            List<CodeInstruction> newCodes = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ClientApplication), "_backEnd")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BackendInterface), "get_Session")),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(MainApplication), "esideType_0")),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, brFalseLabel),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SessionInterface), "get_Profile")),
                new CodeInstruction(OpCodes.Br, brLabel),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SessionInterface), "get_ProfileOfPet")) {
                    labels = { brFalseLabel }
                },
                new CodeInstruction(OpCodes.Ldc_I4_1)
                {
                    labels = { brLabel }
                }
            };

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }
    }
}
