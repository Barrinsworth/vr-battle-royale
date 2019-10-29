using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Multiplayer
{
    public static class Vector3Util
    {
        public static Vitruvius.Generated.Vector3 ConvertToSpatialOSVector3(UnityEngine.Vector3 vector3)
        {
            return new Vitruvius.Generated.Vector3(vector3.x, vector3.y, vector3.z);
        }

        public static UnityEngine.Vector3 ConvertToUnityVector3(Vitruvius.Generated.Vector3 vector3)
        {
            return new UnityEngine.Vector3(vector3.X, vector3.Y, vector3.Z);
        }
    }
}
