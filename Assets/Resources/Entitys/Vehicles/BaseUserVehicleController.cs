using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Other;
using Assets.Shells;
using Assets.Inventorys;

namespace Assets.Vehicles
{
    public class BaseUserVehicleController : MonoBehaviour
    {
        [SerializeField] protected int health = 1000;
        [SerializeField] public GameObject cameraContainer;
        [SerializeField] public GameObject vehicleInterface;
        //----------------------------------------------

        protected bool isControll = false;
        protected Camera cam;
        protected GameObject player = null;


        //----------------------------------------------

        public void CreateInterface()
        {
            GameObject interf = Instantiate(vehicleInterface);
            interf.transform.SetParent(transform);
            vehicleInterface = interf;
            vehicleInterface.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            vehicleInterface.SetActive(false);
        }

        public void checkEject()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Eject();
            }       
        }

        public void CheckDestroy_HP()
        {
            if (health <= 0)
            {
                if (player != null)
                    Eject();
                Destroy(gameObject);
            }
        }

        public void Eject()
        {
            isControll = false;
            vehicleInterface.SetActive(false);
            player.SetActive(true);
            player.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            player.SendMessage("setIsControll", new SendMessage_CamPos_And_PlayerPos(cam, player));
            player = null;
        }

        //----------------------------------------------

        //SLOT
        public void setIsControll(SendMessage_CamPos_And_PlayerPos mess)
        {
            isControll = true;
            cam = mess.cam;
            OtherMetods.camTransformParent_camDropSettings(cam, cameraContainer);
            player = mess.player;
            vehicleInterface.SetActive(true);
        }

        //SLOT
        public void isShotted(ShellInfo shellInfo)
        {
            health -= shellInfo.destHp;
            
        }

        //----------------------------------------------
      
    }
}
