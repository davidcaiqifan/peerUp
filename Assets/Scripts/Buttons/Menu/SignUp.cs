using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUp : MyButton
{
    public AudioSource buttonSound;
    override
    public void function()
    {
        bg.color = new Color32(120, 120, 120, 255);
        toSetInactive.SetActive(true);
        buttonSound.Play();
    }
}
