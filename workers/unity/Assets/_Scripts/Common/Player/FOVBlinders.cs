using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public class FOVBlinders : MonoBehaviour
    {
        private const float MAX_OFFSET = 600;
        private const float FADE_IN_TIME = 0.2f;
        private const float FADE_OUT_TIME = 0.1f;

        private enum State
        {
            FadingOn,
            On,
            FadingOff,
            Off
        }

        [Header("--FOV Blinders--")]
        [SerializeField] private RectTransform scaleRectTransform;

        private State currentState = State.Off;
        private Coroutine fadeCoroutine;

        #region Unity Lifecycle
        private void OnEnable()
        {
            fadeCoroutine = null;

            PlayerSettingsController.Instance.OnFOVBlindersEnabledChanged += FOVBlindersEnableChanged;

            FOVBlindersEnableChanged();
        }

        private void OnDisable()
        {
            if (PlayerSettingsController.Instance)
            {
                PlayerSettingsController.Instance.OnFOVBlindersEnabledChanged -= FOVBlindersEnableChanged;
            }
        }
        #endregion

        #region Event Listeners
        private void FOVBlindersEnableChanged()
        {
            if (!PlayerSettingsController.Instance.FOVBlindersEnabled)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;

                ChangeSize(MAX_OFFSET);

                currentState = State.Off;
            }
        }
        #endregion

        public void FadeBlindersIn()
        {
            if (!PlayerSettingsController.Instance.FOVBlindersEnabled || currentState == State.FadingOn || currentState == State.On)
                return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOnCoroutine());
        }

        private IEnumerator FadeOnCoroutine()
        {
            currentState = State.FadingOn;

            var timer = 0f;
            var startOffset = scaleRectTransform.offsetMax.x;

            while (timer < FADE_IN_TIME)
            {
                timer += Time.deltaTime;

                ChangeSize(Mathf.Lerp(startOffset, 300 - (PlayerSettingsController.Instance.FOVBlindersStrength * 10), timer / FADE_IN_TIME));

                yield return null;
            }

            ChangeSize(300 - (PlayerSettingsController.Instance.FOVBlindersStrength * 10));

            currentState = State.On;

            fadeCoroutine = null;
        }

        public void FadeBlindersOut()
        {
            if (currentState == State.FadingOff || currentState == State.Off)
                return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOffCoroutine());
        }

        private IEnumerator FadeOffCoroutine()
        {
            currentState = State.FadingOff;

            var timer = 0f;
            var startOffset = scaleRectTransform.offsetMax.x;

            while (timer < FADE_OUT_TIME)
            {
                timer += Time.deltaTime;

                ChangeSize(Mathf.Lerp(startOffset, MAX_OFFSET, timer / FADE_OUT_TIME));

                yield return null;
            }

            ChangeSize(MAX_OFFSET);

            currentState = State.Off;

            fadeCoroutine = null;
        }

        private void ChangeSize(float offset)
        {
            scaleRectTransform.offsetMin = new Vector2(-offset, -offset);
            scaleRectTransform.offsetMax = new Vector2(offset, offset);
        }
    }
}
