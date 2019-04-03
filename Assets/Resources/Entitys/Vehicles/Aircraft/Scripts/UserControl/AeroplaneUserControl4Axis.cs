using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Assets.Other;
using Assets.Shells;
using Assets.Vehicles;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof (AeroplaneController))]
    [RequireComponent(typeof(LandingGear))]
    public class AeroplaneUserControl4Axis : BaseUserVehicleController
    {
        // these max angles are only used on mobile, due to the way pitch and roll input are handled
        [SerializeField] float maxRollAngle = 80;
        [SerializeField] float maxPitchAngle = 80;


        // reference to the aeroplane that we're controlling
        private AeroplaneController m_Aeroplane;
        private LandingGear m_gear;
        private float m_Throttle;
        private bool m_AirBrakes;
        private float m_Yaw;

        private InterfaceViews interfaceViews = new InterfaceViews();

        //-------------------------------
        private void Awake()
        {
            CreateInterface();

            // Set up the reference to the aeroplane controller.
            m_Aeroplane = GetComponent<AeroplaneController>();
            m_gear = GetComponent<LandingGear>();

            interfaceViews.speedView = vehicleInterface.transform.Find("Speed").gameObject;
            interfaceViews.healthView = vehicleInterface.transform.Find("Health").gameObject;
        }

        private void Update()
        {
            CheckDestroy_HP();
            if (isControll)
            {
                checkEject();
                UpdateInterface();
            }
        }

        private void FixedUpdate()
        {
            if (isControll)
            {
                // Read input for the pitch, yaw, roll and throttle of the aeroplane.
                float roll = CrossPlatformInputManager.GetAxis("Mouse X");
                float pitch = CrossPlatformInputManager.GetAxis("Mouse Y");
                m_AirBrakes = CrossPlatformInputManager.GetButton("Fire1");
                m_Yaw = CrossPlatformInputManager.GetAxis("Horizontal");
                m_Throttle = CrossPlatformInputManager.GetAxis("Vertical");

                // Pass the input to the aeroplane
                m_Aeroplane.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
                m_gear.MoveWheels(m_Yaw, m_Throttle);
            }
        }
        //-------------------------------

        public void UpdateInterface()
        {
            interfaceViews.speedView.GetComponent<UnityEngine.UI.Text>().text = "Speed: " + (int)m_Aeroplane.ForwardSpeed;
            interfaceViews.healthView.GetComponent<UnityEngine.UI.Text>().text = "Health: " + health;
        }
        //-------------------------------

        [Serializable]
        public class InterfaceViews
        {
            public GameObject healthView;
            public GameObject speedView;
        }
    }
}
