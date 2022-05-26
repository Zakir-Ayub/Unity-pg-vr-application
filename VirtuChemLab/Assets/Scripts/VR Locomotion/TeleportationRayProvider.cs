using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationRayProvider : MonoBehaviour
{
    // State transition actions
    [SerializeField]
    [Tooltip("The reference to the action of activating the teleport mode for this controller.")]
    InputActionReference m_TeleportModeActivate;
    /// <summary>
    /// The reference to the action of activating the teleport mode for this controller."
    /// </summary>
    public InputActionReference teleportModeActivate
    {
        get => m_TeleportModeActivate;
        set => m_TeleportModeActivate = value;
    }

    [SerializeField]
    public XRRayInteractor ray;

    // Start is called before the first frame update
    void Start()
    {
        EnableAction(m_TeleportModeActivate);
    }

    // Update is called once per frame
    void Update()
    {
        var teleportModeAction = m_TeleportModeActivate.action;

        var triggerTeleportMode = teleportModeAction != null && teleportModeAction.triggered;

        if (triggerTeleportMode)
            ray.enabled = !ray.enabled;            
    }

    static void EnableAction(InputActionReference actionReference)
    {
        var action = actionReference.action;
        if (action != null && !action.enabled)
            action.Enable();
    }
}
