using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Remember : MonoBehaviour
{
    [SerializeField]
    private GameObject checkMark;
    [SerializeField]
    private InputField studentID;
    [SerializeField]
    private InputField password;
    private int prev;

    private void Start()
    {
        prev = PlayerPrefs.GetInt("Remember", -1);
        if (prev == 1)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = checkMark.GetComponent<SpriteRenderer>().sprite;
            studentID.text = PlayerPrefs.GetString("emailNUS", "");
            password.text = PlayerPrefs.GetString("password", "");
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = null;
        }
    }
    public void tick()
   {
        prev = PlayerPrefs.GetInt("Remember", -1);
        PlayerPrefs.SetInt("Remember", -1 * prev);
        transform.GetChild(0).GetComponent<Image>().sprite = -1 * prev == 1 ? checkMark.GetComponent<SpriteRenderer>().sprite : null;
        
   }
}
