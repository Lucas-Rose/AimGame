using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private GameObject gameManager;
    private GameManager gManager;
    private float rotationX;
    private bool canLook;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gManager = gameManager.GetComponent<GameManager>();
        canLook = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canLook)
        {
            Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
            rotationX -= input.y;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            player.Rotate(Vector3.up * input.x);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (canLook)
            {
                Shoot();
            }   
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleCanLook(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, float.PositiveInfinity))
        {
            if (hit.transform.gameObject.CompareTag("Target"))
            {
                gManager.HitTarget(hit);
            }
            else
            {
                gManager.AddMiss();
            }
        }
    }

    public void ToggleCanLook(bool state)
    {
        canLook = state;
    }
    public void ResetRotation()
    {
        rotationX = 0;
        player.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
