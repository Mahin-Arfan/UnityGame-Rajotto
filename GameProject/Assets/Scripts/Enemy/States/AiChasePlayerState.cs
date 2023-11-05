using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState
{

    float timer = 0.0f;

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 2;
    }
    public void Update(AiAgent agent)
    {
        if (!agent.enabled)
        {
            return;
        }

        agent.animator.SetBool("WalkFire", true);

        if (agent.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Fire"))
        {
            agent.navMeshAgent.speed = 2f;
        }
        else
        {
            agent.navMeshAgent.speed = 3.5f;
        }

        timer -= Time.deltaTime;
        if (agent.targeting.HasTarget)
        {
            agent.navMeshAgent.destination = agent.targeting.TargetPosition;

            if (timer < 0.0f)
            {
                float sqDistance = (agent.targeting.TargetPosition - agent.navMeshAgent.destination).sqrMagnitude;
                if (sqDistance > agent.config.maxDistance * agent.config.maxDistance)
                {
                    agent.navMeshAgent.destination = agent.targeting.TargetPosition;
                }
                timer = agent.config.maxTime;
            }
        }

        if (agent.aiWeaponScript.isReloading == true)
        {

            agent.animator.SetLayerWeight(agent.animator.GetLayerIndex("Reloading"), 1);
            agent.aiWeaponScript.reloadTime = 2.55f;
            agent.animator.SetBool("Reloading2", true);
        }
        else
        {
            agent.animator.SetLayerWeight(agent.animator.GetLayerIndex("Reloading"), 0);
            agent.animator.SetBool("Reloading2", false);
        }


        Vector3 playerDirection = agent.targeting.Target.transform.position - agent.transform.position;
        if (playerDirection.magnitude > agent.config.maxSightDistance / 3)
        {
            agent.stateMachine.ChangeState(AiStateId.GoingCover);
        }

        if (playerDirection.magnitude < agent.config.maxSightDistance / 8)
        {
            if (agent.navMeshAgent.hasPath)
            {
                agent.navMeshAgent.CalculatePath(agent.targeting.TargetPosition, agent.navMeshPath);
                if (agent.navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    agent.stateMachine.ChangeState(AiStateId.Melee);
                }
                else
                {
                    agent.stateMachine.ChangeState(AiStateId.GoingCover);
                }
            }
        }

        if (agent.targeting.Target.GetComponent<CharacterDeadState>().IsDead() || !agent.targeting.HasTarget)
        {
            agent.stateMachine.ChangeState(AiStateId.Idle);
        }
    }
    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.stoppingDistance = 0;
        agent.animator.SetBool("WalkFire", false);
    }
}
