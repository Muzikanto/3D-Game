using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Inventorys;

namespace Assets.Other
{
    public class MyTerrain : MonoBehaviour
    {
        public NavMeshSurface surface;
        public GameObject thisGameObject;

        private bool surfaceIsCreated = false;

        void Update()
        {
            if (!surfaceIsCreated && CreateMaze.isCreated)
            {
                surface.BuildNavMesh();
                surfaceIsCreated = true;
            }
        }

        public static GameObject CreateBlock(int id, Vector3 pos)
        {

            GameObject res = Resources.Load<GameObject>(Id.blocks[id].pathToPrefab);
            GameObject obj = Instantiate(res, pos, Quaternion.identity);
            obj.name = Id.setNameObj(id, Id.TypeItem.Block);
            obj.transform.parent = GameObject.Find("Terrain/Blocks").gameObject.transform;
            return obj;
        }

        public static GameObject CreateEntity(int id, Vector3 pos, Quaternion rot)
        {
            GameObject res = Resources.Load<GameObject>(Id.entitys[id].pathToPrefab);
            GameObject obj = Instantiate(res, pos, rot);
            obj.name = Id.setNameObj(id, Id.TypeItem.Entity);
            obj.transform.parent = GameObject.Find("Terrain/Entitys").gameObject.transform;
            return obj;
        }

    }

}

