using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRBattleRoyale.Common;

namespace VRBattleRoyale.SinglePlayer
{
    public class PlayerMotor : MonoBehaviour
    {
        [Header("--Player Motor--")]
        [SerializeField] private CharacterController characterController;

        #region Unity Life Cycle
        private void Awake()
        {
            characterController.transform.parent = null;
        }

        private void Update()
        {

        }
        #endregion
    }
}
