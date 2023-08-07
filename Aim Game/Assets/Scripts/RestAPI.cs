using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using TMPro;

public class RestAPI : MonoBehaviour
{
    [Header("PlayerInfo")]
    [SerializeField] private TMP_InputField nameText;
    [SerializeField] private TMP_Dropdown skillText;
    [SerializeField] private TMP_Dropdown firstTimeText;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private static string PlaytestURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/Playtests";
    private static string RoundURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/Rounds";
    private static string playtestID = "";

    public static IEnumerator GetPlaytestData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(PlaytestURL))
        {
            yield return request.SendWebRequest();
            ProcessRequest(request);
        }
    }
    public static IEnumerator PostPlaytestData(string name, string skill, string firstTime)
    {
        Debug.Log("Posting");
        WWWForm form = new WWWForm();
        form.AddField("Name", $"{name}");
        form.AddField("Skill", $"{skill}");
        form.AddField("FirstTime", $"{firstTime}");

        using (UnityWebRequest request = UnityWebRequest.Post(PlaytestURL, form))
        {
            yield return request.SendWebRequest();
            StartPlaytest(request);
            SceneController.LoadNextScene();
        }
    }

    public void AddPlaytest()
    {
        string firstTime = firstTimeText.options[firstTimeText.value].text == "Is" ? "Yes" : "No";
        string skill = skillText.options[skillText.value].text;
        StartCoroutine(PostPlaytestData(nameText.text, skill, firstTime));
    }

    public void AddRoundData(string killTime, bool onScreen, string distance, Vector2 targetPos, Vector2 playerRot, int misses, float timeToMove)
    {
        StartCoroutine(PostRoundData(killTime, onScreen, distance, targetPos, playerRot, misses, timeToMove));
    }

    public static IEnumerator PostRoundData(string killTime, bool onScreen, string distance, Vector2 targetPos, Vector2 playerRot, int misses, float timeToMove)
    {
        yield return GetPlaytestData();
        WWWForm form = new WWWForm();
        form.AddField("PlaytestID", $"{playtestID}");
        form.AddField("KillTime", killTime);
        form.AddField("OnScreen", onScreen ? "Yes" : "No");
        form.AddField("Distance", distance);
        form.AddField("targetXPos", targetPos.x.ToString());
        form.AddField("targetYPos", targetPos.y.ToString());
        form.AddField("playerYRot", playerRot.x.ToString());
        form.AddField("playerXRot", playerRot.y.ToString());
        form.AddField("Misses", misses);
        form.AddField("Shots", misses + 1);
        form.AddField("ReactionTime", timeToMove.ToString());
        using (UnityWebRequest request = UnityWebRequest.Post(RoundURL, form))
        {
            yield return request.SendWebRequest();
            StartPlaytest(request);
        }
    }

    private static void ProcessRequest(UnityWebRequest req)
    {
        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(req.error);
        }
        else
        {
            string json = req.downloadHandler.text;
            SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
        }
    }

    private static void StartPlaytest(UnityWebRequest req)
    {
        if (req.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(req.error);
        }
        else
        {
            string json = req.downloadHandler.text;
            SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
            if(stats["Name"] != null)
            {
                playtestID = stats["id"];
            }
        }
    }
}
