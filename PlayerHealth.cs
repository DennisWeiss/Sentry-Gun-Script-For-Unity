using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    public float health;

    bool dead;

	// Use this for initialization
	void Start () {
        dead = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (health <= 0 && !dead)
        {
            Death();
        }
	}

    public void dealDamage(float damage)
    {
        health -= damage;
    }

    public void Death()
    {
        dead = true;
        Debug.Log("dead");
    }
}
