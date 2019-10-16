using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public static class QuaternionUtil
    { 
        public static Vitruvius.Generated.Quaternion ConvertToSpatialOSQuaternion(UnityEngine.Quaternion quaternion)
        {
            return new Vitruvius.Generated.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        public static UnityEngine.Quaternion ConvertToUnityQuaternion(Vitruvius.Generated.Quaternion quaternion)
        {
            return new UnityEngine.Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }
    }
}
