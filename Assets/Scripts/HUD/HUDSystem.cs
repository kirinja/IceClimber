using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDSystem: MonoBehaviour
{
    // we're gonna use this to update the amount of UI images to render on screen, we're gonna make each heart a resource and then instaniate them
    // We need to grab some components from the player, such as health, collected fruits and how high we have climbed
    // we're gonna get the total amount of fruits from the scene itself, by searching for a tag and counting how many there are

    private int levelFruitCount;
    //private int playerHealth;

    public GameObject FullHeart;
    public GameObject HalfHeart;
    public int HealthPerHeart;
    public float SpacingX;

    public Text HeightText;
    public Text FruitText;

    private GameObject[] hearts;

	// Use this for initialization
	void Start ()
	{
        hearts = new GameObject[10];
        levelFruitCount = 10;
        CalculateHealth(GameObject.FindGameObjectWithTag("Player").GetComponent<LivingCharacter>().MaxHealth);
        SetHeight(0);
        CountFruit(0);
    }

	// Update is called once per frame
	void Update ()
    {

	}

    private void ClearHearts()
    {
        // pretty crude way of doing this but it should work
        foreach (var g in hearts)
        {
            Destroy(g);
        }
    }

    // we need a way to recalculate the HUD, currently we're only adding stuff but we cant remove them
    public void CalculateHealth(int health)
    {
        ClearHearts();

        // this is all kinda hackishly done
        int fullhearts = health / HealthPerHeart;
        int halfhearts = health - fullhearts * HealthPerHeart;
        halfhearts = (halfhearts * 2) / HealthPerHeart;
        
        int counter = 0;
        for (int i = 0; i < fullhearts; i++)
        {
            var obj = Instantiate(FullHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
        for (int j = 0; j < halfhearts; j++)
        {
            var obj = Instantiate(HalfHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
    }

    // this can simply be the Y position of the player
    public void SetHeight(float height)
    {
        HeightText.text = "Height: " + height + " m";
    }

    public void CountFruit(int amount)
    {
        // we can get the max amount of fruits from the scene/level itself (might need tweaking depending on how we implement scenes/levels)
        // we could also just start with a fruit number, that we set each "level"
        // this is gonna use one Image and text
        FruitText.text = amount + " / " + levelFruitCount;
    }
}
