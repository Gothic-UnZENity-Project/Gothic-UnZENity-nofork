#if GUZ_HVR_INSTALLED
using System;
using GUZ.Core.Context;
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

        public void AddClimbingComponent(GameObject go)
        {
            go.AddComponent<HVRClimbable>();
            HVRGrabbable grabbable = go.AddComponent<HVRGrabbable>();
            grabbable.PoseType = HurricaneVR.Framework.Shared.PoseType.PhysicPoser;
        }

        public void AddItemComponent(GameObject go, bool isLab = false)
        {
            var colliderComp = go.GetComponent<MeshCollider>();
            colliderComp.convex = true;

            HVRGrabbable grabbable = go.AddComponent<HVRGrabbable>();
            grabbable.PoseType = HurricaneVR.Framework.Shared.PoseType.PhysicPoser;
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.isKinematic = false;

            // FIXME - activate/deactivate culling when dragged around
        }
    }
}
#endif