using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Other;
using UnityEngine.EventSystems;
using Assets.Inventorys;

namespace Assets.Character
{
    [RequireComponent(typeof(PlayerController))]
    public class UserPlayerController : MonoBehaviour, InventoryExchangeItemsInterface
    {
        //----------------------------------------
        public Stats stats = new Stats();
        public InterfaceViews interfaceViews = new InterfaceViews();
        public GameObject cameraContainer;
        public Camera cam;

        private PlayerController controller;
        private InventoryExchangeItems inventoryExchangeItems = new InventoryExchangeItems();
        private bool isControll = true;
        private bool nextPressKeyE = true;


        //----------------------------------------
        void Start()
        {
            controller = GetComponent<PlayerController>();
         
            interfaceViews.interfaceView.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            interfaceViews.interfaceView.GetComponent<Canvas>().worldCamera = cam;
            interfaceViews.interfaceView.SetActive(true);
            interfaceViews.inventoryView.SetActive(false);
            inventoryExchangeItems.SetInventoryOnclicksListneer(interfaceViews.inventoryView, interfaceViews.toolBar, 1, 3);

            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.SuperTank,5, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.M4A3E2, 5, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Pz4, 5, Id.TypeItem.Entity));

            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.AlfaRomeo,2, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Porshe, 2, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Lamborgini, 2, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Dodge, 2, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.SkyCar,2, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Jet,3, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Plane, Id.TypeItem.Entity));

            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Turret1, 50, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Turret2, 3, Id.TypeItem.Entity));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.Turret3, 3, Id.TypeItem.Entity));

           // inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Entity.ChestBlue, 5, Id.TypeItem.Entity));

            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.Briks, 64, Id.TypeItem.Block));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.Grass, 64, Id.TypeItem.Block));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.RedHoneyCumb, 64, Id.TypeItem.Block));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.SnakeLether, 64, Id.TypeItem.Block));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.Stone, 64, Id.TypeItem.Block));
            inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory((int)Id.Block.Wood, 64, Id.TypeItem.Block));
        }

        void Update()
        {
            if (isControll)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    OpenInventory(!interfaceViews.inventoryView.active);
                }
                if (!interfaceViews.inventoryView.active)
                {
                    controller.RotateAndJump();

                    //Select ToolBar Item
                    inventoryExchangeItems.inventory.selectCurrentItemToolBar();

                    //Update Interface stats
                    updateInterface();

                    //Raycast
                    checkForwardRaycst();

                    //
                    take_Or_Place_Item();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    inventoryExchangeItems.UpdateInventory(interfaceViews.inventoryView, 1, 3,10, 10);
                    inventoryExchangeItems.UpdateInventoryLine(interfaceViews.toolBar, 0, 10);
                }
            }
            else if (interfaceViews.inventoryView.active)
            {
                inventoryExchangeItems.UpdateInventory(interfaceViews.inventoryView, 1, 3, 10, 10);
                inventoryExchangeItems.UpdateInventoryLine(interfaceViews.toolBar, 0, 10);
            }      
        }

        private void FixedUpdate()
        {
            if(isControll)
             controller.Move();
        }

        //Inventory
        //----------------------------------------    

        private void take_Or_Place_Item()
        {
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(cameraRay, 2f);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Block" || hit.collider.tag == "Vehicles" || hit.collider.tag == "Turret" || hit.collider.tag == "Chest")
                    {
                        Transform parent = hit.collider.transform;
                        if(hit.collider.tag != "Block")
                            parent = OtherMetods.FindParent(hit.collider.transform, "Entitys");
                        if (parent == null)
                            return;
    
                        int idItem = Id.getId(parent.name);

                        ItemInventory item = new ItemInventory(idItem, 1, Id.TypeItem.None);

                        if (hit.collider.tag == "Block")
                            item.type = Id.TypeItem.Block;
                        else 
                            item.type = Id.TypeItem.Entity;

                        inventoryExchangeItems.inventory.AddItemInventory(item);

                        Destroy(parent.gameObject);
                        break;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar].count > 0)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(cam.transform.position, cameraRay.direction * 2f);

                    ItemInventory item = inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar];

                    if (item.count > 0 && item.type != Id.TypeItem.None)
                    {
                        switch (item.type)
                        {
                            case Id.TypeItem.Block:
                                {
                                    if (Physics.Raycast(ray, out hit, 2f))
                                    {
                                        int x, y, z;
                                        x = Mathf.RoundToInt(hit.point.x);
                                        y = Mathf.RoundToInt(hit.point.y);
                                        z = Mathf.RoundToInt(hit.point.z);
                                        if (hit.point.x % 0.5 == 0)
                                        {
                                            if (transform.position.x < hit.point.x)
                                            {
                                                if (x != (int)hit.point.x)
                                                    x--;
                                            }
                                            else
                                            {
                                                if (x != (int)hit.point.x + 1)
                                                    x++;
                                            }
                                        }
                                        if (hit.point.y % 0.5 == 0)
                                        {
                                            if (cam.transform.position.y < hit.point.y)
                                            {
                                                if (y != (int)hit.point.y)
                                                    y--;
                                            }
                                            else
                                            {
                                                if (y != (int)hit.point.y + 1)
                                                    y++;
                                            }
                                        }
                                        if (hit.point.z % 0.5 == 0)
                                        {
                                            if (transform.position.z < hit.point.z)
                                            {
                                                if (z != (int)hit.point.z)
                                                    z--;
                                            }
                                            else
                                            {
                                                if (z != (int)hit.point.z + 1)
                                                    z++;
                                            }
                                        }

                                        MyTerrain.CreateBlock(item.id, new Vector3(x, y, z));
                                    }
                                    break;
                                }
                            case Id.TypeItem.Entity:
                                {
                                    Vector3 pos = transform.position + transform.forward * 5;
                                    MyTerrain.CreateEntity(item.id, pos, transform.rotation);
                                    break;
                                }
                        }
                        inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar].count--;
                        if (inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar].count == 0)
                        {
                            inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar].type = Id.TypeItem.None;
                            inventoryExchangeItems.inventory.arrItems[inventoryExchangeItems.inventory.currentItemToolbar].id = -1;
                        }
                    }
                }
            }
        }
      
        private void OpenInventory(bool b)
        {
            inventoryExchangeItems.inventory.selectedItem = -1;
            interfaceViews.inventoryView.SetActive(b);
        }

        //Interface
        //----------------------------------------
        private void updateInterface()
        {
            interfaceViews.healthView.GetComponent<UnityEngine.UI.Text>().text = "Health: " + stats.heath;
            interfaceViews.ammoView.GetComponent<UnityEngine.UI.Text>().text = "Ammo: " + stats.ammo;
            interfaceViews.scoreView.GetComponent<UnityEngine.UI.Text>().text = "Score: " + stats.score;

            inventoryExchangeItems.UpdateInventoryLine(interfaceViews.toolBar,0, 10);
        }
     
        //Raycast
        //----------------------------------------
        private void checkForwardRaycst()
        {
            RaycastHit hit;
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out hit, 2.0f))
            {
                if (hit.collider.tag == "VehicleDoor")
                {
                    interfaceViews.checkRaycast.SetActive(true);
                    interfaceViews.checkRaycast.GetComponent<UnityEngine.UI.Text>().text = "Press E to seat";
                    if (Input.GetKeyUp(KeyCode.E) && nextPressKeyE)
                    {
                        isControll = false;
                        gameObject.SetActive(false);
                 
                        GameObject obj = OtherMetods.FindParent(hit.collider.transform, "Entitys").gameObject;
                        obj.SendMessage("setIsControll", new SendMessage_CamPos_And_PlayerPos(cam, gameObject));
                    }
                }
                else if(hit.collider.tag == "Turret")
                {
                    interfaceViews.checkRaycast.SetActive(true);
                    interfaceViews.checkRaycast.GetComponent<UnityEngine.UI.Text>().text = "Press E to activate";
                    if (Input.GetKeyUp(KeyCode.E) && nextPressKeyE)
                    {
                        OtherMetods.FindParent(hit.collider.transform, "Entitys").SendMessage("setActive");
                        StartCoroutine("delayPressKeyE", 0.5f);
                    }
                }
                else if(hit.collider.tag == "Chest")
                {
                    interfaceViews.checkRaycast.SetActive(true);
                    interfaceViews.checkRaycast.GetComponent<UnityEngine.UI.Text>().text = "Press E to open Inventory";
                    if (Input.GetKeyUp(KeyCode.E) && nextPressKeyE)
                    {
                        OpenInventory(true);
                        isControll = false;
                        GameObject obj = hit.collider.gameObject;
                        obj.gameObject.SendMessage("OpenChest", gameObject);
                        inventoryExchangeItems.anotherInventory = obj;
                    }
                }
                else if (interfaceViews.checkRaycast.activeSelf)
                    interfaceViews.checkRaycast.SetActive(false);
            }
            else if (interfaceViews.checkRaycast.activeSelf)
                interfaceViews.checkRaycast.SetActive(false);
        }

        private IEnumerator delayPressKeyE(float time)
        {
            nextPressKeyE = false;
            yield return new WaitForSeconds(time);
            nextPressKeyE = true;
        }

        //SLOT
        //----------------------------------------
        public void setIsControll(SendMessage_CamPos_And_PlayerPos mess)
        {
            isControll = true;
            cam = mess.cam;
            OtherMetods.camTransformParent_camDropSettings(cam, cameraContainer);
            StartCoroutine("delayPressKeyE", 1f);
            transform.position = mess.player.transform.position;
        }

        //SLOT
        public void setIsControll(bool b)
        {
            inventoryExchangeItems.anotherInventory = null;
            OpenInventory(false);
            isControll = b;
            StartCoroutine("delayPressKeyE", 0.5f);
        }

        //----------------------------------------
        [Serializable]
        public class InterfaceViews
        {
            public GameObject interfaceView;
            public GameObject healthView;
            public GameObject ammoView;
            public GameObject scoreView;
            public GameObject checkRaycast;
            public GameObject inventoryView;
            public GameObject toolBar;           
        }
   
        [Serializable]
        public class Stats
        {
            public int heath = 0;
            public int ammo = 0;
            public int score = 0;
        }
        //----------------------------------------

        public void RemoveSelectedItem() { inventoryExchangeItems.RemoveSelectedItem(); }
        public void ItemSelect(int index) { inventoryExchangeItems.ItemSelect(index); }
        public void GetItem() { inventoryExchangeItems.GetItem(); }
        public void TakeItem(ItemInventory item) { inventoryExchangeItems.TakeItem(item); }
    }
}