using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveProfile : MonoBehaviour
{
    [SerializeField]
    private GameObject[] interactables;
    [SerializeField]
    private Text success;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(function);

    }
    public void function()
    {
        foreach (GameObject g in interactables)
        {
            InputField inputField = g.GetComponent<InputField>();
            inputField.interactable = false;
            success.text = "Profile saved successfully!";

        }
    }
}
