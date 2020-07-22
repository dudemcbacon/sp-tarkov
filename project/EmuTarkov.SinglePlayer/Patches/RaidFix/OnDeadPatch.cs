using EFT;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Reflection.CodeWrapper;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmuTarkov.SinglePlayer.Patches.RaidFix
{
    public class OnDeadPatch : GenericPatch<OnDeadPatch>
    {
        public OnDeadPatch() : base(transpiler: nameof(PatchTranspile)) {}

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnDead", PatcherConstants.DefaultBindingFlags);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where this.Speaker is called for the first time.
            var searchCode = new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Player), "Speaker"));
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
                Debug.LogError("Patch " + MethodBase.GetCurrentMethod().DeclaringType.Name + "failed: Could not find reference code");
                return instructions;
            }

            // We expected the next three callvirt instructions are the following.
            // TagBank Play
            // TaggedClip Match
            // Speaker Shut
            // We are interested in each.
            int callVirtCount = 0;
            int playIndex = -1, matchIndex = -1, lastCallIndex = -1;

            for (var i = searchIndex; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Callvirt)
                {
                    switch (callVirtCount)
                    {
                        case 0:
                            playIndex = i;
                            break;
                        case 1:
                            matchIndex = i;
                            break;
                        case 2:
                            lastCallIndex = i;
                            break;
                        default:
                            Debug.LogError("Mismatched callvirt instruction count.");
                            break;
                    }
                    searchIndex = i;
                    callVirtCount++;
                    if (callVirtCount > 2) break;
                }
            }

            if (playIndex == -1 || matchIndex == -1 || lastCallIndex == -1)
            {
                Debug.LogError("Patch " + MethodBase.GetCurrentMethod().DeclaringType.Name + "failed: Could not callvirt instructions.");
                return instructions;
            }

            Label skipLabel = generator.DefineLabel();
            Label playPostLabel = generator.DefineLabel();
            Label matchPostLabel = generator.DefineLabel();

            List<CodeInstruction> playCodes = GetCodes(ref playPostLabel, ref skipLabel);
            List<CodeInstruction> matchCodes = GetCodes(ref matchPostLabel, ref skipLabel);

            // If either of the first two calls returns null, we want to skip to the instruction after Speak Shut instruction.
            codes[playIndex + 1].labels.Add(playPostLabel);
            codes[matchIndex + 1].labels.Add(matchPostLabel);
            codes[lastCallIndex + 1].labels.Add(skipLabel);

            codes.InsertRange(playIndex + 1, playCodes);
            codes.InsertRange(matchIndex + playCodes.Count + 1, matchCodes);

            return codes.AsEnumerable();
        }

        private static List<CodeInstruction> GetCodes(ref Label postLabel, ref Label skipLabel)
        {
            return CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Dup),
                new Code(OpCodes.Ldnull),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, postLabel),
                new Code(OpCodes.Pop),
                new Code(OpCodes.Br, skipLabel)
            });
        }
    }
}
