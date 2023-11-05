using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    private Animator animator;
    private WeaponScript weaponScript;
    void Start()
    {
        animator = GetComponent<Animator>();
        weaponScript = GetComponent<WeaponScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            animator.SetBool("adsOn", true);
        }
        else
        {
            animator.SetBool("adsOn", false);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_ADS_On"))
        {
            weaponScript.canFire = true;
        }
        else
        {
            weaponScript.canFire = false;
        }
    }
}
