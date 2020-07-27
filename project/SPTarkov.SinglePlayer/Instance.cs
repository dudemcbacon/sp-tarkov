using UnityEngine;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Patches.Bots;
using SPTarkov.SinglePlayer.Patches.Matchmaker;
using SPTarkov.SinglePlayer.Patches.Progression;
using SPTarkov.SinglePlayer.Patches.Quests;
using SPTarkov.SinglePlayer.Patches.RaidFix;
using SPTarkov.SinglePlayer.Patches.ScavMode;
using SPTarkov.SinglePlayer.Utils;

namespace SPTarkov.SinglePlayer
{
    public class Instance : MonoBehaviour
    {
        private void Start()
		{
            Debug.LogError("SPTarkov.SinglePlayer: Loaded");

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
            
            PatcherUtil.Patch<OnDeadPatch>();
            PatcherUtil.Patch<OnShellEjectEventPatch>();
            PatcherUtil.Patch<BotStationaryWeaponPatch>();

            PatcherUtil.PatchPrefix<BeaconPatch>();
			PatcherUtil.PatchPostfix<DogtagPatch>();

            PatcherUtil.Patch<LoadOfflineRaidScreenPatch>();
            PatcherUtil.Patch<ScavPrefabLoadPatch>();
            PatcherUtil.Patch<ScavProfileLoadPatch>();
            //PatcherUtil.Patch<ScavSpawnPointPatch>();
            PatcherUtil.Patch<ScavExfilPatch>();

            PatcherUtil.Patch<EndByTimerPatch>();
        }
    }
}
