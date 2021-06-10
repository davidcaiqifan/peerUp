using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MyButton
{
    public AudioSource buttonSound;
    public GameObject loginPanel;
    override
    public void function()
    {
        bg.color = new Color32(120, 120, 120, 255);
        toSetInactive.SetActive(true);
        buttonSound.Play();

    }
   
    public void loginB()
    {
        loginPanel.SetActive(true);
    }
    public void quitButton()
    {
        Application.Quit();
    }
}
