using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private List<Vector3> shotPoints;
    private bool canMove;
    private bool reacted;
    private float currTime;
    private float movingTime;
    private float aimTime;
    private int shotTotal;

    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject missIndicator;

    [Header("Parameters")]
    [SerializeField] private float reactionTime; //from idle to rotating towards target
    [SerializeField] private float aimingGradient;
    [SerializeField] private float missGradient;
    [SerializeField] private float xAccuracy;
    [SerializeField] private float yAccuracy;
    [SerializeField] private float xRotationGrad;
    [SerializeField] private float yRotationGrad;

    [SerializeField] private Transform activeTarget;


    // Start is called before the first frame update
    void Start()
    {
        shotPoints = new List<Vector3>();
        canMove = false;
        reacted = false;
    }

    private void Update()
    {
        if (canMove)
        {
            currTime += Time.deltaTime;
            if(currTime > reactionTime)
            {
                reacted = true;
            }
            if (reacted)
            {
                movingTime += Time.deltaTime;
                Vector3 dir = shotPoints[0] - transform.position;
                float yStep = shotPoints[0].z * xRotationGrad * Time.deltaTime;
                float xStep = shotPoints[0].z * yRotationGrad * Time.deltaTime;
                Vector3 newX = Vector3.RotateTowards(transform.forward, dir, xStep, 0.0f);
                Vector3 newY = Vector3.RotateTowards(cameraTransform.forward, dir, yStep, 0.0f);
                cameraTransform.rotation = Quaternion.LookRotation(newY);
                transform.rotation = Quaternion.LookRotation(newX);
                if(movingTime > (aimingGradient * shotPoints[0].z) / shotTotal)
                {
                    shotPoints.RemoveAt(0);
                    if(shotPoints.Count == 0)
                    {

                    }
                }
            }   
        }
    }

    public void StartAiming(Transform targetPos)
    {
        activeTarget = targetPos;
        //cameraTransform.LookAt(new Vector3(cameraTransform.position.x, targetPos.y, targetPos.z));
        //transform.LookAt(new Vector3(targetPos.x, this.transform.position.y, targetPos.z));

        float accuracyMeasure = activeTarget.position.z * missGradient;

        float numberOfMiss = 0;
        numberOfMiss += (accuracyMeasure >= 1) ? Mathf.Floor(accuracyMeasure) : 0;

        float remainderChance = Mathf.Floor(accuracyMeasure) >= 1 ? accuracyMeasure / 10f : accuracyMeasure;

        numberOfMiss += (Random.value > remainderChance) ? 1 : 0;
        Debug.Log(numberOfMiss);

        GenerateShotVectors(activeTarget.position, (int)numberOfMiss, activeTarget.position.z);
    }

    public void GenerateShotVectors(Vector3 target, int n, float dist)
    {
        float offSetX;
        float offSetY;
        float xLoc;
        float yLoc;

        for (int i = 0; i < n; i++)
        {
            offSetX = Random.value > .5f ? Random.Range(-xAccuracy * 3, 0) : Random.Range(0, xAccuracy * 3);
            offSetY = Random.value > .5f ? Random.Range(-yAccuracy * 3, 0) : Random.Range(0, yAccuracy * 3);
            xLoc = target.x + offSetX;
            yLoc = target.y + offSetY;
            shotPoints.Add(new Vector3(xLoc, yLoc, dist));
            Instantiate(missIndicator, new Vector3(shotPoints[shotPoints.Count - 1].x, shotPoints[shotPoints.Count - 1].y, dist), Quaternion.identity);
        }
        offSetX = Random.value > .5f ? Random.Range(-xAccuracy, 0) : Random.Range(0, xAccuracy);
        offSetY = Random.value > .5f ? Random.Range(-yAccuracy, 0) : Random.Range(0, yAccuracy);
        xLoc = target.x + offSetX;
        yLoc = target.y + offSetY;
        shotPoints.Add(new Vector3(xLoc, yLoc, dist));
        shotTotal = shotPoints.Count;
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
    }
}
