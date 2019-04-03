using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Other
{
    public class SendMessage_CamPos_And_PlayerPos
    {
        public Camera cam;
        public GameObject player;

        public SendMessage_CamPos_And_PlayerPos(Camera cam, GameObject player)
        {
            this.cam = cam;
            this.player = player;
        }
    }

    public class OtherMetods
    {
        public static void camTransformParent_camDropSettings(Camera cam, GameObject cameraContainer)
        {
            cam.transform.parent = cameraContainer.transform;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        public static Transform FindParent(Transform tr, string stop)
        {
            Transform res = tr.parent;
            while (res != null)
            {
                if (res.parent == null || res.parent.name == stop)
                {
                    break;
                }
 
                res = res.parent;
            }

            return res;
        }

        
    }

    public class Tags
    {
        [Serializable]
        [Flags]
        public enum Tag { Vehicles, Entity, Player };
    }

   

}
