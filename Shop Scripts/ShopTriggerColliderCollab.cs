/* 
    Code adapted from codeMonkey
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTriggerColliderCollab : MonoBehaviour
{

    [SerializeField] private UI_Shop2 uiShop2;
    [SerializeField] private UI_Shop uiShop;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player1") {
            IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();

            if (shopCustomer != null)
            {
                uiShop.Show(shopCustomer);
            }
        }
        if (collider.gameObject.tag == "Player2")
        {
            IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();

            if (shopCustomer != null)
            {
                uiShop2.Show(shopCustomer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player1")
        {
            IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();

            if (shopCustomer != null)
            {
                uiShop.Hide();
            }
        }
        if (collider.gameObject.tag == "Player2")
        {
            IShopCustomer shopCustomer = collider.GetComponent<IShopCustomer>();

            if (shopCustomer != null)
            {
                uiShop2.Hide();
            }
        }
    }

}
