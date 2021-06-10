using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Password : MonoBehaviour
{
    [SerializeField]
    private InputField Password1;
    [SerializeField]
    private Text error;
    [SerializeField]
    private Button enter;
    public void checkPassword()
    {
        if(this.GetComponent<InputField>().text != Password1.text)
        {
            error.text = "Password is different!";
            enter.interactable = false;
        } else
        {
            error.text = "";
            enter.interactable = true;
        }
    }
}
