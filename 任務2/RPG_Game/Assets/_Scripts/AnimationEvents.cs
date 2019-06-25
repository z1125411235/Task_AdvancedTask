using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {

    public delegate void AnimationEvent();
    public static event AnimationEvent OnSlashAnimationHit;
    public static event AnimationEvent OnJumpAnimationJump;
    public event AnimationEvent OnAnimationAttackEvent;

    void SlashAnimationHitEvent()
    {
        OnSlashAnimationHit();
    }
    void JumpAnimationJumpEvent()
    {
        Debug.Log("JumpAnimationJumpEvent called");
        OnJumpAnimationJump();
    }

    void AnimationAttackEvent()
    {
        OnAnimationAttackEvent();
    }
}
