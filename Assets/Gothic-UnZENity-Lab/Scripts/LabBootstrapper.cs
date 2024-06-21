using GUZ.Core.Caches;
using GUZ.Core.Context;
using GUZ.Core.Globals;
using GUZ.Core.Manager;
using GUZ.Core.Manager.Settings;
using GUZ.Lab.Handler;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUZ.Lab
{
    public class LabBootstrapper : MonoBehaviour
    {
        public LabMusicHandler labMusicHandler;
        public LabNpcDialogHandler npcDialogHandler;
        public LabLockableHandler lockableHandler;
        public LabLadderLabHandler ladderLabHandler;
        public LabVobSpawnHandler vobSpawnHandler;
        public LabNpcAnimationHandler labNpcAnimationHandler;

        private bool _isBooted;
        /// <summary>
        /// It's easiest to wait for Start() to initialize all the MonoBehaviours first.
        /// </summary>
        private void Update()
        {
            if (_isBooted)
                return;
            _isBooted = true;
            
            GUZBootstrapper.BootGothicUnZENity(SettingsManager.GameSettings.GothicIPath);

            BootLab();

            labNpcAnimationHandler.Bootstrap();
            labMusicHandler.Bootstrap();
            npcDialogHandler.Bootstrap();
            lockableHandler.Bootstrap();
            ladderLabHandler.Bootstrap();
            vobSpawnHandler.Bootstrap();
        }

        private void BootLab()
        {
            var playerGo = GUZContext.InteractionAdapter.CreatePlayerController(SceneManager.GetActiveScene());
            XRDeviceSimulatorManager.I.AddXRDeviceSimulator();
            NpcHelper.CacheHero(playerGo);
        }

        private void OnDestroy()
        {
            GameData.Dispose();
            AssetCache.Dispose();
            TextureCache.Dispose();
            LookupCache.Dispose();
            PrefabCache.Dispose();
            MorphMeshCache.Dispose();
        }
    }
}
