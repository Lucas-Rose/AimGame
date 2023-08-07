using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneController : MonoBehaviour
{
    [Header("Playtest Info")]
    [SerializeField] private TMP_InputField nameText;
    [SerializeField] private TMP_Dropdown skillText;
    [SerializeField] private TMP_Dropdown firstTimeText;

    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public static void LoadSceneNumber(int val)
    {
        SceneManager.LoadScene(val);
    }
}
