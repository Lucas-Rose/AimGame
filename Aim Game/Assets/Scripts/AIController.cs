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
    private int shotTotal;

    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject missIndicator;
    [SerializeField] private GameManager gm;

    [Header("Parameters")]
    [SerializeField] private float reactionTime; //from idle to rotating towards target
    [SerializeField] private float aimingGradient;
    [SerializeField] private float missGradient;
    [SerializeField] private float xAccuracy;
    [SerializeField] private float yAccuracy;

    private Transform activeTarget;


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
            if (currTime > reactionTime && shotPoints.Count != 0)
            {
                reacted = true;
            }
            else
            {
                reacted = false;
            }
            if (reacted)
            {
                movingTime += Time.deltaTime;
                var bodyLookPos = shotPoints[0] - transform.position;
                var camLookPos = shotPoints[0] - cameraTransform.position;
                Quaternion bodyRot = Quaternion.LookRotation(bodyLookPos);
                bodyRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, bodyRot.eulerAngles.y, transform.rotation.eulerAngles.z);
                Quaternion camRot = Quaternion.LookRotation(camLookPos);
                camRot.eulerAngles = new Vector3(camRot.eulerAngles.x, cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, bodyRot, Time.deltaTime * 15 * shotPoints[0].z / shotTotal);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, camRot, Time.deltaTime * 15 * shotPoints[0].z / shotTotal);
                cameraTransform.rotation = Quaternion.Euler(cameraTransform.localEulerAngles.x, transform.localEulerAngles.y, 0);

                if (movingTime > aimingGradient * 5 * shotPoints[0].z / shotTotal)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, float.PositiveInfinity))
                    {
                        if (hit.transform.gameObject.CompareTag("Target"))
                        {
                            gm.HitTarget(hit);
                        }
                    }
                    shotPoints.RemoveAt(0);
                    movingTime = 0;
                }
                if(shotPoints.Count == 0)
                {
                    reacted = false;
                }
            }   
        }
    }

    public void StartAiming(Transform targetPos)
    {
        activeTarget = targetPos;

        float accuracyMeasure = activeTarget.position.z * missGradient;

        float numberOfMiss = 0;
        numberOfMiss += (accuracyMeasure >= 1) ? Mathf.Floor(accuracyMeasure) : 0;

        float remainderChance = Mathf.Floor(accuracyMeasure) >= 1 ? accuracyMeasure / 10f : accuracyMeasure;

        numberOfMiss += (Random.value > remainderChance) ? 1 : 0;

        GenerateShotVectors(activeTarget.position, (int)numberOfMiss, activeTarget.position.z);
        currTime = 0;
    }

    public void GenerateShotVectors(Vector3 target, int n, float dist)
    {
        shotPoints = new List<Vector3>();
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
            //Instantiate(missIndicator, new Vector3(shotPoints[shotPoints.Count - 1].x, shotPoints[shotPoints.Count - 1].y, dist), Quaternion.identity);
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

    public void ResetRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        cameraTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
