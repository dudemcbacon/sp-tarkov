using System.Linq;
using System.Collections.Generic;
using EmuTarkov.Common.Utils.Patching;
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

            List<CodeInstruction> newCodes = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(GetBaseLocalGameType(), "get_Profile_0")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Profile), "Info")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ProfileInfo), "Side")),
                new CodeInstruction(OpCodes.Ldc_I4_4),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse, brFalseLabel),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExfilPointManager), "get_Instance")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(GetBaseLocalGameType(), "gparam_0")),
                new CodeInstruction(OpCodes.Box, typeof(PlayerOwner)),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PlayerOwner), "get_Player")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Player), "get_Position")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(GetBaseLocalGameType(), "get_Profile_0")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Profile), "Id")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ExfilPointManager), "ScavExfiltrationClaim", new System.Type[]{ typeof(Vector3), typeof(string) })),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExfilPointManager), "get_Instance")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExfilPointManager), "get_Instance")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(GetBaseLocalGameType(), "get_Profile_0")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Profile), "Id")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ExfilPointManager), "GetScavExfiltrationMask")),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(GetBaseLocalGameType(), "get_Profile_0")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Profile), "Id")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ExfilPointManager), "ScavExfiltrationClaim", new System.Type[]{ typeof(int), typeof(string) })),
                new CodeInstruction(OpCodes.Br, brLabel),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ExfilPointManager), "get_Instance")) { labels = { brFalseLabel } },
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(GetBaseLocalGameType(), "get_Profile_0")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ExfilPointManager), "EligiblePoints", new System.Type[]{ typeof(Profile) })),
                new CodeInstruction(OpCodes.Stloc_0) { labels = { brLabel } }
            };

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
