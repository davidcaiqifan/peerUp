using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectAvatar : MonoBehaviour
{
    [SerializeField]
    private GameObject Avatars;

    public void activateAvatars()
    {
        Avatars.SetActive(true);
    }
}
