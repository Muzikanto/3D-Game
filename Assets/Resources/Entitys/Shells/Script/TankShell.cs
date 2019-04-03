using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Vehicles.Cars;
using System;
using Assets.Other;

namespace Assets.Shells
{
    public class TankShell : MonoBehaviour
    {
        public GameObject explosionEfects;

        private ShellInfo shellInfo;
        private bool isActive = false;
        private Vector3 startPos;
        Rigidbody rBody;
        private bool isExplosion = false;

        void Update()
        {
            if (isActive)
            {
                if (Mathf.Abs(Vector3.Distance(startPos, transform.position)) < shellInfo.maxDistanceMove)
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, shellInfo.speed * shellInfo.speed);
                else StartCoroutine("explosion");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
                StartCoroutine("explosion");
        }

        IEnumerator explosion()
        {
            if (!isExplosion)
            {
                isExplosion = true;
                GameObject obj = Instantiate(explosionEfects, transform.position, Quaternion.identity);

                foreach (Collider item in Physics.OverlapSphere(transform.position, shellInfo.radiusDestroy))
                {
                    if (item.tag == "Block")
                        Destroy(item.gameObject);
                    else if (item.tag == "Vehicles")
                    {
                        Transform parent = item.transform.parent.transform.parent;
                        parent.gameObject.SendMessage("isShotted", shellInfo);
                    }
                }
                yield return new WaitForSeconds(1f);
                Destroy(obj);
                Destroy(gameObject);
            }
        }

        //Slot start
        public void slotShellMessage(ShellInfo shellInfoV)
        {
            shellInfo = shellInfoV;
            isActive = true;
            startPos = transform.position;
            rBody = gameObject.GetComponent<Rigidbody>();
        }


    }

    [Serializable]
    public class ShellInfo
    {
        public ShellInfo(float speed, float radiusDestroy, float maxDistanceMove, int destHp)
        {
            this.speed = speed;
            this.radiusDestroy = radiusDestroy;
            this.maxDistanceMove = maxDistanceMove;
            this.destHp = destHp;
        }
        public float speed;
        public float radiusDestroy;
        public float maxDistanceMove;
        public int destHp;
    }

}