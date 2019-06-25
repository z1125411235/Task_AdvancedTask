using UnityEngine;
using System.Collections;

public class AnimationEvents : MonoBehaviour
{

    public delegate void AnimationEvent();
    public static event AnimationEvent OnSlashAnimationHit;
    public static event AnimationEvent OnJumpAnimationJump;
    public event AnimationEvent OnAnimationAttackEvent;
    public event AnimationEvent OnQuestCompEvent;

    void QuestCompEvent()
    {
        OnQuestCompEvent();
    }

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
