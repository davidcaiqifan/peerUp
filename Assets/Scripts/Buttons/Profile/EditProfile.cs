using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProfile : MonoBehaviour
{
    [SerializeField]
    private GameObject[] interactables;
    [SerializeField]
    private Text success;
    internal static EditProfile instance;

    private void Awake()
    {
        instance = this;
    }
    public void function()
    {
        foreach(GameObject g in interactables)
        {
            Selectable selectable = g.GetComponent<Selectable>();
            selectable.interactable = !selectable.interactable;
            success.text = "";
        }
    }
}
