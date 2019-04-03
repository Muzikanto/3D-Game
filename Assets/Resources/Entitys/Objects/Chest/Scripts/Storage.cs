using System;
using UnityEngine;
using Assets.Other;
using Assets.Inventorys;
using UnityEngine.EventSystems;


namespace Assets.Objects.Storage
{
    public class Storage : MonoBehaviour, InventoryExchangeItemsInterface
    {
        [SerializeField] public GameObject interfaceInventory;
        public GameObject inventoryView;

        public bool isOpen = false;

        public InventoryExchangeItems inventoryExchangeItems = new InventoryExchangeItems();
        public void RemoveSelectedItem() { inventoryExchangeItems.RemoveSelectedItem(); }
        public void ItemSelect(int index) { inventoryExchangeItems.ItemSelect(index); }
        public void GetItem() { inventoryExchangeItems.GetItem(); }
        public void TakeItem(ItemInventory item) { inventoryExchangeItems.TakeItem(item); }

        protected void Awake()
        {
            interfaceInventory = Instantiate(interfaceInventory);
            interfaceInventory.transform.SetParent(gameObject.transform);
            interfaceInventory.SetActive(false);
            inventoryView = interfaceInventory.transform.Find("Inventory").gameObject;

            inventoryExchangeItems.SetInventoryOnclicksListneer(inventoryView, 1, 2);

        }

        protected void Update()
        {
            if (isOpen)
            {
                if (Input.anyKey)
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
                    {
                        isOpen = false;
                        interfaceInventory.SetActive(false);
                        inventoryExchangeItems.anotherInventory.SendMessage("setIsControll", true);
                    }
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                inventoryExchangeItems.UpdateInventory(inventoryView, 1, 2, 0, 10);
            }
        }
    
        //SLOT
        public void OpenChest(GameObject playerV)
        {
            isOpen = true;
            interfaceInventory.SetActive(true);
            inventoryExchangeItems.anotherInventory = playerV;
        }

    }


}
