using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public static class Vector2Util
    {
        public static Vitruvius.Generated.Vector2 ConvertToSpatialOSVector2(UnityEngine.Vector2 Vector2)
        {
            return new Vitruvius.Generated.Vector2(Vector2.x, Vector2.y);
        }

        public static UnityEngine.Vector2 ConvertToUnityVector2(Vitruvius.Generated.Vector2 Vector2)
        {
            return new UnityEngine.Vector2(Vector2.X, Vector2.Y);
        }
    }
}
