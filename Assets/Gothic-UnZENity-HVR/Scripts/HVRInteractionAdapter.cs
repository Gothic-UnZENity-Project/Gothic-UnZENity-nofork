#if GUZ_HVR_INSTALLED
using System.Collections;
using GUZ.Core.Context;
using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Shared;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GUZ.HVR
{
    public class HVRInteractionAdapter : IInteractionAdapter
    {
        private const string CONTEXT_NAME = "HVR";

        public string GetContextName()
        {
            return CONTEXT_NAME;
        }

        public GameObject CreatePlayerController(Scene scene)
        {
            var newPrefab = Resources.Load<GameObject>("HVR/Prefabs/VRPlayer");
            var go = Object.Instantiate(newPrefab);
            go.name = "VRPlayer - HVR";

            // During normal gameplay, we need to move the VRPlayer to General scene. Otherwise, it will be created inside
            // world scene and removed whenever we change the world.
            SceneManager.MoveGameObjectToScene(go, scene);

            return go;
        }

        public void SpawnPlayerToSpot(GameObject playerGo, Vector3 position, Quaternion rotation)
        {
            // We highjack another Component to call StartCoroutine().
            playerGo.GetComponentInChildren<HVRTeleporter>().StartCoroutine(Wait1FrameBeforeTeleport(playerGo, position, rotation));
        }

        /// <summary>
        /// We need to wait 1 frame (after the Player Prefab is initialized) for HVR to recognize our position change.
        /// </summary>
        private IEnumerator Wait1FrameBeforeTeleport(GameObject playerGo, Vector3 position, Quaternion rotation)
        {
            yield return new WaitForNextFrameUnit();

            playerGo.GetComponentInChildren<HVRTeleporter>().Teleport(position, rotation * Vector3.forward);
        }

        public void AddClimbingComponent(GameObject go)
        {
            go.AddComponent<HVRClimbable>();
            HVRGrabbable grabbable = go.AddComponent<HVRGrabbable>();
            grabbable.PoseType = PoseType.PhysicPoser;
        }

        public void AddItemComponent(GameObject go, bool isLab = false)
        {
            // TODO - Hack for Lab.scene - Better way to get something thrown. Needs to be removed soon.
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            // FIXME - activate/deactivate culling when dragged around
        }
    }
}
#endif
