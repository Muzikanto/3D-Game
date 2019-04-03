using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Assets.Other;
using System.Collections;
using System.Collections.Generic;
using Assets.Shells;
using Assets.Vehicles;

namespace Assets.Vehicles.Cars
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : BaseUserVehicleController
    {
        private CarController m_Car; // the car controller we want to use
        private InterfaceViews interfaceViews = new InterfaceViews();


        //----------------------------------------------
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();

            CreateInterface();
            interfaceViews.speedView = vehicleInterface.transform.Find("Speed").gameObject;
            interfaceViews.healthView = vehicleInterface.transform.Find("Health").gameObject;
        }

        private void Update()
        {
            CheckDestroy_HP();
            if (isControll)
            {              
                checkEject();
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

        //----------------------------------------------
        public void updateInterface()
        {           
            interfaceViews.speedView.GetComponent<UnityEngine.UI.Text>().text = "Speed: " + (int)m_Car.CurrentSpeed;
            interfaceViews.healthView.GetComponent<UnityEngine.UI.Text>().text = "Health: " + health;
        }

        public class InterfaceViews
        {
            public GameObject speedView;
            public GameObject healthView;
        }

    }

}
