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
    [SerializeField] private GameObject startButton;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private static string PlaytestURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/Playtests";
    private static string RoundURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/Rounds";
    private static string RoundTwoURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/RoundsTwo";
    private static string RoundThreeURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/RoundsThree";
    private static string RoundFourURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/RoundsFour";
    private static string RoundFiveURL = "https://64d051a5ff953154bb78c435.mockapi.io/api/RoundsFive";
    private static int activeRoundURLIndex;
    private static string[] roundURLs = { RoundURL, RoundTwoURL, RoundThreeURL, RoundFourURL, RoundFiveURL };
    private static string playtestID = "";

    public static IEnumerator GetPlaytestData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(PlaytestURL))
        {
            yield return request.SendWebRequest();
            ProcessRequest(request);
        }
    }

    public static IEnumerator GetRoundData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(roundURLs[activeRoundURLIndex]))
        {
            yield return request.SendWebRequest();
            ProcessRequest(request);
        }
    }
    public static IEnumerator PostPlaytestData(string name, string skill, string firstTime)
    {
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
        startButton.SetActive(false);
        string firstTime = firstTimeText.options[firstTimeText.value].text == "Is" ? "Yes" : "No";
        string skill = skillText.options[skillText.value].text;
        StartCoroutine(PostPlaytestData(nameText.text, skill, firstTime));
    }

    public void AddRoundData(float killTime, bool onScreen, string distance, Vector2 targetPos, Vector3 hitPos, Vector2 playerRot, int misses, float timeToMove)
    {
        StartCoroutine(PostRoundData(killTime, onScreen, distance, targetPos, hitPos, playerRot, misses, timeToMove, killTime - timeToMove));
        StartCoroutine(GetRoundData());
    }

    public static IEnumerator PostRoundData(float killTime, bool onScreen, string distance, Vector2 targetPos, Vector3 hitPos, Vector2 playerRot, int misses, float timeToMove, float moveTime)
    {
        yield return GetPlaytestData();
        WWWForm form = new WWWForm();
        form.AddField("PlaytestID", $"{playtestID}");
        form.AddField("KillTime", killTime.ToString());
        form.AddField("OnScreen", onScreen ? "Yes" : "No");
        form.AddField("Distance", distance);
        form.AddField("targetXPos", targetPos.x.ToString());
        form.AddField("targetYPos", targetPos.y.ToString());
        form.AddField("hitXPos", hitPos.x.ToString());
        form.AddField("hitYPos", hitPos.y.ToString());
        form.AddField("hitZPos", hitPos.z.ToString());
        form.AddField("playerXRot", playerRot.x.ToString());
        form.AddField("playerYRot", playerRot.y.ToString());
        form.AddField("Misses", misses);
        form.AddField("Shots", misses + 1);
        form.AddField("ReactionTime", timeToMove.ToString());
        form.AddField("Time Moving", moveTime.ToString());
        using (UnityWebRequest request = UnityWebRequest.Post(roundURLs[activeRoundURLIndex], form))
        {
            yield return request.SendWebRequest();
            ProcessRequest(request);
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
            if(stats["Misses"] != null)
            {
                if(stats[0] == "100")
                {
                    if (activeRoundURLIndex < roundURLs.Length - 1)
                    {
                        activeRoundURLIndex++;
                    }
                }
            }
            
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
            playtestID = stats["id"];
        }
    }
}
