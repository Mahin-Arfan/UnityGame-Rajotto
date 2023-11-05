using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    [SerializeField] private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController.weaponScript = GetComponentInChildren<WeaponScript>();
        playerController.weaponAnimator = playerController.weaponScript.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && playerController.weaponScript.canChangeWeapon)
        {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2 && playerController.weaponScript.canChangeWeapon)
        {
            selectedWeapon = 1;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            StartCoroutine(SelectWeapon());
        }

        if(selectedWeapon == 1)
        {
            playerController.characterController.radius = 0.15f;
        }
        else
        {
            playerController.characterController.radius = 0.25f;
        }
    }

    IEnumerator SelectWeapon()
    {
        int i = 0;
        playerController.weaponAnimator.SetBool("switching", true);
        yield return new WaitForSeconds(1f);
        playerController.weaponAnimator.SetBool("switching", false);
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }

        playerController.weaponScript = GetComponentInChildren<WeaponScript>();
        playerController.weaponAnimator = playerController.weaponScript.GetComponent<Animator>();
    }
}
