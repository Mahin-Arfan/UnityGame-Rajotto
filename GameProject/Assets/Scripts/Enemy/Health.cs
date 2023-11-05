using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Enemy_Damage enemy_Damage;
    [SerializeField] private float health = 50f;
    public void Start()
    {
        
    }
    public void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if (enemy_Damage.mainHealth > 0)
        {
            enemy_Damage.mainHealth -= health * (amount / 100);
            enemy_Damage.takingHit = true;
            if(enemy_Damage.mainHealth <= 0)
            {
                enemy_Damage.Die();
            }
        }
    }
}
