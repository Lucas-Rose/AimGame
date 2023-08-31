using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDispenser : MonoBehaviour
{
    [Header("Transforms")]

    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [Header("Distance")]
    [SerializeField] private List<float> distances;

    private enum DispenseMode { 
    OnScreen,
    OffScreen
    }

    [SerializeField] private DispenseMode dispensorMode;

    private GameObject activeTarget;

    public void DispenseTarget()
    {
        switch (dispensorMode)
        {
            case (DispenseMode.OnScreen):
                activeTarget = Instantiate(targetPrefab,
                    new Vector3(Random.Range(-distances[0] + 0.5f, distances[0] + 0.5f),
                    Random.Range(0, distances[0] * 0.8f),
                    distances[0]),
                    Quaternion.Euler(new Vector3(0,0,0)));
                break;
            case (DispenseMode.OffScreen):
                activeTarget = Instantiate(targetPrefab,
                    new Vector3(Random.Range(-distances[0] + 0.5f, distances[0] + 0.5f),
                    Random.Range(0, -distances[0] * 0.8f),
                    distances[0]),
                    Quaternion.Euler(new Vector3(0, 180, 0)));
                break;
        }
    }

    public float NextDistance()
    {
        distances.RemoveAt(0);
        if(distances.Count > 0)
        {
            return distances[0];
        }
        return 0;
    }
    public float getActiveDistance()
    {
        return distances[0];
    }
    public int getRemainingRounds()
    {
        return distances.Count;
    }
    public bool GetOnScreen()
    {
        return dispensorMode == DispenseMode.OnScreen;
    }
    public GameObject GetTarget()
    {
        return activeTarget;
    }
}
