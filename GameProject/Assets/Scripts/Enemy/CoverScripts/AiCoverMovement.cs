using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AiCoverMovement : MonoBehaviour
{
    private AiAgent agent;
    public LayerMask HidableLayers;
    public AiLineOfSightChecker LineOfSightChecker;
    public NavMeshAgent NavAgent;
    [Range(-1, 1)]
    [Tooltip("Lower is a better hiding spot")]
    public float HidaSensitivity = 0;

    [Range(0.01f, 1f)]
    public float UpdateFrequency = 0.50f;

    private Coroutine MovementCoroutine;
    private Collider[] Collider = new Collider[10];

    private void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        agent = GetComponent<AiAgent>();
        LineOfSightChecker.OnGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;
    }
    public void Update() { }

    private void HandleGainSight(Transform Target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        MovementCoroutine = StartCoroutine(Hide(Target));
    }

    private void HandleLoseSight(Transform Target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
    }

    private IEnumerator Hide(Transform Target)
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateFrequency);

        while (true)
        {
            for (int i = 0; i < Collider.Length; i++)
            {
                Collider[i] = null;
            }
            int hits = Physics.OverlapSphereNonAlloc(NavAgent.transform.position, LineOfSightChecker.Collider.radius, Collider, HidableLayers);

            System.Array.Sort(Collider, ColliderArraySortComparer);

            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(Collider[i].transform.position, out NavMeshHit hit, 2f, NavAgent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, NavAgent.areaMask))
                    {
                        Debug.Log($"Unable to find edge close to {hit.position}");
                    }

                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HidaSensitivity)
                    {
                        if (agent.deathState.isDead != true)
                        {
                            NavAgent.SetDestination(hit.position);
                            break;
                        }
                    }
                    else
                    {
                        if (NavMesh.SamplePosition(Collider[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, NavAgent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, NavAgent.areaMask))
                            {
                                Debug.Log($"Unable to find edge close to {hit2.position} (Second Attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HidaSensitivity)
                            {
                                if (agent.deathState.isDead != true)
                                {
                                    NavAgent.SetDestination(hit2.position);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log($"Unable to find NavMesh near object {Collider[i].name} at {Collider[i].transform.position}");
                }
            }
            yield return Wait;
        }
    }

    private int ColliderArraySortComparer(Collider A, Collider B)
    {
        if (A == null && B != null)
        {
            return 1;
        }
        else if (A != null && B == null)
        {
            return -1;
        }
        else if (A == null && B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(NavAgent.transform.position, A.transform.position).CompareTo(Vector3.Distance(NavAgent.transform.position, B.transform.position));
        }
    }
}
