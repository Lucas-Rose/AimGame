using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slideshow : MonoBehaviour
{
    private List<GameObject> children = new List<GameObject>();
    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
    }
    public void NextSlide()
    {
        Destroy(transform.GetChild(0).gameObject);
        children.RemoveAt(0);
        children[0].SetActive(true);
    }
}
