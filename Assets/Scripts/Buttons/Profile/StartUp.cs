using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUp : MonoBehaviour
{
    [SerializeField]
    private Image face;
    private Object[] sprites;
    [SerializeField]
    private InputField name;
    [SerializeField]
    private InputField course;
    [SerializeField]
    private InputField description;
    private void Start()
    {
         sprites = Resources.LoadAll("Avatars", typeof(Sprite));
         string studentID = PlayerPrefs.GetString("studentID");
         name.text = PlayerPrefs.GetString(studentID + "name", ""); 
         course.text = PlayerPrefs.GetString(studentID + "course", "");
         description.text = PlayerPrefs.GetString(studentID + "description", "");
    }
    void Update()
    {
        face.sprite = (Sprite)sprites[PlayerPrefs.GetInt("avatar", 0)];
    }

}
