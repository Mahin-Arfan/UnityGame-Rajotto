using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGoingCoverState : AiState
{
    float timer = 0f;
    public AiStateId GetId()
    {
        return AiStateId.GoingCover;
    }

    public void Enter(AiAgent agent)
    {
        agent.aiCoverMovement.enabled = true;
        agent.aiLineOfSightChecker.enabled = true;
    }
    public void Update(AiAgent agent)
    {
        if (agent.targeting.HasTarget)
        {

            if (agent.moving == true)
            {
                agent.animator.SetBool("Run", true);
                agent.animator.SetBool("CoverIdle", false);
                agent.animator.SetBool("coverFire1", false);
                agent.animator.SetBool("coverFire2", false);

                agent.peakingScript.enabled = false;
                agent.navMeshAgent.angularSpeed = 720;

                Vector3 targetDirection = agent.targeting.Target.transform.position - agent.transform.position;
                Vector3 aimDirection = agent.transform.forward;
                float targetAngle = Vector3.Angle(targetDirection, aimDirection);
                if (targetAngle < 90)
                {
                    agent.animator.SetBool("inSight", true);
                }
                else
                {
                    agent.animator.SetBool("inSight", false);
                }


            }
            else
            {
                agent.peakingScript.enabled = true;
                agent.animator.SetBool("CoverIdle", true);

                Vector3 targetDirection = agent.targeting.Target.transform.position - agent.transform.position;
                Vector3 aimDirection = agent.transform.forward;
                float targetAngle = Vector3.Angle(targetDirection, aimDirection);
                if (targetAngle > 45)
                {
                    if (Time.time >= timer)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(agent.targeting.Target.transform.position - agent.transform.position);
                        timer = Time.time + 1f;
                        agent.transform.rotation = Quaternion.Euler(agent.transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, agent.transform.rotation.eulerAngles.z);
                    }
                }

                agent.navMeshAgent.angularSpeed = 0;
            }
        }

        

        if (agent.aiWeaponScript.isReloading == true)
        {
            agent.peakingScript.enabled = false;

            agent.animator.SetBool("coverFire1", false);
            agent.animator.SetBool("coverFire2", false);

            agent.animator.SetLayerWeight(agent.animator.GetLayerIndex("Reloading"), 1);
            if (agent.moving == true)
            {
                agent.aiWeaponScript.reloadTime = 2.55f;
                agent.animator.SetBool("Reloading2", true);
            }
            else
            {
                agent.aiWeaponScript.reloadTime = 6.40f;
                agent.animator.SetBool("Reloading1", true);
            }
        }else
        {
            agent.animator.SetLayerWeight(agent.animator.GetLayerIndex("Reloading"), 0);
            agent.animator.SetBool("Reloading1", false);
            agent.animator.SetBool("Reloading2", false);
        }

        if (agent.targeting.HasTarget)
        {
            Vector3 playerDirection = agent.targeting.Target.transform.position - agent.transform.position;
            if (playerDirection.magnitude < agent.config.maxSightDistance / 5)
            {
                if (agent.navMeshAgent.hasPath)
                {
                    agent.navMeshAgent.CalculatePath(agent.targeting.TargetPosition, agent.navMeshPath);
                    if (agent.navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                    {
                        agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
                    }
                }
            }
        }

        if (!agent.targeting.HasTarget || agent.targeting.Target.GetComponent<CharacterDeadState>().IsDead())
        {
            agent.stateMachine.ChangeState(AiStateId.Idle);
        }
        

    }

    public void Exit(AiAgent agent)
    {
        agent.peakingScript.enabled = false;
        agent.animator.SetBool("coverFire1", false);
        agent.animator.SetBool("coverFire2", false);
        agent.navMeshAgent.angularSpeed = 720;
        agent.aiCoverMovement.enabled = false;
        agent.aiLineOfSightChecker.enabled = false;
    }
}
