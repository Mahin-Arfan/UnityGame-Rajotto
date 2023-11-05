using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimInSightRAB : MonoBehaviour
{
    private AiAgent agent;
    private Animator animator;
    private WeaponIk weaponIk;

    void Start()
    {
        agent = GetComponent<AiAgent>();
        animator = GetComponent<Animator>();
        weaponIk = GetComponent<WeaponIk>();
    }

    void Update()
    {
        if (!agent.targeting.HasTarget)
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run") || animator.GetCurrentAnimatorStateInfo(0).IsName("Walk(Rab)"))
        {
            if(weaponIk.targetAngle < weaponIk.angleLimit)
            {
                Debug.LogWarning("True");
                animator.SetBool("inSight", true);
            }
            else
            {
                Debug.LogWarning("False");
                animator.SetBool("inSight", false);
            }
        }
    }
}
