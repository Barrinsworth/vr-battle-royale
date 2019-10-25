using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale.Common.Player
{
    public class FOVBlinders : MonoBehaviour
    {
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
        [SerializeField] private Vector2 outterBounds = Vector2.zero;
        [SerializeField] private Vector2 innerBoundsMax = Vector2.zero;
        [SerializeField] private Vector2 innerBoundsMin = Vector2.zero;
        [SerializeField] private RectTransform blinderRectTransform;
        [SerializeField] private RectTransform[] boarderRectTransforms = new RectTransform[0];

        private State currentState = State.Off;
        private float percentOn = 0f;
        private Vector2 stepBounds = Vector2.zero;
        private Coroutine fadeCoroutine;

        #region Unity Lifecycle
        private void Awake()
        {
            stepBounds = (innerBoundsMax - innerBoundsMin) / PlayerSettings.MAX_FOV_BLINDERS_STRENGTH;
        }

        private void OnEnable()
        {
            fadeCoroutine = null;

            PlayerSettingsController.Instance.OnFOVBlindersEnabledChanged += FOVBlindersEnableChanged;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ChangeSize(innerBoundsMax);
        }
#endif

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

                ChangeSize(innerBoundsMax);

                percentOn = 0f;
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

            var timer = FADE_IN_TIME * percentOn;
            var targetSize = innerBoundsMax - (stepBounds * PlayerSettingsController.Instance.FOVBlindersStrength);

            while (timer < FADE_IN_TIME)
            {
                timer += Time.deltaTime;

                percentOn = timer / FADE_IN_TIME;

                ChangeSize(Vector2.Lerp(innerBoundsMax, targetSize, percentOn));

                yield return null;
            }

            ChangeSize(targetSize);

            percentOn = 1f;
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

            var timer = (1f - percentOn) * FADE_OUT_TIME;
            var targetSize = innerBoundsMax - (stepBounds * PlayerSettingsController.Instance.FOVBlindersStrength);

            while (timer < FADE_OUT_TIME)
            {
                timer += Time.deltaTime;

                percentOn = 1f - (timer / FADE_OUT_TIME);

                ChangeSize(Vector2.Lerp(innerBoundsMax, targetSize, percentOn));

                yield return null;
            }

            ChangeSize(innerBoundsMax);

            percentOn = 0f;
            currentState = State.Off;

            fadeCoroutine = null;
        }

        private void ChangeSize(Vector2 size)
        {
            blinderRectTransform.sizeDelta = size;

            var sideSize = new Vector2((outterBounds.x - size.x) * 0.5f, size.y);
            var sideAnchorPosition = new Vector2(((size.x * 0.5f) + (sideSize.x * 0.5f)), 0f);

            boarderRectTransforms[0].sizeDelta = sideSize;
            boarderRectTransforms[0].anchoredPosition = -sideAnchorPosition;

            boarderRectTransforms[1].sizeDelta = sideSize;
            boarderRectTransforms[1].anchoredPosition = sideAnchorPosition;

            var topBottomSize = new Vector2(outterBounds.x, (outterBounds.y - size.y) * 0.5f);
            var topBottomAnchorPosition = new Vector2(0f, ((size.y * 0.5f) + (topBottomSize.y * 0.5f)));

            boarderRectTransforms[2].sizeDelta = topBottomSize;
            boarderRectTransforms[2].anchoredPosition = topBottomAnchorPosition;

            boarderRectTransforms[3].sizeDelta = topBottomSize;
            boarderRectTransforms[3].anchoredPosition = -topBottomAnchorPosition;
        }
    }
}
