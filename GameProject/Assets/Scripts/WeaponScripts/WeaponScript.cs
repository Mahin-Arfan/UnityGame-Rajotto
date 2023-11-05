using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using TMPro;

public class WeaponScript : MonoBehaviour
{
    [Header("Bullet Impact")]
    public bool canFire = true;
    public bool isFiring = false;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float impactForce = 250f;
    [SerializeField] private LayerMask bulletIgnore;
    [SerializeField] private float bulletSpread = 0.05f;

    private float nextTimeToFire = 0f;
    private float nextTimeToMelee = 0f;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int currentAmmo;

    [Header("Reload")]
    public bool isReloading = false;
    [SerializeField] private bool isMeleeing = false;
    [SerializeField] private bool meleedWhileReload = false;
    [SerializeField] private float reloadTime = 1f;
    public bool adsOn;

    [Header("Recoil")]
    //Hipfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //Aimfire Recoil
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    //Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    [Header("Weapon Close")]
    [SerializeField] private GameObject gunRotate;
    [SerializeField] private float weaponLenght = 0.65f;
    [SerializeField] private float weaponRotate = 75f;
    [SerializeField] private float weaponPositionx = 0.025f;
    [SerializeField] private float weaponPositiony = 0f;
    [SerializeField] private float weaponPositionz = -0.05f;
    private float percentage;
    public bool weaponClose = false;


