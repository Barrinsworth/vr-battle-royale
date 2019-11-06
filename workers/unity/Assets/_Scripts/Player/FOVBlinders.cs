using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public class FOVBlinders : MonoBehaviour
    {
        [SerializeField] private float fadeInTime = 0.2f;
        [SerializeField] private float fadeOutTime = 0.1f;
        [SerializeField] private Vector2 outterBounds = Vector2.zero;
        [SerializeField] private Vector2 innerBoundsMax = Vector2.zero;
        [SerializeField] private Vector2 innerBoundsMin = Vector2.zero;
        [SerializeField] private RectTransform blinderRectTransform;
        [SerializeField] private RectTransform[] boarderRectTransforms = new RectTransform[0];

        private FaderStateEnum currentState = FaderStateEnum.Off;
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

            PlayerSettingsManager.Instance.OnFOVBlindersEnabledChanged += FOVBlindersEnableChanged;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ChangeSize(innerBoundsMax);
        }
#endif

        private void OnDisable()
        {
            if (PlayerSettingsManager.Instance)
            {
                PlayerSettingsManager.Instance.OnFOVBlindersEnabledChanged -= FOVBlindersEnableChanged;
            }
        }
        #endregion

        #region Event Listeners
        private void FOVBlindersEnableChanged()
        {
            if (!PlayerSettingsManager.Instance.FOVBlindersEnabled)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;

                ChangeSize(innerBoundsMax);

                percentOn = 0f;
                currentState = FaderStateEnum.Off;
            }
        }
        #endregion

        public void FadeBlindersIn()
        {
            if (!PlayerSettingsManager.Instance.FOVBlindersEnabled || currentState == FaderStateEnum.FadingOn || currentState == FaderStateEnum.On)
                return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOnCoroutine());
        }

        private IEnumerator FadeOnCoroutine()
        {
            currentState = FaderStateEnum.FadingOn;

            var timer = fadeInTime * percentOn;
            var targetSize = innerBoundsMax - (stepBounds * PlayerSettingsManager.Instance.FOVBlindersStrength);

            while (timer < fadeInTime)
            {
                timer += Time.deltaTime;

                percentOn = timer / fadeInTime;

                ChangeSize(Vector2.Lerp(innerBoundsMax, targetSize, percentOn));

                yield return null;
            }

            ChangeSize(targetSize);

            percentOn = 1f;
            currentState = FaderStateEnum.On;

            fadeCoroutine = null;
        }

        public void FadeBlindersOut()
        {
            if (currentState == FaderStateEnum.FadingOff || currentState == FaderStateEnum.Off)
                return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOffCoroutine());
        }

        private IEnumerator FadeOffCoroutine()
        {
            currentState = FaderStateEnum.FadingOff;

            var timer = (1f - percentOn) * fadeOutTime;
            var targetSize = innerBoundsMax - (stepBounds * PlayerSettingsManager.Instance.FOVBlindersStrength);

            while (timer < fadeOutTime)
            {
                timer += Time.deltaTime;

                percentOn = 1f - (timer / fadeOutTime);

                ChangeSize(Vector2.Lerp(innerBoundsMax, targetSize, percentOn));

                yield return null;
            }

            ChangeSize(innerBoundsMax);

            percentOn = 0f;
            currentState = FaderStateEnum.Off;

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
