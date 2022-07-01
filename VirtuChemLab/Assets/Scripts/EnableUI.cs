using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class EnableUI : MonoBehaviour
{
    [SerializeField]
    InputActionReference action;

    // Event delegates triggered on click.
    [SerializeField]
    private UnityEvent onInteract = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        EnableAction(action);
    }

    // Update is called once per frame
    void Update()
    {
        bool activate = action != null && action.action.triggered;

        if (activate)
        {
            onInteract.Invoke();
        }
    }

    static void EnableAction(InputActionReference actionReference)
    {
        var action = actionReference.action;
        if (action != null && !action.enabled)
            action.Enable();
    }
}
