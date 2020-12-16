/* 
    Code adapted from CodeMonkey
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStats : MonoBehaviour, IShopCustomer
{

    //Updates UI on events
    public event EventHandler OnGoldAmountChanged;
    public event EventHandler OnHealthPotionAmountChanged;

    //UI Elements
    public int goldAmount;
    public int healthPotionAmount;

    //Items Owned
    public int itemOwnCntr;
    public GameObject weaponslot;
    public Item.ItemType[] itemsOwned;

    //Health
    public int maxHealth = 100;
    public int currentHealth;

    //Health
    public HealthBarScript HealthBar;

    public AudioSource audio;
    public AudioClip hpUp;

    float incrementGuard; //guards us from double dipping when adding currency

    public void Start()
    {
        goldAmount = 0;
        healthPotionAmount = 0;

        itemOwnCntr = 0;
        itemsOwned = new Item.ItemType[10];
        itemsOwned[itemOwnCntr] = Item.ItemType.gun;

        //Health
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);

        incrementGuard = Time.time;
    }



    /************************Getters*****************************/
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetHealthPotionAmount()
    {
        return healthPotionAmount;
    }

    public int GetGoldAmount()
    {
        return goldAmount;
    }




    /************************Interacting with UI*****************************/
    public void TryConsumeHealthPotion()
    {
        if (healthPotionAmount>0&&currentHealth != 100)
        {
            audio.PlayOneShot(hpUp);
            currentHealth = (currentHealth+50);
            if (currentHealth > 100)
                currentHealth = 100;
            healthPotionAmount--;
            OnHealthPotionAmountChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TrySpendGoldAmount(int spendGoldAmount, Item.ItemType item)
    {
        if (GetGoldAmount() >= spendGoldAmount && !itemOwned(item))
        {
            goldAmount -= spendGoldAmount;
            OnGoldAmountChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void BoughtItem(Item.ItemType itemType)
    {
        Debug.Log("Bought item: " + itemType);
        switch (itemType)
        {
            case Item.ItemType.HealthPotion: AddHealthPotion(); break;
            case Item.ItemType.gun1: AddWeapon(Item.ItemType.gun1); weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun1); break;
            case Item.ItemType.gun2: AddWeapon(Item.ItemType.gun2); weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun2); break;
            case Item.ItemType.gun3: AddWeapon(Item.ItemType.gun3); weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun3); break;
            case Item.ItemType.gun4: AddWeapon(Item.ItemType.gun4); weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun4); break;
        }
    }



    /************************Update the UI*****************************/

    private void AddHealthPotion()
    {
        healthPotionAmount++;
        OnHealthPotionAmountChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddGoldAmount(int addGoldAmount)
    {
        if (Time.time - incrementGuard > 0.005)
        {
            goldAmount += addGoldAmount;
            OnGoldAmountChanged?.Invoke(this, EventArgs.Empty);
            incrementGuard = Time.time;
        }
    }


    /************************Add and keep track of Inventory*****************************/


    //add weapon
    public void AddWeapon(Item.ItemType weapon)
    {
        if (!itemOwned(weapon))
        {
            itemOwnCntr++;
            itemsOwned[itemOwnCntr] = weapon;
        }
       /* if (weapon == Item.ItemType.Sword_2) {
            weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(weapon);
        }*/
    }


    //check if player has item 
    public bool itemOwned(Item.ItemType weapon)
    {
        for (int i = 0; i < itemsOwned.Length; i++)
            if (itemsOwned[i] == weapon)
                return true;
        return false;
    }

    /************************Health Management*****************************/
    public void resetPlayerHealth()
    {
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);
    }
}