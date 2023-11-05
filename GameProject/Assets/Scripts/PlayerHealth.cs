using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    public float maxHealth = 500f;
    public float healthIncreaseRate = 5f;
    private float damageNotTakenTime = 0f;
    private float time = 0f;
    private float increaseTime = 0.5f;

    public Camera playerCam;
    public Vector3 deathCamPos;
    private float deathCamSpeed = 5f;
    private float deathCamFOV = 130f;

    VolumeProfile postProcessing;
    public GameObject deathBlood;
    public GameObject crossHair;

    private float currentLean;
    private float targetLean;
    public float leanAngle;
    private float leanVelocity;
    public Recoil recoil;

    public void Start()
    {
        health = maxHealth;
        damageNotTakenTime = 0f;

        postProcessing = FindObjectOfType<Volume>().profile;

        recoil = GetComponentInChildren<Recoil>();
    }

    public void Update()
    {
        damageNotTakenTime += Time.deltaTime;
        time += Time.deltaTime;

        if (health > 0 && health < maxHealth)
        {
            if(damageNotTakenTime >= 5f)
            {
                if (time >= increaseTime)
                {
                    health += healthIncreaseRate;
                    time = 0;
                    if (increaseTime > 0)
                    {
                        increaseTime -= 0.025f;
                    }
                }
            }
        }


        Vignette vignette;
        if (postProcessing.TryGet(out vignette))
        {
            float percent = 1.0f - (health / maxHealth);
            vignette.intensity.value = percent * 0.5f;
        }

        if (health <= 0f)
        {
            playerCam.transform.localPosition = Vector3.Slerp(playerCam.transform.localPosition, deathCamPos, deathCamSpeed * Time.deltaTime);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, deathCamFOV, deathCamSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        healthIncreaseRate = 5f;
        damageNotTakenTime = 0f;
        increaseTime = 0.5f;
        if (health <= 0f)
        {
            Die();
        }
        else
        {
            recoil.RecoilFire(-25f, 3.5f, 5f, 10f, 40f);
        }
    }

    void Die()
    {
        GetComponent<CharacterDeadState>().Dead();
        FindObjectOfType<GameOver>().EndGame();
        GetComponent<PlayerController>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GameObject.Find("WeaponHolder").SetActive(false);
        playerCam.transform.GetComponent<MouseLook>().enabled = false;
        crossHair.SetActive(false);
        deathBlood.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
