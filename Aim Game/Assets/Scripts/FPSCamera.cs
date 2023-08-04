using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity;
    private float rotationX;   

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
        rotationX -= input.y;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.localEulerAngles = Vector3.right * rotationX;


        player.Rotate(Vector3.up * input.x);


    }
}
