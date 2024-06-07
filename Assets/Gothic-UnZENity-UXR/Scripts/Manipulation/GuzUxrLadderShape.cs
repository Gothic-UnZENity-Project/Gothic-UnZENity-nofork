using UltimateXR.Manipulation;
using UnityEngine;

namespace GUZ.UXR.Manipulation
{
    public class GuzUxrLadderShape : UxrGrabPointShape
    {
        public override float GetDistanceFromGrabber(UxrGrabber grabber, Transform snapTransform, Transform objectDistanceTransform,
            Transform grabberDistanceTransform)
        {
            throw new System.NotImplementedException();
        }

        public override void GetClosestSnap(UxrGrabber grabber, Transform snapTransform, Transform distanceTransform,
            Transform grabberDistanceTransform, out Vector3 position, out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }
    }
}
