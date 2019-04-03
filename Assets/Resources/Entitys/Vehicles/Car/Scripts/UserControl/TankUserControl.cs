using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Assets.Other;
using Assets.Shells;
using Assets.Vehicles;

namespace Assets.Vehicles.Cars
{
    [RequireComponent(typeof(CarController))]
    public class TankUserControl : BaseUserVehicleController
    {
        //----------------------------------------------
        [SerializeField] Stats stats = new Stats();
        [SerializeField] ShellInfo shellInfo = new ShellInfo(1.0f, 0.5f, 40f, 300);
        [SerializeField] TankParts tankParts = new TankParts();
        [SerializeField] GameObject shell;


        //----------------------------------------------

        //----------------------------------------------
        private CarController m_Car; // the car controller we want to use
        private bool nextShoot = true;
        private InterfaceViews interfaceViews = new InterfaceViews();
        private float rotateY = 0, rotateX = 0;
        private float maxUpUVN = -20, maxDownUVN = 15;
        //----------------------------------------------

        private void Awake()
        {         
            CreateInterface();
            // get the car controller
            m_Car = GetComponent<CarController>();
            
            interfaceViews.healthView = vehicleInterface.transform.Find("Health").gameObject;
            interfaceViews.ammoView = vehicleInterface.transform.Find("Ammo").gameObject;
            interfaceViews.speedView = vehicleInterface.transform.Find("Speed").gameObject;
            interfaceViews.reloadView = vehicleInterface.transform.Find("Reload").gameObject;
        }

        private void Update()
        {
            CheckDestroy_HP();
            if (isControll)
            {
                checkEject();
                fixedTurnAndLockCursor();
                checkShoot();
                updateInterface();
            }
        }

        private void FixedUpdate()
        {
            if (isControll)
            {
                // pass the input to the car!
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
            }
        }

        private void fixedTurnAndLockCursor()
        {
            if (!Cursor.visible)
            {
                float value = Input.GetAxis("Mouse Y");
                if ((rotateY >= maxUpUVN && rotateY <= maxDownUVN) || (rotateY >= maxDownUVN && value > 0) || (rotateY <= maxUpUVN && value < 0))
                {
                    rotateY -= value;
                    if (rotateY < maxUpUVN) rotateY = maxUpUVN;
                    if (rotateY > maxDownUVN) rotateY = maxDownUVN;
                    tankParts.barrelHolder.transform.localRotation = Quaternion.Euler(rotateY, 0, 0);
                }
                rotateX += Input.GetAxis("Mouse X");
                tankParts.tower.transform.localRotation = Quaternion.Euler(-90, rotateX, 0);
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Cursor.visible = false;
                    Screen.lockCursor = true;
                }
            }
        }

        public void updateInterface()
        {
            interfaceViews.speedView.GetComponent<UnityEngine.UI.Text>().text = "Speed: " +  (int)m_Car.CurrentSpeed;
            interfaceViews.ammoView.GetComponent<UnityEngine.UI.Text>().text = "Ammo: " + stats.ammo;
            interfaceViews.healthView.GetComponent<UnityEngine.UI.Text>().text = "Health: " + health;
        }
   
        public void checkShoot()
        {
            if (Input.GetKeyDown(KeyCode.Space) && nextShoot && stats.ammo > 0)
            {
                Vector3 origin = tankParts.startPosShell.transform.position;
                Vector3 direction = tankParts.startPosShell.transform.forward;
                GameObject obj = Instantiate(shell, origin, Quaternion.LookRotation(direction));
                obj.SendMessage("slotShellMessage", shellInfo);
                stats.ammo--;
                StartCoroutine("CourDelayNextShoot");
            }
        }

        private IEnumerator CourDelayNextShoot()
        {
            nextShoot = false;
            float currentDelayTime = stats.delayNextShoot;
            while(currentDelayTime >= 0.2f)
            {               
                yield return new WaitForSeconds(0.2f);
                currentDelayTime -= 0.2f;
                interfaceViews.reloadView.GetComponent<UnityEngine.UI.Text>().text = "Reload: " + currentDelayTime;
            }
            interfaceViews.reloadView.GetComponent<UnityEngine.UI.Text>().text = "Reload: Ready!";
            nextShoot = true;
        }

        //----------------------------------------------



        //----------------------------------------------
        [Serializable]
        public class InterfaceViews
        {
            public GameObject speedView;
            public GameObject healthView;
            public GameObject ammoView;
            public GameObject reloadView;
        }

        [Serializable]
        public class TankParts
        {
            public GameObject tower;
            public GameObject barrelHolder;
            public GameObject startPosShell;
        }

        [Serializable]
        public class Stats
        {
            public float ammo = 100;
            public float delayNextShoot = 1.0f;
        }
        //----------------------------------------------
    }
}