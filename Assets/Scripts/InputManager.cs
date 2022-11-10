using UnityEngine;

public class InputManager : PersistentSingleton<InputManager>
{
    public float horizontal;
    public float vertical;
    public bool primaryDown;
    public bool primaryHeld;
    public bool primaryUp;
    public bool secondaryDown;
    public bool secondaryHeld;
    public bool secondaryUp;
    public Vector2 mousePosition;
    public float scrollDelta;
    public bool shiftHeld;

    public float primaryHoldTime;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        primaryDown = Input.GetMouseButtonDown(0);
        primaryHeld = Input.GetMouseButton(0);
        primaryUp = Input.GetMouseButtonUp(0);
        secondaryDown = Input.GetMouseButtonDown(1);
        secondaryHeld = Input.GetMouseButton(1);
        secondaryUp = Input.GetMouseButtonUp(0);
        mousePosition = Input.mousePosition;
        scrollDelta = Input.mouseScrollDelta.y;
        shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (primaryHeld) primaryHoldTime += Time.deltaTime;
        else if (primaryUp) primaryHoldTime = 0;
    }
}
