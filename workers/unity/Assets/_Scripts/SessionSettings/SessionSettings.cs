using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    [CreateAssetMenu(fileName = "New Session Settings", menuName = "ScriptableObjects/Session Settings")]
    public class SessionSettings : ScriptableObject
    {
        public HMDTypeEnum HMDType = HMDTypeEnum.OculusQuest;
        public PlatformEnum Platform = PlatformEnum.OculusHome;
    }
}
