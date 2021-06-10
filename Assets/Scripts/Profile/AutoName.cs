using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoName : MonoBehaviour
{
    [SerializeField]
    private string name;
    [SerializeField]
    private int startIndex;
    private void Awake()
    {
        int counts = transform.childCount;
        int index = startIndex;
        for (int i = startIndex; i < counts; i++)
        {
            transform.GetChild(i).name = name + index++;
        }
    }
}
