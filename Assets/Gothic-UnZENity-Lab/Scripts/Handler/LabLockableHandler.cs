using GUZ.Core.Caches;
using GUZ.Core.Creator.Meshes.V2;
using UnityEngine;

namespace GUZ.Lab.Handler
{
    public class LabLockableHandler : MonoBehaviour, ILabHandler
    {
        public GameObject chestsGo;
        public GameObject doorsGo;

        public void Bootstrap()
        {
            var prefab = PrefabCache.TryGetObject(PrefabCache.PrefabType.VobContainer);
            var chestName = "CHESTBIG_OCCHESTLARGELOCKED.MDS";
            var mdh = AssetCache.TryGetMdh(chestName);
            var mdm = AssetCache.TryGetMdm(chestName);

            MeshFactory.CreateVob(chestName, mdm, mdh, Vector3.zero, Quaternion.identity, chestsGo, rootGo: prefab, useTextureArray: false);


            var doorName = "DOOR_WOODEN";
            var mdlDoor = AssetCache.TryGetMdl(doorName);

            MeshFactory.CreateVob(doorName, mdlDoor, Vector3.zero, Quaternion.identity, doorsGo, useTextureArray: false);
        }
    }
}
