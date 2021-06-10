using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MyButton : MonoBehaviour
{
    [SerializeField]
    internal Image bg;
    [SerializeField]
    internal GameObject toSetInactive;
    public abstract void function();
}
