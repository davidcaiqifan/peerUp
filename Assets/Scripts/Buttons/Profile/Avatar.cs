using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(updateAvatar);
    }
    public void updateAvatar()
    {
        int avatarIndex = (int)char.GetNumericValue(transform.name[6]);
        Debug.Log(avatarIndex);
        PlayerPrefs.SetInt("avatar", avatarIndex); // Only works for 0-9
        transform.parent.parent.gameObject.SetActive(false);
        userProfile.instance.function();
    }
}
