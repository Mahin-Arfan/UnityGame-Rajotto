using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public GameObject screamAudio;
    public int screamTime = 0;
    public float velocity;
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    public AiWeaponScript aiWeaponScript;
    public WeaponIk weaponIk;
    public AiSensor sensor;
    public Animator animator;
    public AiTargetingSystem targeting;
    public Enemy_Damage enemyDamage;
    public AiCoverMovement aiCoverMovement;
    public AiLineOfSightChecker aiLineOfSightChecker;
    public CharacterDeadState deathState;
    public GameObject gameScripts;
    public bool moving = true;
    public bool firing = false;

    public Transform player;
    public PeakingScript peakingScript;
    public Rigidbody rb;
    public LayerMask friendlyFire;
    public Transform meleeRaycast;
    public NavMeshPath navMeshPath;

    private float nextTimeToScream = 0f;
    // Start is called before the first frame update
    void Start()
    {
        gameScripts = GameObject.FindGameObjectWithTag("GameController");
        aiCoverMovement = GetComponent<AiCoverMovement>();
        aiLineOfSightChecker = GetComponentInChildren<AiLineOfSightChecker>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        aiWeaponScript = GetComponentInChildren<AiWeaponScript>();
        meleeRaycast = transform.Find("MeleeRaycast");
        weaponIk = GetComponent<WeaponIk>();
        enemyDamage = GetComponent<Enemy_Damage>();
        sensor = GetComponent<AiSensor>();
        targeting = GetComponent<AiTargetingSystem>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiIdleState());
        stateMachine.RegisterState(new AiShootingState());
        stateMachine.RegisterState(new AiGoingCoverState());
        stateMachine.ChangeState(initialState);
        navMeshPath = new NavMeshPath(); 

        rb = GetComponent<Rigidbody>();
        peakingScript = GetComponent<PeakingScript>();
        deathState = GetComponentInChildren<CharacterDeadState>();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
        StartCoroutine(MovementCheck());

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("CoverFire (1)") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("CoverFire (2)") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Fire") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Walk(Rab)"))
        {
            firing = true;
            weaponIk.enabled = true;
            aiWeaponScript.enabled = true;
        }
        else
        {
            firing = false;
            weaponIk.enabled = false;
            aiWeaponScript.enabled = false;
            aiWeaponScript.muzzleLight.SetActive(false);
        }

        if (enemyDamage.takingHit == true)
        {
            animator.SetLayerWeight(animator.GetLayerIndex("Reloading"), 0);
            animator.SetBool("Hit", true);
            int hitAnimChoice = Random.Range(1, 3);
            if (hitAnimChoice == 1)
            {
                animator.SetBool("Hit1", true);
            }
            else
            {
                animator.SetBool("Hit2", true);
            }
        }
        else
        {
            animator.SetBool("Hit", false);
            animator.SetBool("Hit1", false);
            animator.SetBool("Hit2", false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            navMeshAgent.speed = 3.5f;
            animator.SetFloat("RunAnimSpeed", (velocity / 100f));
        }

        if (Time.time >= nextTimeToScream)
        {
            nextTimeToScream = Time.time + 1f / 0.25f;
            if (screamTime == 1)
            {
                PlayerHealth targetcheck = targeting.Target.transform.GetComponent<PlayerHealth>();
                if (targetcheck != null)
                {
                    GameObject audioEffect = Instantiate(screamAudio, transform.position, Quaternion.identity);
                    Destroy(audioEffect, 1.25f);
                    screamTime = 2;
                }
            }
        }
    }

    IEnumerator MovementCheck()
    {
        Vector3 prevPos = transform.position;
        yield return new WaitForSeconds(0.5f);
        Vector3 actualPos = transform.position;

        velocity = ((prevPos - actualPos).magnitude) / Time.deltaTime;

        if (Vector3.Distance(transform.position, prevPos) - Vector3.Distance(transform.position, actualPos) < .5 &&
            Vector3.Distance(transform.position, prevPos) - Vector3.Distance(transform.position, actualPos) > -.5)
        {
            moving = false;
        }
        else
        {
            moving = true;

        }
    }
}
