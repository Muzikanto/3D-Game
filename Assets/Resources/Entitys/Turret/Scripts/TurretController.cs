using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Shells;
using Assets.Other;

namespace Assets.Turret
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] bool isActive = false;
        [SerializeField] bool moveToGround = true;
        [SerializeField] GameObject tower;
        [SerializeField] GameObject burrelHolder;
        [SerializeField] GameObject[] startPositionShells;
        [SerializeField] GameObject raycaster;
        [SerializeField] float radiusSearch = 60f;
        [SerializeField] float delayShoot = 0.5f;
        [SerializeField] float speedRotate = 0.01f;
        [SerializeField] float maxUpAngle = 30f;
        [SerializeField] float maxDownAngle = 40f;
        [SerializeField] GameObject shell;
        [SerializeField] ShellInfo shellInfo = new ShellInfo(20, 0.2f, 100, 100);
        [SerializeField] Tags.Tag[] targetsTags;


        private void Start()
        {
            radiusSearch /= 5;
            if (isActive)
                StartCoroutine("CheckSphereOverlap");
            transform.rotation = Quaternion.Euler(Vector3.zero);

            Ray ray = new Ray(transform.position, Vector3.down);

            while (moveToGround)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
                foreach (Collider c in colliders)
                {
                    if (c.tag == "Block" || c.tag == "Solid" || c.tag == "Vehicles")
                        moveToGround = false;
                }
                transform.position = transform.position + Vector3.down * Time.deltaTime;
            }
        }

        private IEnumerator CheckSphereOverlap()
        {
            int layerMask = 1 << 10;
            while (true)
            {               
                Collider[] colliders = Physics.OverlapSphere(transform.position, radiusSearch, layerMask);

                foreach (var c in colliders)
                {
                    if (c == null)
                        continue;
                    foreach (var targetTag in targetsTags)
                    {
                        if (c.tag == targetTag.ToString())
                        {
                            if (!findBarrier(c) && c != null)
                            {
                                Quaternion targetRot = Quaternion.LookRotation((c.transform.position - burrelHolder.transform.position).normalized);
                                targetRot = new Quaternion(targetRot.z, targetRot.y, -targetRot.x, targetRot.w);

                                float factor = 0f;
                                while (factor < 1f)
                                {
                                    tower.transform.localRotation = Quaternion.Slerp(tower.transform.localRotation, targetRot, factor);
                                    tower.transform.localEulerAngles = new Vector3(0, tower.transform.localEulerAngles.y, tower.transform.localEulerAngles.z);
                                    factor += speedRotate;
                                    yield return new WaitForSeconds(speedRotate);
                                }


                                float dist = Vector3.Distance(startPositionShells[0].transform.position, c.transform.position);

                                foreach (GameObject ob in startPositionShells)
                                {
                                    Debug.DrawRay(raycaster.transform.position, raycaster.transform.forward * radiusSearch, Color.red, delayShoot);
                                    if (c == null)
                                        break;
                                    Shot(dist, ob.transform.position, Quaternion.LookRotation((c.transform.position - burrelHolder.transform.position).normalized));
                                    yield return new WaitForSeconds(0.2f);
                                }
                                yield return new WaitForSeconds(delayShoot);
                            }
                            break;
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }

        private bool findBarrier(Collider c)
        {
            raycaster.transform.LookAt(c.transform.position);

            float angle = raycaster.transform.eulerAngles.x;
            if (!((angle > 0 && angle < maxDownAngle) || (angle > 360 - maxUpAngle && angle < 360)))
                return true;

            RaycastHit[] hits = Physics.RaycastAll(raycaster.transform.position, raycaster.transform.forward, radiusSearch);
            bool barrier = false;
            foreach (var h in hits)
            {
                if (h.collider.tag == "Solid" || h.collider.tag == "Block")
                {                    
                    if (h.distance < Vector3.Distance(transform.position, c.transform.position))
                    {
                        barrier = true;
                        break;
                    }
                }
                else if (h.collider.tag == c.tag)
                    break;
            }
            return barrier;
        }
 
        private void Shot(float dist, Vector3 pos, Quaternion direction)
        {
            GameObject obj = Instantiate(shell, pos, direction);
            obj.SendMessage("slotShellMessage", shellInfo);
        }

        //SLOT
        public void setActive()
        {
            isActive = !isActive;
            if (isActive)
            {
                StartCoroutine("CheckSphereOverlap");
            }
            else
            {
                StopCoroutine("CheckSphereOverlap");
            }
        }
    }

}