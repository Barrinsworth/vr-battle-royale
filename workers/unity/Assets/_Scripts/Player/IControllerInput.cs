using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRBattleRoyale
{
    public interface IControllerInput
    {
        bool ButtonStartDown { get; }
        bool ButtonOneDown { get; }
        bool ButtonTwoDown { get; }
        bool TriggerPrimaryDown { get; }
        bool TriggerPrimaryUp { get; }
        bool TriggerSecondaryDown { get; }
        bool TriggerSecondaryUp { get; }
        bool TriggerInteractionDown { get; }
        bool TriggerInteractionUp { get; }
        float TriggerPrimaryPressure { get; }
        float TriggerSecondaryPressure { get; }
        float TriggerInteractionPressure { get; }
        float ThumbPressure { get; }
        float IndexFingerPressure { get; }
        float MiddleFingerPressure { get; }
        float RingFingerPressure { get; }
        float PinkyPressure { get; }
        Vector2 ThumbstickPrimary { get; }
    }
}
