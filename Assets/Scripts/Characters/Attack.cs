using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    public int Damage;
    public float HitboxDuration;
    public float CoolDown;

    private bool active;
    private bool onCooldown;
    private float timeActive;
    private float coolDownTime;

	// Use this for initialization
	void Start ()
	{
	    onCooldown = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (active)
        {
            timeActive += Time.deltaTime;
            if (timeActive >= HitboxDuration)
            {
                Deactivate();
            }
        }
	    if (onCooldown)
	    {
	        coolDownTime += Time.deltaTime;
	        if (coolDownTime >= CoolDown)
	        {
	            onCooldown = false;
	        }
	    }
	}

    public bool CanActivate { get { return !onCooldown; } }

    public void ActivateAttack()
    {
        active = true;
        //onCooldown = true;
        coolDownTime = 0f;
        timeActive = 0f;
    }

    private void Deactivate()
    {
        active = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (active && other.CompareTag("Enemy"))
        {
            other.GetComponent<LivingCharacter>().Hurt(Damage);
            active = false;
        }
    }
}
