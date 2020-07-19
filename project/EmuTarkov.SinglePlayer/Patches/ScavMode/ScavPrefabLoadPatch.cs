﻿using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Reflection.CodeWrapper;
using EFT;
using UnityEngine;
using HarmonyLib;

namespace EmuTarkov.SinglePlayer.Patches.ScavMode
{
    public class ScavPrefabLoadPatch : GenericPatch<ScavPrefabLoadPatch>
    {
        public ScavPrefabLoadPatch() : base(transpiler: nameof(PatchTranspile)) {}

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).
                Single(x => x.GetField("entryPoint") != null && x.GetField("timeAndWeather") != null && x.Name.Contains("Struct"))
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(x => x.Name == "MoveNext");
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where backend.Session.getProfile() is called.
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(PatcherConstants.SessionInterfaceType, "get_Profile"));
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

            List<CodeInstruction> newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(ClientApplication), "_backEnd"),
                new Code(OpCodes.Callvirt, PatcherConstants.BackendInterfaceType, "get_Session"),
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(MainApplication), "esideType_0"),
                new Code(OpCodes.Ldc_I4_0),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Callvirt, PatcherConstants.SessionInterfaceType, "get_Profile"),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Callvirt, brFalseLabel, PatcherConstants.SessionInterfaceType, "get_ProfileOfPet"),
                new CodeWithLabel(OpCodes.Ldc_I4_1, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }
    }
}
