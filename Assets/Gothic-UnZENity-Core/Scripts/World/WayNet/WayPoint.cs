using UnityEngine;

namespace GUZ.Core.World.WayNet
{
    /// <summary>
    /// Gothic handles WayNet in WayPoints (WP) and FreePoints (FP).
    /// Sometimes there are functions which need to check both and requires this superclass as response.
    /// </summary>
    public class WayPoint
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Direction;
        public bool IsLocked;
        public bool IsFree;
    }
}
