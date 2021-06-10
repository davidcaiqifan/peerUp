using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;

public class LoadScreen : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("CheckDone");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

        });
        StartCoroutine(MoveToNext());
    }

    IEnumerator MoveToNext()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }
}
