using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SCR_RobotAnimEvents : MonoBehaviour
{
    [Tooltip("The controlling robot character.")]
    public SCR_RobotCharacter owningRobot;
    [Tooltip("The animator of this robot character.")]
    public Animator robotAnimator;

    public void HitReactEnd()
    {
        owningRobot.OnHitReactEnd();
    }

    public void PunchStart()
    {
        owningRobot.OnPunchStart();
    }

    public void PunchEnd()
    {
        owningRobot.OnPunchEnd();
    }

    public void RocketPunchStart()
    {
        owningRobot.OnRocketPunchStart();
    }

    public void RocketPunchEnd()
    {
        owningRobot.OnRocketPunchEnd();
    }

    public void JumpStart()
    {
        owningRobot.OnJumpStart();
    }
}
