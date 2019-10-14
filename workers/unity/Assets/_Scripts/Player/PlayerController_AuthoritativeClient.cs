using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public class PlayerController_AuthoritativeClient : MonoBehaviour
    {
        private static PlayerController_AuthoritativeClient instance;

        public static PlayerController_AuthoritativeClient Instance { get { return instance; } }

        #region Unity Life Cycle
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
        #endregion
    }
}
