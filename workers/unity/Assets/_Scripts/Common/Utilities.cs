using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common
{
    public static class Utilities
    {
        public static bool IsDevelopment { get { return Application.isEditor || Debug.isDebugBuild; } }
    }
}
