using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack02 : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Sword>().SwordColliderOff();
    }
}
