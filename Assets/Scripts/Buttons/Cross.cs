using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Cross : MyButton
{
    override
    public void function()
    {
        bg.color = new Color(255, 255, 255);
        toSetInactive.SetActive(false);
    }
}
