/* 
    Code adapted from CodeMonkey
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShopCustomer {
    void BoughtItem(Item.ItemType itemType);
    bool TrySpendGoldAmount(int goldAmount, Item.ItemType item);
    void AddWeapon(Item.ItemType weapon);
    bool itemOwned(Item.ItemType weapon);
    }
