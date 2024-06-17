using System.Collections.Generic;
using System.IO;
using GUZ.Core.Globals;
using GUZ.Core.Manager.Settings;
using GUZ.Core.World;
using ZenKit;

namespace GUZ.Core.Manager
{
    /// <summary>
    /// Usage:
    ///
    /// Loading:
    /// 1. LoadNewGame() | LoadSavedGame()  -> Initializes the save game state
    /// 2. ChangeWorld(worldName:str)       -> Load the required world. Will be fetched from save or from game data itself.
    ///
    /// Saving:
    /// 1. + 2. Load*() and ChangeWorld()   -> Needs to be called before to fill the data.
    /// 3. SaveGame(saveGameId:int)         -> Will use the currently loaded world from runtime and stores changes.
    ///
    /// Helper methods:
    /// * GetSaveGame(saveGameId:int)       -> Return a save game object (or null) if requested. (e.g. used for LoadMenu to prepare data.
    /// </summary>
    public static class SaveGameManager
    {
        public static int SaveGameId;
        public static bool IsNewGame => SaveGameId <= 0;
        public static bool IsLoadedGame => !IsNewGame;
        private static SaveGame _save;

        private static readonly Dictionary<string, (ZenKit.World zkWorld, WorldData uWorld)> _worlds = new();
        public static string CurrentWorldName;
        public static ZenKit.World CurrentZkWorld => _worlds[CurrentWorldName].zkWorld;
        public static WorldData CurrentWorldData => _worlds[CurrentWorldName].uWorld;


        public static void LoadNewGame()
        {
            SaveGameId = 0;
            _save = new SaveGame(GameVersion.Gothic1);
        }

        /// <summary>
        /// Hint: G1 save game folders start with 1. We leverage the same numbering.
        /// </summary>
        public static void LoadSavedGame(int saveGameId)
        {
            LoadSavedGame(saveGameId, GetSaveGame(saveGameId));
        }

        public static void LoadSavedGame(int saveGameId, SaveGame save)
        {
            SaveGameId = saveGameId;
            _save = save;
        }

        public static void ChangeWorld(string worldName)
        {
            CurrentWorldName = worldName;

            // World was already loaded.
            if (_worlds.TryGetValue(worldName, out var w))
            {
                return;
            }

            ZenKit.World world = new ZenKit.World(GameData.Vfs, worldName);
            ZenKit.World saveGameWorld = null;

            // We can't ask a SaveGame for a world if we didn't save it before. Bug?
            if (IsLoadedGame)
            {
                // Try to load world from save game.
                saveGameWorld = _save.LoadWorld(worldName);
            }

            // Store this world into runtime data as it's now loaded and cached during gameplay. (To save later when needed.)
            _worlds[worldName] = new()
            {
                zkWorld = world,
                uWorld = new WorldData()
                {
                    // Not contained inside saveGame
                    Mesh = (CachedMesh)world.Mesh.Cache(),
                    BspTree = (CachedBspTree)world.BspTree.Cache(),
                    // Contained inside normal .zen file and also saveGame.
                    Vobs = saveGameWorld == null ? world.RootObjects : saveGameWorld.RootObjects,
                    WayNet = (CachedWayNet)(saveGameWorld == null ? world.WayNet.Cache() : saveGameWorld.WayNet.Cache())
                }
            };
        }

        public static SaveGame GetSaveGame(int folderSaveId)
        {
            // Load metadata
            var save = new SaveGame(GameVersion.Gothic1);
            save.Load(GetSaveGamePath(folderSaveId));

            return save;
        }

        /// <summary>
        /// Saving means:
        /// 1. Collect changed data from all the worlds visited during this gameplay
        /// 2. Alter its values inside the ZenKit data
        /// 3. Save world-by-world into the save game itself
        /// </summary>
        public static void SaveGame(int saveGameId)
        {
            foreach (var world in _worlds)
            {
                PrepareWorldDataForSaving(world.Value.zkWorld, world.Value.uWorld);
                _save.Save(GetSaveGamePath(saveGameId), world.Value.zkWorld, world.Key);
            }
        }

        /// <summary>
        /// We write data from Unity data back into ZenKit data.
        /// Hint: Not all elements need to be replaced and therefore have no setter (e.g. .Mesh, .WayNet).
        /// We therefore only set what's needed.
        /// </summary>
        private static void PrepareWorldDataForSaving(ZenKit.World zkWorld, WorldData uWorld)
        {
            zkWorld.RootObjects = uWorld.Vobs;
        }

        private static string GetSaveGamePath(int folderSaveId)
        {
            var g1Dir = SettingsManager.GameSettings.GothicIPath;
            return Path.GetFullPath(Path.Join(g1Dir, $"Saves/savegame{folderSaveId}"));
        }
    }
}
