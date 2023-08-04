using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDispenser : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance;

    private enum DispenseMode { 
    OnScreen,
    OffScreen
    }

    private DispenseMode dispensorMode;

    // Start is called before the first frame update
    void Start()
    {
        dispensorMode = DispenseMode.OnScreen;
        DispenseTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DispenseTarget()
    {
        switch (dispensorMode)
        {
            case (DispenseMode.OnScreen):
                Instantiate(targetPrefab, new Vector3(Random.Range(-distance + 0.5f, distance + 0.5f), Random.Range(0, distance - 1), distance), Quaternion.identity);
                break;
            case (DispenseMode.OffScreen):
                break;
        }
    }
}
