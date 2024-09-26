using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationConstants
{

    public const float SCALEBOUNCE_DEFAULT_DURATION = 0.2f;

    public static float SLIDE_GAMESETUP_DEFAULT_DURATION = 0.5f;

    public static float DROP_ANIMATION_DURATION = 0.5f;

    public static float DROP_ANIMATION_OVERSHOOT_AMOUNT = 0.2f;

    public const float UNIT_FORM_ELASTICITIY_ANIMATION_DURATION = 0.3f;

    public const float UNIT_FORM_FORWARD_ANIMATION_DURATION = 0.25f;
    /// <summary>
    /// Unit elasticity before forming to the destination unit.
    /// To go right, go left slightly and smash to the right.
    /// </summary>
    public const float UNIT_FORM_ELASTICITY_OFFSET = 1.5f;
}
