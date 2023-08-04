using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private TargetDispenser dispenser;
    [SerializeField] private FPSCamera fpsCamera;
    private int round;

    [Header("Time Elements")]
    [SerializeField] private float maxWaitTime;
    [SerializeField] private Vector2 spawnDelayMinMax;
    private float waitTime;
    private float spawnDelay;
    private float currTime;
    private float spawnTime;
    private float shootTime;

    private enum GameState
    {
        Waiting,
        Spawning,
        Shooting
    }
    private GameState gameState;

    [Header("Text Elements")]
    [SerializeField] private List<float> scoreList;
    [SerializeField] private Transform scoreContainer;
    [SerializeField] private TMP_Text avgText;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text distanceText;

    private void Start()
    {
        gameState = GameState.Waiting;
        waitTime = maxWaitTime;
        fpsCamera.ToggleCanLook(false);
        fpsCamera.ResetRotation();
        round = 1;
        roundText.text = "Round: " + round;
        distanceText.text = "Distance: " + dispenser.getActiveDistance().ToString("f1") + "m";
    }
    // Start is called before the first frame update
    public void HitTarget(RaycastHit hit)
    {
        fpsCamera.ToggleCanLook(false);
        fpsCamera.ResetRotation();
        Destroy(hit.transform.gameObject);
        gameState = GameState.Waiting;
        waitTime = maxWaitTime;
        UpdateScore();
        if (scoreList.Count == scoreContainer.childCount)
        {
            round++;
            roundText.text = "Round: " + round;
            distanceText.text = "Distance: " + dispenser.NextDistance().ToString("f1") + "m";
        }
    }

    private void Update()
    {
        currTime += Time.deltaTime;
        switch (gameState)
        {
            case (GameState.Spawning):
                spawnDelay -= Time.deltaTime;
                promptText.text = "";
                if(spawnDelay <= 0)
                {
                    dispenser.DispenseTarget();
                    spawnTime = currTime;
                    fpsCamera.ToggleCanLook(true);
                    gameState = GameState.Shooting;
                }
                break;
            case (GameState.Waiting):
                waitTime -= Time.deltaTime;
                promptText.text = waitTime.ToString("f0");
                if(waitTime < 0)
                {
                    if(scoreList.Count == scoreContainer.childCount && dispenser.getRemainingRounds() == 0)
                    {
                        Debug.Log("Finito");
                    }
                    if(scoreList.Count == scoreContainer.childCount)
                    {
                        ResetScores();
                    }
                    spawnDelay = Random.Range(spawnDelayMinMax.x, spawnDelayMinMax.y);
                    gameState = GameState.Spawning;
                }
                break;
        }
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
    public void ResetScores()
    {
        for(int i = 0; i < scoreContainer.childCount; i++)
        {
            scoreContainer.GetChild(i).GetComponent<TMP_Text>().text = "Round " + (i + 1) + ':';
        }
        for(int i = 0; i < scoreList.Count; i++)
        {
            scoreList.RemoveAt(i);
            i--;
        }
        avgText.text = "Average: ";
    }
}
