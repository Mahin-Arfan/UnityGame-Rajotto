using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage : MonoBehaviour
{
    public float mainHealth = 100f;
    public GameObject gunBone;
    private AiAgent aiAgent;

    public bool takingHit = false;


    // Start is called before the first frame update
    void Start()
    {
        setRigidBodyState(true);
        aiAgent = GetComponent<AiAgent>();
        gunBone = aiAgent.aiWeaponScript.enemyWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if (takingHit == true)
        {
            StartCoroutine(TakingHit());
        }
    }

    IEnumerator TakingHit()
    {
        if(mainHealth > 0)
        {
            GetComponent<AudioSource>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            GetComponent<AudioSource>().enabled = false;
            takingHit = false;
        }
    }

    public void Die()
    {
        setRigidBodyState(false);
        setColliderState(true);

        GetComponentInChildren<CharacterDeadState>().Dead();
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<PeakingScript>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Animator>().enabled = false;
        aiAgent.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<WeaponIk>().enabled = false;
        GetComponent<AiSensor>().enabled = false;
        GetComponent<AiTargetingSystem>().enabled = false;
        GetComponent<AiCoverMovement>().enabled = false;
        GetComponentInChildren<AiLineOfSightChecker>().enabled = false;
        aiAgent.aiWeaponScript.muzzleLight.SetActive(false);
        aiAgent.aiWeaponScript.enabled = false;


        gunBone.transform.parent = null;
        Destroy(gameObject, 12f);
    }

    void setRigidBodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
    }

    void setColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<BoxCollider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
    }
}
