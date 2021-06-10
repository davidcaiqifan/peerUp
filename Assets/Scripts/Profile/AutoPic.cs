using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AutoPic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sprites = Resources.LoadAll("Avatars", typeof(Sprite));
        int len = transform.childCount;
        for(int i = 0; i < len; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = sprites[i] as Sprite;
        }
  
    }

}
