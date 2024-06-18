using GUZ.Core.Caches;
using GUZ.Core.Context;
using GUZ.Core.Manager;
using GUZ.Core.Manager.Culling;
using GUZ.Core.Manager.Settings;
using GUZ.Core.World;
using GUZ.Core;
using GUZ.Core.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUZ.Core
{
	public class GameManager : MonoBehaviour, CoroutineManager
	{
		public GameConfiguration config;
		public GameObject xrInteractionManager;
		public GameObject invalidInstallationPathMessage;

		private SettingsManager _gameSettingsManager;
		private VobMeshCullingManager _meshCullingManager;
		private VobSoundCullingManager _soundCullingManager;
		private BarrierManager _barrierManager;
		private SkyManager _skyVisualManager;
		private StationaryLightsManager _stationaryLightsManager;
		private XRDeviceSimulatorManager _xrSimulatorManager;
		private GameTime _gameTimeManager;
		private MusicManager _gameMusicManager;
		private GUZSceneManager _gameSceneManager;
		private LoadingManager _gameLoadingManager;
		private RoutineManager _npcRoutineManager;

		private bool _isInitialised = false;

		// ReSharper disable Unity.PerformanceAnalysis
		private void Load()
		{
			// If the Gothic installation directory is not set, show an error message and exit.
			if (!_gameSettingsManager.CheckIfGothic1InstallationExists())
			{
				invalidInstallationPathMessage.SetActive(true);
				return;
			}

			// Otherwise, continue loading Gothic.
            ResourceLoader.Init(SettingsManager.GameSettings.GothicIPath);
	
            _gameMusicManager.Init();
            
			GUZBootstrapper.BootGothicUnZENity(SettingsManager.GameSettings.GothicIPath);
			_gameSceneManager.LoadStartupScenes();

			if (config.enableBarrierVisual)
			{
				_barrierManager.CreateBarrier();
			}
		}

		private void Awake()
		{
			LookupCache.Init();

			_gameLoadingManager = new LoadingManager();
			_gameSettingsManager = new SettingsManager();
			_meshCullingManager = new VobMeshCullingManager(config, this);
			_soundCullingManager = new VobSoundCullingManager(config);
			_barrierManager = new BarrierManager(config);
			_stationaryLightsManager = new StationaryLightsManager();
			_xrSimulatorManager = new XRDeviceSimulatorManager(config);
			_gameTimeManager = new GameTime(config, this);
			_skyVisualManager = new SkyManager(config, _gameTimeManager);
			_gameMusicManager = new MusicManager(config);
			_gameSceneManager = new GUZSceneManager(config, _gameLoadingManager, xrInteractionManager);
			_npcRoutineManager = new RoutineManager(config);
		}

		private void Start()
		{
			ZenKit.Logger.Set(config.zenkitLogLevel, Logging.OnZenKitLogMessage);
			DirectMusic.Logger.Set(config.directMusicLogLevel, Logging.OnDirectMusicLogMessage);

			_gameLoadingManager.Init();
			_gameSettingsManager.Init();
			_meshCullingManager.Init();
			_soundCullingManager.Init();
			_gameTimeManager.Init();
			_skyVisualManager.Init();
			_xrSimulatorManager.Init();
			_gameSceneManager.Init();
			_npcRoutineManager.Init();

			// Just in case we forgot to disable it in scene view. ;-)
			invalidInstallationPathMessage.SetActive(false);
			
			// Load the player controller upon MainMenu loaded
            GlobalEventDispatcher.MainMenuSceneLoaded.AddListener(delegate
            {
                GUZContext.InteractionAdapter.CreatePlayerController(SceneManager.GetActiveScene());
            });
		}

		private void Update()
		{
			_gameSceneManager.Update();

			if (_isInitialised)
			{
				return;
			}

			_isInitialised = true;
			Load();
		}

		private void FixedUpdate()
		{
			_barrierManager.FixedUpdate();
		}

		private void LateUpdate()
		{
			_stationaryLightsManager.LateUpdate();
		}

		private void OnValidate()
		{
			_skyVisualManager?.OnValidate();
		}

		private void OnDrawGizmos()
		{
			_meshCullingManager?.OnDrawGizmos();
		}

		public void OnDestroy()
		{
			_meshCullingManager.Destroy();
			_soundCullingManager.Destroy();

			_gameLoadingManager = null;
			_gameSettingsManager = null;
			_meshCullingManager = null;
			_soundCullingManager = null;
			_barrierManager = null;
			_stationaryLightsManager = null;
			_xrSimulatorManager = null;
			_gameTimeManager = null;
			_skyVisualManager = null;
			_gameMusicManager = null;
			_gameSceneManager = null;
			_npcRoutineManager = null;
		}

		private void OnApplicationQuit()
		{
			GUZBootstrapper.OnApplicationQuit();
		}
	}
}