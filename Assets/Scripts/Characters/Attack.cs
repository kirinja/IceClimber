using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public int Damage;
    public float AttackTime;

    private bool active;
    private float timeActive;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (active)
        {
            timeActive += Time.deltaTime;
            if (timeActive >= AttackTime)
            {
                Deactivate();
            }
        }
	}

    public void Activate()
    {
        active = true;
        timeActive = 0f;
    }

    private void Deactivate()
    {
        active = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (active && other.CompareTag("Enemy"))
        {
            other.GetComponent<LivingCharacter>().Hurt(Damage);
        }
    }
}
