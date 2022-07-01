using Unity.Netcode;
using UnityEngine;


public class DevicePositionSyncer : NetworkBehaviour
{

    private Transform lastTransform;

    void Start()
    {
        lastTransform = transform;
    }

    void Update()
    {
        if (lastTransform != transform)
        {
            SetTransform(transform.position, transform.eulerAngles);
            lastTransform = transform;
        }
    }

    [ServerRpc]
    private void SetTransformServerRpc(Vector3 position, Vector3 rotation)
    {
        if (IsServer && IsOwner)
        {
            transform.position = position;
            transform.eulerAngles = rotation;
        }
    }

    private void SetTransform(Vector3 position, Vector3 rotation)
    {
        if (!IsServer)
        {
            SetTransformServerRpc(position, rotation);
        }
    }

}
