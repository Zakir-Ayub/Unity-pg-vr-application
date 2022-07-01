using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Can be used by GrabInteractable objects to teleport objects on or in them.
/// For this all chemicals inside the radius of the center position are consiedered.
/// </summary>
public class TeleportEnhancer : MonoBehaviour
{
    [SerializeField]
    Transform center;

    [SerializeField]
    float radius = 0.1f;

    [SerializeField]
    LayerMask interactionLayer = (1 << 0);

    Collider[] currentObjects;
    bool isTeleporting = false;
    Vector3 lastPosition;



    void Start()
    {
        // subscribe to Teleport events
        var tp_areas = FindObjectsOfType <TeleportationArea>();
        foreach (var tp_area in tp_areas)
            tp_area.teleporting.AddListener(OnTeleport);
    }

    private void OnTeleport(TeleportingEventArgs args)
    {
        var colliders = Physics.OverlapSphere(center.position, radius, interactionLayer, QueryTriggerInteraction.Ignore);
        // consider chemicals
        currentObjects = colliders.Where(x => x.CompareTag("Chemical") && x.GetComponent<Rigidbody>() != null).ToArray();
        lastPosition = center.position;
        isTeleporting = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isTeleporting)
            return;
        Vector3 diff = center.position - lastPosition;
        if (diff.magnitude == 0f)
            return;

        foreach(var o in currentObjects)
        {
            o.transform.position += diff;
        }
        isTeleporting = false;
        currentObjects = null;
    }
}
