using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingCharacter : MonoBehaviour {

    public int MaxHealth;

    // public for easy debugging
    public int health;

	// Use this for initialization
	void Start () {
        //health = MaxHealth;
	}

    // changed to Awake instead of Start since HUD need this to be calculated before it runs (Awake runs before Start)
    void Awake()
    {
        health = MaxHealth;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Hurt(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        // TODO: Add stuff
    }
}
