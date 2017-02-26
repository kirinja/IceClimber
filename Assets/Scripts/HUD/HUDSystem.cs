using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    public GameObject ThreeQuarterHeart;
    public GameObject OneQuarterHeart;
    public GameObject EmptyHeart;
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
        CalculateHealth(GameObject.FindGameObjectWithTag("Player").GetComponent<LivingCharacter>().health, GameObject.FindGameObjectWithTag("Player").GetComponent<LivingCharacter>().MaxHealth);
        SetHeight(0);
        CountFruit(0);
    }

	// Update is called once per frame
	void Update ()
    {
        // hackish, might be detremental to performance
        SetHeight(GameObject.FindGameObjectWithTag("Player").transform.position.y);
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
    public void CalculateHealth(int health, int max)
    {
        ClearHearts();

        int half = HealthPerHeart / 2;
        int oneQuart = HealthPerHeart / 4;
        int threeQuart = oneQuart * 3;
        
        // first check how many full hearts
        int fullH = health / HealthPerHeart;

        // then check how many three quarts out of the remainder
        int remainder = health - (fullH * HealthPerHeart);
        int threeQh = remainder / threeQuart;

        // if the three quarts value is above 1 then we can skip the rest of the calculations
        int halfH = 0;
        int oneQh = 0;
        if (threeQh <= 0)
        {
            // check how many half hearts remains
            halfH = remainder / half;
            if (halfH <= 0)
                oneQh = remainder / oneQuart;
        }

        // finally we need to calculate how many empty hearts we need to show (we can do this by comparing the current health to the max value)
        int emptyH = (max - health) / HealthPerHeart;

        // next we create and place the elements
        int counter = 0;
        for (int i = 0; i < fullH; i++)
        {
            var obj = Instantiate(FullHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
        for (int i = 0; i < threeQh; i++)
        {
            var obj = Instantiate(ThreeQuarterHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
        for (int i = 0; i < halfH; i++)
        {
            var obj = Instantiate(HalfHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
        for (int i = 0; i < oneQh; i++)
        {
            var obj = Instantiate(OneQuarterHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
        for (int i = 0; i < emptyH; i++)
        {
            var obj = Instantiate(EmptyHeart);
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3((50 * counter) + (SpacingX * counter), transform.position.y * 2, 0);
            hearts[counter] = obj;
            counter++;
        }
    }

    // this can simply be the Y position of the player
    public void SetHeight(float height)
    {
        HeightText.text = "Height: " + height.ToString("F") + " m";
    }

    public void CountFruit(int amount)
    {
        // we can get the max amount of fruits from the scene/level itself (might need tweaking depending on how we implement scenes/levels)
        // we could also just start with a fruit number, that we set each "level"
        // this is gonna use one Image and text
        FruitText.text = amount + " / " + levelFruitCount;
    }
}
