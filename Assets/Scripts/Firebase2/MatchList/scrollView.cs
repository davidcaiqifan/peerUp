using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollView : MonoBehaviour
{
    Vector2 scrollPos;

    void OnGUI() // simply an example of a long ScrollView
    {
        scrollPos = GUI.BeginScrollView(
         new Rect(110, 50, 130, 150), scrollPos,
          new Rect(110, 50, 130, 560),
          GUIStyle.none, GUIStyle.none);
        // HOORAY THOSE TWO ARGUMENTS ELIMINATE
        // THE STUPID RIDICULOUS UNITY SCROLL BARS
        for (int i = 0; i < 20; i++)
            GUI.Box(new Rect(110, 50 + i * 28, 100, 25), "xxxx_" + i);
        GUI.EndScrollView();
    }

    void Update()  // so, make it scroll with the user's finger
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.touches[0];
        if (touch.phase == TouchPhase.Moved)
        {
            // simplistically, scrollPos.y += touch.deltaPosition.y;
            // but that doesn't actually work

            // don't forget you need the actual delta "on the glass"
            // fortunately it's easy to do this...

            float dt = Time.deltaTime / touch.deltaTime;
            if (dt == 0 || float.IsNaN(dt) || float.IsInfinity(dt))
                dt = 1.0f;
            Vector2 glassDelta = touch.deltaPosition * dt;

            scrollPos.y += glassDelta.y;
        }
    }
    
}