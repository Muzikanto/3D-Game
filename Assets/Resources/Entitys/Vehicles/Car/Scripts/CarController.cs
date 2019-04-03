using System;
using UnityEngine;

namespace Assets.Vehicles.Cars
{
    internal enum SpeedType
    {
        MPH,
        KPH
    }

    public class CarController : MonoBehaviour
    {
        [SerializeField] private AxleInfo[] axleInfos;
        [SerializeField] private WheelMeshes[] wheelMeshes;
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper; // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private SpeedType m_SpeedType;
        [SerializeField] private float m_Topspeed = 200;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;

    //    private TwoQuat[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle{ get { return m_SteerAngle; }}
        public float CurrentSpeed{ get { return m_Rigidbody.velocity.magnitude*2.23693629f; }}
        public float MaxSpeed{get { return m_Topspeed; }}
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }

        // Use this for initialization
        private void Start()
        {
         //   m_WheelMeshLocalRotations = new TwoQuat[axleInfos.Length];
            for (int i = 0; i < axleInfos.Length; i++)
            {
              //  m_WheelMeshLocalRotations[i].left = axleInfos[i].leftWheel.transform.localRotation;           
              //  m_WheelMeshLocalRotations[i].right = axleInfos[i].rightWheel.transform.localRotation;
            }
            axleInfos[0].leftWheel.attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl*m_FullTorqueOverAllWheels);
        }


        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed/MaxSpeed);
            float upgearlimit = (1/(float) NoOfGears)*(m_GearNum + 1);
            float downgearlimit = (1/(float) NoOfGears)*m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor)*(1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value)*from + value*to;
        }


        private void CalculateGearFactor()
        {
            float f = (1/(float) NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f*m_GearNum, f*(m_GearNum + 1), Mathf.Abs(CurrentSpeed/MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime*5f);
        }


        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum/(float) NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }


        public void Move(float steering, float accel, float footbrake, float handbrake)
        {

            for (int i = 0; i < wheelMeshes.Length; i++)
            {
                Quaternion quatL;
                Quaternion quatR;
                Vector3 positionL;
                Vector3 positionR;
                axleInfos[i].leftWheel.GetWorldPose(out positionL, out quatL);
                axleInfos[i].rightWheel.GetWorldPose(out positionR, out quatR);
                wheelMeshes[i].leftWheel.transform.position = positionL;
                wheelMeshes[i].rightWheel.transform.position = positionR;
                wheelMeshes[i].leftWheel.transform.rotation = quatL;
                wheelMeshes[i].rightWheel.transform.rotation = quatR;

            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput = footbrake = -1*Mathf.Clamp(footbrake, -1, 0);
            handbrake = Mathf.Clamp(handbrake, 0, 1);

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering*m_MaximumSteerAngle;
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = m_SteerAngle;
                    axleInfo.rightWheel.steerAngle = m_SteerAngle;
                }
            }

            SteerHelper();
            ApplyDrive(accel, footbrake);
            CapSpeed();

            //Set the handbrake.
            //Assuming that wheels 2 and 3 are the rear wheels.
    
            if (handbrake > 0f)
            {
                var hbTorque = handbrake*m_MaxHandbrakeTorque;
                foreach(AxleInfo axleInfo in axleInfos)
                {
                    if (axleInfo.motor)
                    {
                        axleInfo.leftWheel.brakeTorque = hbTorque;
                        axleInfo.rightWheel.brakeTorque = hbTorque;
                    }
                }                
            }


            CalculateRevs();
            GearChanging();

            AddDownForce();
            CheckForWheelSpin();
            TractionControl();
        }


        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
            switch (m_SpeedType)
            {
                case SpeedType.MPH:

                    speed *= 2.23693629f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/2.23693629f) * m_Rigidbody.velocity.normalized;
                    break;

                case SpeedType.KPH:
                    speed *= 3.6f;
                    if (speed > m_Topspeed)
                        m_Rigidbody.velocity = (m_Topspeed/3.6f) * m_Rigidbody.velocity.normalized;
                    break;
            }
        }

        
        private void ApplyDrive(float accel, float footbrake)
        {

            float thrustTorque;        
            thrustTorque = accel * (m_CurrentTorque / axleInfos.Length * 2);
            for (int i = 0; i < axleInfos.Length; i++)
            {
                    axleInfos[i].leftWheel.motorTorque = axleInfos[i].rightWheel.motorTorque = thrustTorque;
            }

            for (int i = 0; i < axleInfos.Length; i++)
            {
                if (CurrentSpeed > 5 && Vector3.Angle(transform.forward, m_Rigidbody.velocity) < 50f)
                {
                    axleInfos[i].leftWheel.brakeTorque = axleInfos[i].rightWheel.brakeTorque = m_BrakeTorque * footbrake;
                }
                else if (footbrake > 0)
                {
                    axleInfos[i].leftWheel.brakeTorque = axleInfos[i].rightWheel.brakeTorque = 0f;
                    axleInfos[i].leftWheel.motorTorque = axleInfos[i].rightWheel.motorTorque = -m_ReverseTorque * footbrake;
                }
            }
        }
        

        private void SteerHelper()
        {
            for (int i = 0; i < axleInfos.Length; i++)
            {
                WheelHit wheelhitL;
                WheelHit wheelhitR;
                axleInfos[i].leftWheel.GetGroundHit(out wheelhitL);
                axleInfos[i].rightWheel.GetGroundHit(out wheelhitR);
                if (wheelhitL.normal == Vector3.zero || wheelhitR.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            axleInfos[0].leftWheel.attachedRigidbody.AddForce(-transform.up*m_Downforce*
                                                         axleInfos[0].leftWheel.attachedRigidbody.velocity.magnitude);
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < axleInfos.Length; i++)
            {
                WheelHit wheelHitL;
                WheelHit wheelHitR;
                if (axleInfos[i].motor)
                {
                    axleInfos[i].leftWheel.GetGroundHit(out wheelHitL);
                    axleInfos[i].rightWheel.GetGroundHit(out wheelHitR);
                }
            }
        }
        

        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHitL;
            WheelHit wheelHitR;
            for (int i = 0; i < axleInfos.Length; i++)
            {
                    axleInfos[i].leftWheel.GetGroundHit(out wheelHitL);
                    axleInfos[i].rightWheel.GetGroundHit(out wheelHitR);
                    AdjustTorque(wheelHitL.forwardSlip);
                    AdjustTorque(wheelHitR.forwardSlip);
            }
        }


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }
     
        

        [Serializable]
        public class AxleInfo
        {
            public WheelCollider leftWheel;
            public WheelCollider rightWheel;
            public bool steering = false;
            public bool motor = false;
        }

        [Serializable]
        public class WheelMeshes
        {
            public GameObject leftWheel;
            public GameObject rightWheel;
        }

        public class TwoQuat
        {
            public Quaternion left;
            public Quaternion right;
        }
    }
}
