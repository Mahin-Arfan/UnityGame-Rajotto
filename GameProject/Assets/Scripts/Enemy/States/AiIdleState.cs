using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdleState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.Idle;
    }

    public void Enter(AiAgent agent)
    {
        agent.aiWeaponScript.enabled = false;
        agent.weaponIk.enabled = false;
        agent.navMeshAgent.stoppingDistance = Random.Range(5, 11);
        if(agent.player == null)
        {
            agent.player = GameObject.Find("Player").transform;
        }
        agent.animator.Play("Run");
    }
    public void Update(AiAgent agent)
    {
        if (agent.moving)
        {
            agent.animator.SetBool("Run", true);
        }
        else
        {
            agent.animator.SetBool("Run", false);
        }
        agent.navMeshAgent.destination = agent.player.position;

        if (agent.targeting.HasTarget)
        {
            float RandomChoice = Random.Range(0, 10);
            if (RandomChoice < 8)
            {
                agent.stateMachine.ChangeState(AiStateId.GoingCover);
            }
            else if (RandomChoice > 7)
            {
                agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            }
        }
    }
    public void Exit(AiAgent agent)
    {

    }
}
