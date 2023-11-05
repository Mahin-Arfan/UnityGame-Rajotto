using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeakingScript : MonoBehaviour
{
    public AiAgent agent;

    public int peakTimeChoice;
    public int peakChoice;
    public int peak2CoverChoice;
    public bool running = false;
    

    void Start()
    {
        agent = GetComponent<AiAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running == true)
        {
            return;
        }
        StartCoroutine(PeakingState());

    }

    IEnumerator PeakingState()
    {
        running = true;
        agent.weaponIk.enabled = false;
        agent.aiWeaponScript.enabled = false;

        peakTimeChoice = Random.Range(1, 7);
        yield return new WaitForSeconds(peakTimeChoice);

        peakChoice = Random.Range(1, 3);
        peak2CoverChoice = Random.Range(1, 3);
        if (peakChoice == 1)
        {
            agent.animator.SetBool("coverFire1", true);
            yield return new WaitForSeconds(0.5f);
            if (agent.deathState.isDead != true)
            {
                agent.weaponIk.enabled = true;
                agent.aiWeaponScript.enabled = true;
            }
            yield return new WaitForSeconds(peak2CoverChoice);
            agent.weaponIk.enabled = false;
            agent.aiWeaponScript.enabled = false;
            agent.animator.SetBool("coverFire1", false);

        }
        else if (peakChoice >= 2)
        {
            agent.animator.SetBool("coverFire2", true);
            yield return new WaitForSeconds(0.5f);
            if (agent.deathState.isDead != true)
            {
                agent.weaponIk.enabled = true;
                agent.aiWeaponScript.enabled = true;
            }
            yield return new WaitForSeconds(peak2CoverChoice);
            agent.weaponIk.enabled = false;
            agent.aiWeaponScript.enabled = false;
            agent.animator.SetBool("coverFire2", false);
        }
        running = false;
    }
}
