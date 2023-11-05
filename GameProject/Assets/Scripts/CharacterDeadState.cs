using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDeadState : MonoBehaviour
{
    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsDead()
    {
        return isDead == true;
    }

    public void Dead()
    {
        isDead = true;
    }
}
