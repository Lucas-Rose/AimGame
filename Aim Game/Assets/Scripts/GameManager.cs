using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TargetDispenser dispenser;
    [SerializeField] private FPSCamera fpsCamera;

    // Start is called before the first frame update
    public void HitTarget(RaycastHit hit)
    {
        fpsCamera.ToggleCanLook(true);
        
        Destroy(hit.transform.gameObject);
        dispenser.DispenseTarget();
    }
}
