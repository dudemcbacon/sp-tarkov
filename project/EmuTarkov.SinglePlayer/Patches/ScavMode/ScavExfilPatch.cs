using System.Linq;
using System.Collections.Generic;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Reflection.CodeWrapper;
using EFT;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ExfilPointManager = GClass648;
using ProfileInfo = GClass1044;

namespace EmuTarkov.SinglePlayer.Patches.ScavMode
{
    public class ScavExfilPatch : GenericPatch<ScavExfilPatch>
    {
        public ScavExfilPatch() : base(transpiler: nameof(PatchTranspile)) { }

        protected override MethodBase GetTargetMethod()
        {
            return GetBaseLocalGameType().GetMethod("vmethod_4",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ExfilPointManager), "EligiblePoints", new System.Type[] { typeof(Profile) }));
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
            }

            searchIndex -= 3;

            Label brFalseLabel = generator.DefineLabel();
            Label brLabel = generator.DefineLabel();
            List<CodeInstruction> newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, GetBaseLocalGameType(), "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Info"),
                new Code(OpCodes.Ldfld, typeof(ProfileInfo), "Side"),
                new Code(OpCodes.Ldc_I4_4),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Call, typeof(ExfilPointManager), "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, GetBaseLocalGameType(), "gparam_0"),
                new Code(OpCodes.Box, typeof(PlayerOwner)),
                new Code(OpCodes.Callvirt, typeof(PlayerOwner), "get_Player"),
                new Code(OpCodes.Callvirt, typeof(Player), "get_Position"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, GetBaseLocalGameType(), "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, typeof(ExfilPointManager), "ScavExfiltrationClaim", new System.Type[]{ typeof(Vector3), typeof(string) }),
                new Code(OpCodes.Call, typeof(ExfilPointManager), "get_Instance"),
                new Code(OpCodes.Call, typeof(ExfilPointManager), "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, GetBaseLocalGameType(), "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, typeof(ExfilPointManager), "GetScavExfiltrationMask"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, GetBaseLocalGameType(), "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, typeof(ExfilPointManager), "ScavExfiltrationClaim", new System.Type[]{ typeof(int), typeof(string) }),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Call, brFalseLabel, typeof(ExfilPointManager), "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, GetBaseLocalGameType(), "get_Profile_0"),
                new Code(OpCodes.Callvirt, typeof(ExfilPointManager), "EligiblePoints", new System.Type[]{ typeof(Profile) }),
                new CodeWithLabel(OpCodes.Stloc_0, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }

        private static System.Type GetBaseLocalGameType()
        {
            return PatcherConstants.TargetAssembly.GetTypes().Single(x => x.Name == "BaseLocalGame`1").MakeGenericType(typeof(GamePlayerOwner));
        }
    }
}
