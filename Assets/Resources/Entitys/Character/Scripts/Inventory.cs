using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Other;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Inventorys
{
    public class Inventory
    {
        public ItemInventory[] arrItems;
        public int currentItemToolbar = 0;
        public int sizeInventory = 40;
        public int selectedItem = -1;

        public Inventory()
        {
            createEmptyInventory();
        }

        private void createEmptyInventory()
        {
            arrItems = new ItemInventory[sizeInventory];
            ItemInventory emptyItem = getEmptyItem();
            for (int i = 0; i < sizeInventory; i++)
            {
                arrItems[i] = emptyItem;
            }
        }

        public bool AddItemInventory(ItemInventory item)
        {
            for (int i = 0; i < sizeInventory; i++)
            {
                if (arrItems[i].id == item.id && arrItems[i].type == item.type)
                {
                    if (item.count > 0)
                        arrItems[i].count += item.count;
                    else
                        arrItems[i].count = item.count;
                    return true;
                }
            }
            for (int i = 0; i < sizeInventory; i++)
            {
                if (arrItems[i].type == Id.TypeItem.None)
                {
                    if (item.count == 0)
                        item.count = 1;
                    arrItems[i] = item;
                    return true;
                }
            }
            return false;
        }

        public static ItemInventory getEmptyItem()
        {
            return new ItemInventory(-1, 0, Id.TypeItem.None);
        }

        public void selectCurrentItemToolBar()
        {
            //InventoryToolbarCurrentItem
            for (int i = 49; i < 58; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    currentItemToolbar = i - 49;
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
                currentItemToolbar = 9;
        }
    }

    public class ItemInventory
    {
        public int count = 0;
        public int id;
        public Id.TypeItem type;
        public ItemInventory(int id, int count, Id.TypeItem type)
        {
            this.count = count;
            this.id = id;
            this.type = type;
        }
        public ItemInventory(int id, Id.TypeItem type)
        {
            this.id = id;
            this.type = type;
        }
    }

    public class Id
    {
        public enum TypeItem { None, Block, Entity };

        //-------------------------
        private static string pathToBlockIcon = "Terrain/Blocks/Textures/";
        private static string pathToBlockPrefab = "Terrain/Blocks/Prefabs/";

        public enum Block { Grass, Wood, RedHoneyCumb, SnakeLether, Stone, Briks };

        public static ItemId[] blocks = new ItemId[]  {
        new ItemId("Grass", pathToBlockIcon + "Grass", pathToBlockPrefab + "Grass", 0.5f),
        new ItemId("Wood", pathToBlockIcon + "Wood", pathToBlockPrefab + "Wood", 0.5f),
        new ItemId("RedHoneyCumb", pathToBlockIcon + "RedHoneyCumb", pathToBlockPrefab + "RedHoneyCumb", 0.5f),
        new ItemId("SnakeLether", pathToBlockIcon + "GreenSnake", pathToBlockPrefab + "SnakeLether", 0.5f),
        new ItemId("Stone", pathToBlockIcon + "Stone", pathToBlockPrefab + "Stone", 0.5f),
        new ItemId("Briks", pathToBlockIcon + "Briks", pathToBlockPrefab + "Briks", 0.5f),
        new ItemId("Water", pathToBlockIcon + "Water", pathToBlockPrefab + "Water", 0.5f)};

        //--------------------------

        private static string pathToVehiclesIcon = "Entitys/Vehicles/Car/Icons/";
        private static string pathToVehiclesPrefabs = "Entitys/Vehicles/Car/Prefabs/";

        private static string pathToPlaneIcon = "Entitys/Vehicles/Aircraft/Icons/";
        private static string pathToPlanePrefabs = "Entitys/Vehicles/Aircraft/Prefabs/";

        private static string pathToTurretIcon = "Entitys/Turret/Icons/";
        private static string pathToTurretPrefabs = "Entitys/Turret/Prefabs/";

        private static string pathToChestIcon = "Entitys/Objects/Chest/Icons/";
        private static string pathToChestPrefabs = "Entitys/Objects/Chest/Prefabs/";

        public enum Entity { SkyCar, AlfaRomeo, Dodge, Lamborgini, Porshe,
            SuperTank, M4A3E2,
            Jet, Plane,
            Pz4, Turret1, Turret2, Turret3,
            ChestGreen, ChestGreenOx, ChestBlue, ChestBlueOx, ChestRed, ChestRedOx, ChestYellow, ChestYellowOx
        };

        public static ItemId[] entitys = new ItemId[]
        {
        new ItemId("SkyCar", pathToVehiclesIcon + "imageSkyCar", pathToVehiclesPrefabs + "SkyCar", 0.2f),
          new ItemId("AlfaRomeo", pathToVehiclesIcon + "imageAlfaRomeo", pathToVehiclesPrefabs + "AlfaRomeo", 0.2f),
            new ItemId("Dodge", pathToVehiclesIcon + "imageDodge", pathToVehiclesPrefabs + "Dodge", 0.2f),
              new ItemId("Lamborgini", pathToVehiclesIcon + "imageLamborgini", pathToVehiclesPrefabs + "Lamborgini", 0.2f),
                new ItemId("Porshe", pathToVehiclesIcon + "imagePorshe", pathToVehiclesPrefabs + "Porshe", 0.2f),

        new ItemId("SuperTank", pathToVehiclesIcon + "imageTank", pathToVehiclesPrefabs + "Tank", 0.1f),
          new ItemId("M4A3E2", pathToVehiclesIcon + "imageM4A3E2", pathToVehiclesPrefabs + "M4A3E2", 0.1f),
            new ItemId("Pz4", pathToVehiclesIcon + "imagePz4", pathToVehiclesPrefabs + "Pz4", 0.1f),  

        new ItemId("Plane", pathToPlaneIcon + "imagePlane", pathToPlanePrefabs + "AircraftPropeller", 0.05f),
          new ItemId("Jet", pathToPlaneIcon + "imageJet", pathToPlanePrefabs + "AircraftJet", 0.05f),

        new ItemId("Turret1", pathToTurretIcon + "imageTurret1", pathToTurretPrefabs + "Turret1",0.2f),
          new ItemId("Turret2", pathToTurretIcon + "imageTurret2", pathToTurretPrefabs + "Turret2",0.1f),
            new ItemId("Turret3", pathToTurretIcon + "imageTurret3", pathToTurretPrefabs + "Turret3",0.05f),

              new ItemId("ChestGreen", pathToChestIcon + "ChestGreen", pathToChestPrefabs + "chest_verde", 0.01f),
                new ItemId("ChestGreenOx", pathToChestIcon + "ChestGreenOx", pathToChestPrefabs + "chest_verde_ox", 0.01f),
                  new ItemId("ChestBlue", pathToChestIcon + "ChestBlue", pathToChestPrefabs + "chest_blue", 0.01f),
                    new ItemId("ChestBlueOx", pathToChestIcon + "ChestBlueOx", pathToChestPrefabs + "chest_blue", 0.01f),
                      new ItemId("ChestYellow", pathToChestIcon + "ChestYellow", pathToChestPrefabs + "chest_yellow", 0.1f),
                        new ItemId("ChestYellowOx", pathToChestIcon + "ChestYellowOx", pathToChestPrefabs + "chest_yellow_ox", 0.01f),
                          new ItemId("ChestRed", pathToChestIcon + "ChestRed", pathToChestPrefabs + "chest_red", 0.01f),
                            new ItemId("ChestRedOx", pathToChestIcon + "ChestRedOx", pathToChestPrefabs + "chest_red_ox", 0.01f),
        };


        //----------------------------
        public static int getId(string name)
        {
            return int.Parse(name.Split('_')[0]);
        }

        public static string setNameObj(int id, TypeItem type)
        {
            switch (type)
            {
                case TypeItem.Block:
                    {
                        blocks[id].count++;
                        return id + "_" + blocks[id].count;
                    }
                case TypeItem.Entity:
                    {
                        entitys[id].count++;
                        return id + "_" + entitys[id].count;
                    }
            }
            return null;
        }

        public class ItemId
        {
            private static int currentItemId = 0;
            public float chance = 0;
            public int count = 0;
            public int id { get; private set; }
            public string name { get; private set; }
            public string pathToIcon { get; private set; }
            public string pathToPrefab { get; private set; }
            public ItemId(string name, string icon, string prefab, float chance)
            {
                currentItemId++;
                id = currentItemId;
                this.name = name;
                this.pathToIcon = icon;
                this.pathToPrefab = prefab;
                this.chance = chance;
            }
        }
    }


    interface InventoryExchangeItemsInterface
    {
        void RemoveSelectedItem();
        void ItemSelect(int index);
        void GetItem();
        void TakeItem(ItemInventory item);
    }

    public class InventoryExchangeItems
    {
        public Inventory inventory = new Inventory();
        public GameObject anotherInventory;

        //SLOT
        public void RemoveSelectedItem()
        {
            inventory.selectedItem = -1;
        }

        //SLOT
        public void ItemSelect(int index)
        {
            if (inventory.selectedItem == -1)
            {
                if (inventory.arrItems[index].type != Id.TypeItem.None)
                    inventory.selectedItem = index;
                else
                    anotherInventory.SendMessage("GetItem");
            }
            else
            {
                ItemInventory sec = inventory.arrItems[inventory.selectedItem];
                inventory.arrItems[inventory.selectedItem] = inventory.arrItems[index];
                inventory.arrItems[index] = sec;
                inventory.selectedItem = -1;
            }
            if(anotherInventory != null)
                anotherInventory.SendMessage("RemoveSelectedItem");
        }

        //SIGNAL
        public void GetItem()
        {
            if (inventory.selectedItem != -1)
            {
                anotherInventory.SendMessage("TakeItem", inventory.arrItems[inventory.selectedItem]);
                inventory.arrItems[inventory.selectedItem] = Inventory.getEmptyItem();
            }
        }

        //SLOT
        public void TakeItem(ItemInventory item)
        {
            inventory.AddItemInventory(item);
        }

        //Updates
        public void UpdateInventoryLine(GameObject lineView, int startItem, int countItemLine)
        {
            for (int i = 0, j = startItem; i < 10; i++, j++)
            {
                GameObject objCount = lineView.transform.Find("Item" + i + "/Count").gameObject;
                objCount.GetComponent<UnityEngine.UI.Text>().text = inventory.arrItems[j].count.ToString();

                GameObject objTexture = lineView.transform.Find("Item" + i).gameObject;

                if (lineView.name == "ToolBar")
                {
                    if (inventory.currentItemToolbar == i)
                        objTexture.transform.localPosition = new Vector3(objTexture.transform.localPosition.x, 5, objTexture.transform.localPosition.z);
                    else
                        objTexture.transform.localPosition = new Vector3(objTexture.transform.localPosition.x, 0, objTexture.transform.localPosition.z);
                }

                ItemInventory item = inventory.arrItems[j];
                if (item.type != Id.TypeItem.None)
                {
                    if (item.count > 0 && item.type == Id.TypeItem.Block)
                        objTexture.GetComponent<UnityEngine.UI.RawImage>().texture = Resources.Load<Texture>(Id.blocks[item.id].pathToIcon);
                    else
                        objTexture.GetComponent<UnityEngine.UI.RawImage>().texture = Resources.Load<Texture>(Id.entitys[item.id].pathToIcon);
                }
                else
                {
                    objTexture.GetComponent<UnityEngine.UI.RawImage>().texture = Resources.Load<Texture>("None");
                }
            }
        }

        public void UpdateInventory(GameObject inventoryView, int fromLine, int toLine, int startItemInArr, int countInLineItems)
        {             
            for (int i = fromLine, j = startItemInArr; i <= toLine; i++, j += countInLineItems)
            {
                UpdateInventoryLine(inventoryView.transform.Find("LineItems" + i).gameObject, j, countInLineItems);
            }   
        }


        //ClickListneers
        public void SetInventoryOnclicksListneer(GameObject inventoryView, GameObject toolBar, int startLine, int EndLine)
        {
            GameObject[] lines = new GameObject[EndLine + 1];
            lines[0] = toolBar;

            for (int i = 1, j = startLine; j <= EndLine; i++, j++)
                lines[i] = inventoryView.transform.Find("LineItems" + j).gameObject;

            for (int j = 0; j < lines.Length; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject item = lines[j].transform.Find("Item" + i).gameObject;
                    setSelectLineItemsListneer(item, int.Parse(j + "" + i));
                }
            }

        }

        public void SetInventoryOnclicksListneer(GameObject inventoryView, int startLine, int EndLine)
        {
            GameObject[] lines = new GameObject[EndLine - startLine + 1];

            for (int i = 0, j = startLine; j <= EndLine; i++, j++)
                lines[i] = inventoryView.transform.Find("LineItems" + j).gameObject;

            for (int j = 0; j < lines.Length; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    GameObject item = lines[j].transform.Find("Item" + i).gameObject;
                    setSelectLineItemsListneer(item, int.Parse(j + "" + i));
                }
            }

        }

        public void setSelectLineItemsListneer(GameObject item, int index)
        {
            EventTrigger eventTrigger = item.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { ItemSelect(index); });
            eventTrigger.triggers.Add(entry);        
        }

    }

}
