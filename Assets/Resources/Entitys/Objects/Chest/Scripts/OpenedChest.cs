using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Inventorys;
using Assets.Objects.Storage;

public class OpenedChest : Storage
{
	[SerializeField] float velocity = 100f;
    [SerializeField] GameObject target;
    [SerializeField] GameObject pivot;



    private GameObject alvo;
    private bool abrir = false;
    private bool cheio = true;

    new private void Awake()
    {
        base.Awake();
        for(int i = 0; i < 50; i++)
        {
            int id = Random.Range(0, Id.entitys.Length - 1);
            if(Random.Range(0f, 1f) < Id.entitys[id].chance)
                inventoryExchangeItems.inventory.AddItemInventory(new ItemInventory(id, 1, Id.TypeItem.Entity));
        }     
    }

    new void Update () {

        if (abrir)
        {
            if (pivot.transform.rotation.x > -0.9)
            {
                pivot.transform.Rotate(new Vector3(-velocity * Time.deltaTime * 2, 0.0f, 0.0f));
                if (pivot.transform.rotation.x < -0.45 && cheio == true)
                {
                    cheio = false;
                }
            }
        }
        else
        {
            if (pivot.transform.rotation.x < 0)
            {
                pivot.transform.Rotate(new Vector3(velocity * Time.deltaTime * 2, 0.0f, 0.0f));
            }
        }
        base.Update();
    }

	void OnTriggerStay(Collider other) { 

		if (alvo == null && abrir == false) {
			Vector3 pos = pivot.transform.position;
			pos.y += 0.5f;
			pos.z += 0.25f;
			alvo = Instantiate (target, pos, Quaternion.identity) as GameObject;
            alvo.transform.SetParent(transform);

        }
	}

	void OnTriggerExit(Collider other) {
		Destroy (alvo.gameObject);
		abrir = false;
	}

    //SLOT
    new public void OpenChest(GameObject playerV)
    {
        base.OpenChest(playerV);
        abrir = true;
        if(alvo != null)
         Destroy(alvo.gameObject);     
    }


    
}
