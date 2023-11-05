using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiShootingState : AiState
{
    private float nextTimeToMelee = 0f;

    public AiStateId GetId()
    {
        return AiStateId.Melee;
    }

    public void Enter(AiAgent agent)
    {
        agent.screamTime = 1;
    }
    public void Update(AiAgent agent)
    {
        if (!agent.enabled || !agent.targeting.HasTarget)
        {
            return;
        }

        agent.animator.SetLayerWeight(agent.animator.GetLayerIndex("Reloading"), 0);

        agent.navMeshAgent.stoppingDistance = 1.2f;

        Vector3 playerDirection = agent.targeting.TargetPosition - agent.transform.position;
        if (playerDirection.magnitude < agent.config.maxSightDistance / 9.6f)
        {
            agent.animator.SetBool("Melee", true);
        }
        else
        {
            agent.animator.SetBool("Melee", false);
            agent.animator.SetBool("Run", true);
            agent.animator.SetBool("CoverIdle", false);
            agent.navMeshAgent.speed = 3.5f;
        }
        if (agent.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Fire"))
        {
            agent.navMeshAgent.speed = 2f;
        }

        if (agent.animator.GetCurrentAnimatorStateInfo(0).IsName("Melee"))
        {
            agent.animator.SetBool("Melee", false);
            agent.navMeshAgent.speed = 0f;

            // Look at including x and z leaning
            agent.transform.LookAt(agent.targeting.TargetPosition);

            // Euler angles are easier to deal with. You could use Quaternions here also
            // C# requires you to set the entire rotation variable. You can't set the individual x and z (UnityScript can), so you make a temp Vec3 and set it back
            Vector3 eulerAngles = agent.transform.rotation.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;

            // Set the altered rotation back
            agent.transform.rotation = Quaternion.Euler(eulerAngles);

            if (Time.time >= nextTimeToMelee)
            {

                nextTimeToMelee = Time.time + 1f / 1;

                RaycastHit hit;
                
                Vector3 aimDirection = agent.meleeRaycast.transform.forward;

                if (Physics.Raycast(agent.meleeRaycast.transform.position, aimDirection, out hit, 0.8f, ~agent.friendlyFire))
                {
                    PlayerHealth target = hit.transform.GetComponent<PlayerHealth>();
                    if (target != null)
                    {
                        target.TakeDamage(100);
                    }
                    Health enemyTarget = hit.transform.GetComponent<Health>();
                    if (enemyTarget != null)
                    {
                        enemyTarget.TakeDamage(100);
                    }
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * 500);
                    }
                }
            }
        }

        if (!agent.navMeshAgent.hasPath && agent.navMeshAgent.enabled == true)
        {
            agent.navMeshAgent.destination = agent.targeting.TargetPosition;
        }

        if (playerDirection.magnitude > agent.config.maxSightDistance / 9.6f)
        {
            if (agent.navMeshAgent.hasPath)
            {
                agent.navMeshAgent.CalculatePath(agent.targeting.TargetPosition, agent.navMeshPath);
                if (agent.navMeshPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                {
                    agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
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
        agent.animator.SetBool("Melee", false);
    }
}
