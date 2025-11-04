using UnityEngine;
using UnityEngine.InputSystem;
public class MoveCameraVertically : MonoBehaviour
{
    [Header("Rotate Player")]
    public InputActionAsset _action;
    public float mouseSensitivity;
    private Vector2 mouseDelta;
    private bool mouseClicked;
    private void rotatePlayer()
    {
        mouseDelta = Mouse.current.delta.ReadValue();
        transform.Rotate(-mouseDelta.y * mouseSensitivity * Time.deltaTime, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
        }
        if (mouseClicked)
        {
            rotatePlayer();
        }
    }
}
