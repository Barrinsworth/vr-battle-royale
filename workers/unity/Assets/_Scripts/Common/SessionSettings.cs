using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common
{
    [CreateAssetMenu(fileName = "New Session Settings", menuName = "ScriptableObjects/Common/Session Settings")]
    public class SessionSettings : ScriptableObject
    {
        public HMDTypeEnum HMDType = HMDTypeEnum.OculusQuest;
        public PlatformEnum Platform = PlatformEnum.OculusHome;
    }
}
