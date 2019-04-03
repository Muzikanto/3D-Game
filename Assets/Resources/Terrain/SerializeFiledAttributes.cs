using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Assets.SerializeFieldTest
{

    class SerializeFiledAttributes : MonoBehaviour
    {
        [Tooltip("ToolTip")]
        [ContextMenuItem("Reset", "Function")]
        [Multiline(2)]
        public string playerBiography = "";

        [ColorUsage(true)]
        public Color color;

        [Delayed(order = 5)]
        public int Delayed = 0;

        [Header("Health Settings")]
        public int header = 0;

        [Space(10)]

        [Range(0,1)]
        public float range = 0;

        [SerializeField]
        public ValuesEnum myEnum;
     

        public void Function()
        {
            Debug.Log("X");
        }




        [Serializable]
        public class test
        {
            public int i = 0;
            public int j = 0;
        }

        public enum ValuesEnum
        {
            value1,value2
        }

        [CustomEditor(typeof(SerializeFiledAttributes))]
        public class ObjectBuilderEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                SerializeFiledAttributes myScript = (SerializeFiledAttributes)target;
                if (GUILayout.Button("Function"))
                {
                    myScript.Function();
                }
            }
        }
    }

}
