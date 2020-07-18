using UnityEngine;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Patches.Bots;
using EmuTarkov.SinglePlayer.Patches.Matchmaker;
using EmuTarkov.SinglePlayer.Patches.Progression;
using EmuTarkov.SinglePlayer.Patches.Quests;
using EmuTarkov.SinglePlayer.Patches.ScavMode;
using EmuTarkov.SinglePlayer.Utils;

namespace EmuTarkov.SinglePlayer
{
    public class Instance : MonoBehaviour
    {
        private void Start()
		{
            Debug.LogError("EmuTarkov.SinglePlayer: Loaded");

			// todo: find a way to get php session id
			new Settings(null, Utils.Config.BackendUrl);

			PatcherUtil.PatchPrefix<OfflineLootPatch>();
			PatcherUtil.PatchPrefix<OfflineSaveProfilePatch>();
			PatcherUtil.PatchPostfix<WeaponDurabilityPatch>();
            PatcherUtil.PatchPostfix<SingleModeJamPatch>();
            
            PatcherUtil.Patch<Patches.Healing.MainMenuControllerPatch>();
			PatcherUtil.Patch<Patches.Healing.PlayerPatch>();

			PatcherUtil.PatchPostfix<MatchmakerOfflineRaidPatch>();
			PatcherUtil.PatchPostfix<MatchMakerSelectionLocationScreenPatch>();
			PatcherUtil.Patch<InsuranceScreenPatch>();

            PatcherUtil.Patch<BossSpawnChancePatch>();
			PatcherUtil.PatchPostfix<BotTemplateLimitPatch>();
            PatcherUtil.PatchPrefix<GetNewBotTemplatesPatch>();
            PatcherUtil.PatchPrefix<RemoveUsedBotProfilePatch>();
            PatcherUtil.PatchPrefix<SpawnPmcPatch>();
			PatcherUtil.PatchPrefix<CoreDifficultyPatch>();
			PatcherUtil.PatchPrefix<BotDifficultyPatch>();

			PatcherUtil.PatchPrefix<BeaconPatch>();
			PatcherUtil.PatchPostfix<DogtagPatch>();

            PatcherUtil.Patch<LoadOfflineRaidScreenPatch>();
            PatcherUtil.Patch<ScavPrefabLoadPatch>();
            PatcherUtil.Patch<ScavProfileLoadPatch>();
            PatcherUtil.Patch<ScavSpawnPointPatch>();
            PatcherUtil.Patch<ScavExfilPatch>();

            PatcherUtil.Patch<EndByTimerPatch>();
        }
    }
}
