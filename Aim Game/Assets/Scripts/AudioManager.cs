using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> noises;

    public void PlayNoise(int val)
    {
        noises[val].Play();
    }
}
