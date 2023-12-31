using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private TargetDispenser dispenser;
    [SerializeField] private FPSCamera fpsCamera;
    [SerializeField] private AudioManager aManager;
    [SerializeField] private RestAPI api;
    [SerializeField] private GameObject player;
    [SerializeField] private AIController ai;
    private int round;

    [Header("Time Elements")]
    [SerializeField] private float maxWaitTime;
    [SerializeField] private Vector2 spawnDelayMinMax;
    private float waitTime;
    private float spawnDelay;
    private float currTime;
    private float spawnTime;
    private float shootTime;
    private float lastReaction;
    private float timeToMove;
    private float tspm;
    private bool hasMoved;

    [Header("Gun Elements")]
    private int shots;
    private int misses;
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
        if(fpsCamera != null)
        {
            fpsCamera.ToggleCanLook(false);
            fpsCamera.ResetRotation();
            api = GameObject.Find("API").GetComponent<RestAPI>();
        }
        round = 1;
        roundText.text = "Round: " + round;
        distanceText.text = "Distance: " + dispenser.getActiveDistance().ToString("f1") + "m";
        hasMoved = false;
    }

    public void HitTarget(RaycastHit hit)
    {
        aManager.PlayNoise(3);
        if(fpsCamera != null)
        {
            fpsCamera.ToggleCanLook(false);

            api.AddRoundData(
            lastReaction,
            dispenser.GetOnScreen(),
            dispenser.getActiveDistance().ToString(),
            new Vector2(hit.transform.position.x, hit.transform.position.y),
            hit.point,
            new Vector2(player.transform.rotation.eulerAngles.y, fpsCamera.gameObject.transform.rotation.eulerAngles.x),
            misses,
            timeToMove
        );
            fpsCamera.ResetRotation();
            tspm = 0;
        }
        

        if(ai != null)
        {
            ai.ResetRotation();
            ai.SetCanMove(false);
        }
        UpdateScore();

        Destroy(hit.transform.gameObject);
        gameState = GameState.Waiting;
        waitTime = maxWaitTime;
        misses = 0;
        hasMoved = false;
        if (scoreList.Count == scoreContainer.childCount)
        {
            round++;
            roundText.text = "Round: " + round;
            dispenser.NextDistance();
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
                    if(ai!= null)
                    {
                        ai.SetCanMove(true);
                        ai.StartAiming(dispenser.GetTarget().transform);
                    }
                    spawnTime = currTime;
                    if(fpsCamera != null)
                    {
                        fpsCamera.ToggleCanLook(true);
                    }
                    
                    gameState = GameState.Shooting;
                    aManager.PlayNoise(2);
                }
                break;
            case (GameState.Waiting):
                waitTime -= Time.deltaTime;
                promptText.text = waitTime.ToString("f1");
                if(waitTime < 0)
                {
                    if(scoreList.Count == scoreContainer.childCount && dispenser.getRemainingRounds() == 0)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        SceneController.LoadNextScene();
                    }
                    if(scoreList.Count == scoreContainer.childCount)
                    {
                        ResetScores();
                    }
                    aManager.PlayNoise(1);
                    spawnDelay = Random.Range(spawnDelayMinMax.x, spawnDelayMinMax.y);
                    gameState = GameState.Spawning;
                }
                break;
            case (GameState.Shooting):
                if (!hasMoved)
                {
                    if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                    {
                        timeToMove = currTime - spawnTime;
                        hasMoved = true;
                    }
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
            lastReaction = shootTime - spawnTime;
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
        distanceText.text = "Distance: " + dispenser.getActiveDistance().ToString("f1") + "m";
    }

    public void AddMiss(RaycastHit hit)
    {
        tspm = misses == 0 ? 0 : currTime - tspm;
        misses += 1;
        api.AddMissData(
            currTime,
            misses,
            tspm,
            dispenser.GetOnScreen(),
            dispenser.getActiveDistance().ToString(),
            new Vector2(hit.transform.position.x, hit.transform.position.y),
            hit.point,
            new Vector2(player.transform.rotation.eulerAngles.y, fpsCamera.gameObject.transform.rotation.eulerAngles.x)
        );
    }
}
