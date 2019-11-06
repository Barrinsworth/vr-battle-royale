using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        public static InputManager Instance { get { return instance; } }

        private Input input;

        #region Unity Life Cycle
        private void Awake()
        {
            if (instance != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(gameObject);
#else
                Destroy(gameObject);
#endif
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);

            input = new Input();
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                input.Disable();
                input.Dispose();

                instance = null;
            }
        }
        #endregion
    }
}
