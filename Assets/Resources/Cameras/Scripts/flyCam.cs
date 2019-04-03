using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Cam
{
    class flyCam : MonoBehaviour
    {
        public float speed = 1;
        private float xR = 0, yR = 0;

        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
                transform.position = transform.position + transform.forward * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S))
                transform.position = transform.position - transform.forward * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                transform.position = transform.position + transform.right * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                transform.position = transform.position - transform.right * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Q))
                transform.position = transform.position + transform.up * speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Z))
                transform.position = transform.position - transform.up * speed * Time.deltaTime;

            xR += Input.GetAxis("Mouse X");
            yR -= Input.GetAxis("Mouse Y");
            transform.rotation = Quaternion.Euler(yR, xR, 0);
        }
    }
}
