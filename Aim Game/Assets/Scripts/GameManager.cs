using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TargetDispenser dispenser;
    [SerializeField] private FPSCamera fpsCamera;
    [SerializeField] private List<float> scoreList;
    [SerializeField] private Transform scoreContainer;
    [SerializeField] private TMP_Text avgText;
    private float currTime;
    private float spawnTime;
    private float shootTime;


    private void Start()
    {
        spawnTime = currTime;
    }
    // Start is called before the first frame update
    public void HitTarget(RaycastHit hit)
    {
        fpsCamera.ToggleCanLook(true);
        Destroy(hit.transform.gameObject);
        UpdateScore();
        dispenser.DispenseTarget();
    }

    private void Update()
    {
        currTime += Time.deltaTime;
    }

    public void UpdateScore()
    {
        if (scoreList.Count < scoreContainer.childCount)
        {
            shootTime = currTime;
            scoreList.Add(shootTime - spawnTime);
            scoreContainer.GetChild(scoreList.Count - 1).GetComponent<TMP_Text>().text += " " + scoreList[scoreList.Count - 1].ToString("f3");
            spawnTime = currTime;
            avgText.text = "Average: " + (getScoreSum() / scoreList.Count).ToString("f3");
        }
    }

    public float getScoreSum()
    {
        float ret = 0;
        for(int i = 0; i < scoreList.Count; i++)
        {
            ret += scoreList[i];
        }
        return ret;
    }
}
