using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn_object : MonoBehaviour
{
    [SerializeField] public Transform Spawnpoint;
    [SerializeField] public GameObject Prefab;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject.tag == "VRRightHand")
        {
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
        }

    }

}
