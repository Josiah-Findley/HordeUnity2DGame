/* 
    Code adapted from CodeMonkey
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UI_Shop : MonoBehaviour
{

    private Transform container;
    private Transform shopItemTemplate;
    private IShopCustomer shopCustomer;

    //Keep track of items in shop
    public Transform[] shopItems;
    public const int NUM_ITEMS_IN_SHOP = 5;
    int itemSelected;

    Color navy;
    Color selected;

    private bool p1_toggleshop = false;
    public void Awake()
    {
        container = transform.Find("container");
        shopItemTemplate = container.Find("shopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
    }

    public void Start()
    {
        shopItems = new Transform[NUM_ITEMS_IN_SHOP];

        //shop elements
        CreateItemButton(Item.ItemType.gun1, Item.GetSprite(Item.ItemType.gun1), "BigBoy", Item.GetCost(Item.ItemType.gun1), 0);
        CreateItemButton(Item.ItemType.gun2, Item.GetSprite(Item.ItemType.gun2), "RapidFire", Item.GetCost(Item.ItemType.gun2), 1);
        CreateItemButton(Item.ItemType.gun3, Item.GetSprite(Item.ItemType.gun3), "2X Damage", Item.GetCost(Item.ItemType.gun3), 2);
        CreateItemButton(Item.ItemType.gun4, Item.GetSprite(Item.ItemType.gun4), "Grenader", Item.GetCost(Item.ItemType.gun4), 3);
        // CreateItemButton(Item.ItemType.Sword_2, Item.GetSprite(Item.ItemType.Sword_2), "Sword", Item.GetCost(Item.ItemType.Sword_2), 3);
        CreateItemButton(Item.ItemType.HealthPotion, Item.GetSprite(Item.ItemType.HealthPotion), "HPotion", Item.GetCost(Item.ItemType.HealthPotion), 4);


        navy = new Color(82 / 255f, 117 / 255f, 137 / 255f);
        selected = new Color(29 / 255f, 55 / 255f, 70 / 255f);
        itemSelected = 0;

        shopItems[itemSelected].Find("background").GetComponent<Image>().color = selected;

        Hide();
    }

    //UpdateShop
    public void UpdateStore(Item.ItemType item)
    {
        if (item == Item.ItemType.gun1 && shopCustomer.itemOwned(Item.ItemType.gun1))
            CreateItemButton(Item.ItemType.gun1, Item.GetSprite(Item.ItemType.gun1), "BigBoy", 0, 0);
        else if (item == Item.ItemType.gun2 && shopCustomer.itemOwned(Item.ItemType.gun2))
            CreateItemButton(Item.ItemType.gun2, Item.GetSprite(Item.ItemType.gun2), "RapidFire", 0, 1);
        else if (item == Item.ItemType.gun3 && shopCustomer.itemOwned(Item.ItemType.gun3))
            CreateItemButton(Item.ItemType.gun3, Item.GetSprite(Item.ItemType.gun3), "2X Damage", 0, 2);
        else if (item == Item.ItemType.gun4 && shopCustomer.itemOwned(Item.ItemType.gun4))
            CreateItemButton(Item.ItemType.gun4, Item.GetSprite(Item.ItemType.gun3), "Grenader", 0, 3);
        //else if (item == Item.ItemType.Sword_2 && shopCustomer.itemOwned(Item.ItemType.Sword_2))
        // CreateItemButton(Item.ItemType.Sword_2, Item.GetSprite(Item.ItemType.Sword_2), "Sword", 0, 3);
    }

    //Creates Shop
    private void CreateItemButton(Item.ItemType itemType, Sprite itemSprite, string itemName, int itemCost, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 55f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("nameText").GetComponent<TextMeshProUGUI>().SetText(itemName);
        shopItemTransform.Find("costText").GetComponent<TextMeshProUGUI>().SetText(itemCost.ToString());
        shopItemTransform.Find("itemImage").GetComponent<Image>().sprite = itemSprite;

        //Adds item to shopItems
        shopItems[positionIndex] = shopItemTransform;

    }


    public void Update()
    {

        /************************Goes Through Items*****************************/

        if (Input.GetButtonDown("P1_ToggleShop") || (Input.GetAxisRaw("P1_ToggleShop") > 0 && !p1_toggleshop))
        {
            p1_toggleshop = true;
            shopItems[itemSelected].Find("background").GetComponent<Image>().color = navy;
            itemSelected = (itemSelected + 1) % NUM_ITEMS_IN_SHOP;
            shopItems[itemSelected].Find("background").GetComponent<Image>().color = selected;
        }
        if (Input.GetAxisRaw("P1_ToggleShop") <= 0)
        {
            p1_toggleshop = false;
        }

        //Buy Desired Item
        if (Input.GetButtonDown("P1_BuyItem"))
        {
            switch (itemSelected)
            {
                case 0: TryBuyItem(Item.ItemType.gun1); break;
                case 1: TryBuyItem(Item.ItemType.gun2); break;
                case 2: TryBuyItem(Item.ItemType.gun3); break;
                case 3: TryBuyItem(Item.ItemType.gun4); break;
                // case 3: TryBuyItem(Item.ItemType.Sword_2); break;
                case 4: TryBuyItem(Item.ItemType.HealthPotion); break;
            }
        }
    }


    //Atempt to buy
    private void TryBuyItem(Item.ItemType itemType)
    {
        if (shopCustomer.TrySpendGoldAmount(Item.GetCost(itemType), itemType))
        {
            // Can afford cost
            shopCustomer.BoughtItem(itemType);
            UpdateStore(itemType);
        }
    }



    public void Show(IShopCustomer shopCustomer)
    {
        this.shopCustomer = shopCustomer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