    [Header("References")]
    public bool canChangeWeapon = false;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private Recoil recoilScript;

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject muzzlePrefeb;
    [SerializeField] private LineRenderer bulletTrail;
    [SerializeField] private GameObject gunAudio;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private GameObject impactEffectBlood;
    [SerializeField] private GameObject impactEffectWOHole;
    [SerializeField] private GameObject currentAmmoText;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentAmmo = maxAmmo;
        isReloading = false;
        isMeleeing = false;
        animator.SetBool("Reloading", false);
        animator.SetBool("Reloading2", false);
        animator.SetBool("Melee", false);
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmoText.GetComponent<TextMeshPro>().text = currentAmmo.ToString();
        if(gunRotate != null)
        {
            WeaponClose();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Melee"))
        {
            isMeleeing = true;
            canChangeWeapon = false;
        }
        else
        {
            isMeleeing = false;
            canChangeWeapon = true;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Weapon_On") || animator.GetCurrentAnimatorStateInfo(0).IsName("Weapon_Off"))
        {
            canChangeWeapon = false;
            isReloading = false;
        }
        else
        {
            canChangeWeapon = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Reload1") || animator.GetCurrentAnimatorStateInfo(0).IsName("Reload2"))
        {
            animator.SetBool("Reloading", false);
            animator.SetBool("Reloading2", false);
        }

        if (isMeleeing)
        {
            return;
        }

        if (Input.GetKey(KeyCode.V) && Time.time >= nextTimeToMelee && !Input.GetKey(KeyCode.LeftShift))
        {
            nextTimeToMelee = Time.time + 1f / 20f;
            if(isReloading == true)
            {
                meleedWhileReload = true;
            }
            Melee();
        }

        if (isReloading)
        {
            return;
        }
        
        if (currentAmmo <= 0 || Input.GetKey(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (animator.GetBool("adsOn") && canFire)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && !Input.GetKey(KeyCode.LeftShift) && !weaponClose)
            {
                canChangeWeapon = false;
                nextTimeToFire = Time.time + 1f / fireRate;
                if(currentAmmo == 1)
                {
                    animator.SetBool("isFiringEmpty", true);
                }
                else
                {
                    int animChoice = Random.Range(1, 3);
                    if (animChoice == 1)
                    {
                        animator.SetBool("isFiring", true);
                        if (adsOn == true)
                        {
                            recoilScript.RecoilFire(aimRecoilX, aimRecoilY, aimRecoilZ, returnSpeed, snappiness);
                        }
                        else
                        {
                            recoilScript.RecoilFire(recoilX, recoilY, recoilZ, returnSpeed, snappiness);
                        }

                    }
                    else
                    {
                        animator.SetBool("isFiring2", true);
                        if (adsOn == true)
                        {
                            recoilScript.RecoilFire(aimRecoilX, aimRecoilY, aimRecoilZ, returnSpeed, snappiness);
                        }
                        else
                        {
                            recoilScript.RecoilFire(recoilX, recoilY, recoilZ, returnSpeed, snappiness);
                        }
                    }
                }
                Shoot();
            }
        }
        if(Input.GetButtonUp("Fire1") || Input.GetKey(KeyCode.LeftShift) || weaponClose)
        {
            animator.SetBool("isFiring", false);
            animator.SetBool("isFiring2", false);
            animator.SetBool("isFiringEmpty", false);
            isFiring = false;
            canChangeWeapon = true;
        }
    }

    IEnumerator Reload()
    {
        isFiring = false;
        animator.SetBool("isFiring", false);
        animator.SetBool("isFiring2", false);
        animator.SetBool("isFiringEmpty", false);
        isReloading = true;
        Debug.Log("Reloading...");
        int animChoice = Random.Range(1, 3);
        if(animChoice == 1)
        {
            animator.SetBool("Reloading", true);
        }
        else
        {
            animator.SetBool("Reloading2", true);
        }

        yield return new WaitForSeconds(reloadTime - .025f);

        isReloading = false;
        animator.SetBool("Reloading", false);
        animator.SetBool("Reloading2", false);

        if (meleedWhileReload == false)
        {
            currentAmmo = maxAmmo;
        }
        else { meleedWhileReload = false; }
    }

    void Melee()
    {
        animator.Play("Melee");
        isFiring = false;
        animator.SetBool("isFiring", false);
        animator.SetBool("isFiring2", false);
        animator.SetBool("isFiringEmpty", false);
        animator.SetBool("Reloading", false);
        animator.SetBool("Reloading2", false);
        Vector3 aimDirection = fpsCam.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, aimDirection, out hit, 1f, ~bulletIgnore))
        {
            Debug.Log(hit.transform.name);

            Health target = hit.transform.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(damage * 500f);
                hit.rigidbody.AddForce(-hit.normal * impactForce / 10f);
                GameObject impactGO = Instantiate(impactEffectBlood, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
            if (hit.rigidbody != null && target == null)
            {
                GameObject impactGO = Instantiate(impactEffectWOHole, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
                hit.rigidbody.AddForce(-hit.normal * impactForce / 10f);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce / 10f);
            }
        }
    }

    void Shoot()
    {
        isFiring = true;
        currentAmmo--;

        RaycastHit hit;

        int muzzleRotation = Random.Range(-41, 41);
        GameObject audioEffect = Instantiate(gunAudio, muzzleFlash.transform.position, Quaternion.identity);
        Destroy(audioEffect, 1f);
        GameObject muzzle = Instantiate(muzzlePrefeb, muzzleFlash.transform.position, Quaternion.identity);
        muzzle.transform.rotation = Quaternion.Euler(new Vector3(muzzleRotation, 0, 0));
        Destroy(muzzle, 0.075f);

        Vector3 aimDirection = fpsCam.transform.forward;
        if (Input.GetButton("Fire2"))
        {
            aimDirection = fpsCam.transform.forward;
        }
        else
        {
            aimDirection += Random.insideUnitSphere * bulletSpread;
        }

        if (Physics.Raycast(fpsCam.transform.position, aimDirection, out hit, range, ~bulletIgnore))
        {
            Debug.Log(hit.transform.name);
            Health target = hit.transform.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(damage);
                hit.rigidbody.AddForce(-hit.normal * impactForce);
                GameObject impactGO = Instantiate(impactEffectBlood, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
            if (hit.rigidbody != null && target == null)
            {
                GameObject impactGO = Instantiate(impactEffectWOHole, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
            else if(hit.rigidbody == null)
            {
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 10f);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            SpawnBulletTrail(hit.point);
        }
    }

    private void SpawnBulletTrail(Vector3 hitPoint)
    {
        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, muzzleFlash.transform.position, Quaternion.identity);

        LineRenderer lineRender = bulletTrailEffect.GetComponent<LineRenderer>();

        lineRender.SetPosition(0, muzzleFlash.transform.position);
        lineRender.SetPosition(1, hitPoint);

        Destroy(bulletTrailEffect, 0.05f);
    }

    void WeaponClose()
    {
        if (playerController.lookDistance > weaponLenght)
        {
            weaponClose = false;
            percentage = 0;
        }
        else
        {
            weaponClose = true;
            percentage = (playerController.lookDistance - weaponLenght) / (0.2f - weaponLenght);
        }
        float angle = percentage * weaponRotate;
        float distancex = percentage * weaponPositionx;
        float distancey = percentage * weaponPositiony;
        float distancez = percentage * weaponPositionz;
        Vector3 weaponPosition = new Vector3(distancex, distancey, distancez);
        gunRotate.transform.localRotation = Quaternion.Lerp(gunRotate.transform.localRotation, Quaternion.Euler(angle, 0f, 0f), Time.deltaTime * 5f);
        gunRotate.transform.localPosition = Vector3.Slerp(gunRotate.transform.localPosition, weaponPosition, Time.deltaTime * 5f);
    }
}
