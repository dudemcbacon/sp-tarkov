using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EFT.Interactive;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils;
using EmuTarkov.SinglePlayer.Utils.Reflection.CodeWrapper;
using HarmonyLib;
using UnityEngine;

namespace EmuTarkov.SinglePlayer.Patches.RaidFix
{
    // Fix null reference exception errors when bots are trying to think about using stationary weapons (i.e. on Reserve).
    public class BotStationaryWeaponPatch : GenericPatch<BotStationaryWeaponPatch>
    {
        private static readonly string kMethodName = "CheckWantTakeStationary";

        public BotStationaryWeaponPatch() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(kMethodName) != null).GetMethod(kMethodName);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(StationaryWeapon), "get_OperatorPosition"));
            int searchIndex = -1;
            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed
            if (searchIndex == -1)
            {
                PatchLogger.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }

            // Look ahead and search for a bgt instruction (should be within the next 10 lines) and get its operand. We want the same label to jump to
            // for our code below.
            Label jumpToLabel = default(Label);
            bool labelFound = false;
            for (var i = searchIndex; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Bgt_S)
                {
                    jumpToLabel = (Label)codes[i].operand;
                    break;
                }
                labelFound = true;
            }

            if (!labelFound)
            {
                Debug.LogError("Label not found.");
                return instructions;
            }

            // This is start of the instruction that we are interested in.
            searchIndex -= 2;

            List<CodeInstruction> newCodes = CodeGenerator.GenerateInstructions(new List<Code>() {
                new Code(OpCodes.Ldloc_3),
                new Code(OpCodes.Ldfld, typeof(StationaryWeaponLink), "Weapon"),
                new Code(OpCodes.Ldnull),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brtrue_S, jumpToLabel)
            });

            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }
    }
}
