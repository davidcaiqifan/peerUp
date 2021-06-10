using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stay : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 position;
    private float timeBeforeStop = 5f;
    private float timerToSlow = 4f;
    internal float timer;
    private bool pressed;
    private float zChanging = -5f;
    internal static Stay instance;
    [SerializeField]
    private GameObject spinButton;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.localPosition = position;
        if (pressed)
        {
            timer += Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, zChanging));
            if (timer > timerToSlow)
            {
                zChanging = -1f;
            }
        }
        if (timer >= timeBeforeStop)
        {
            pressed = false;
        }
       
    }

    public void onPress()
    {
        timer = 0;
        pressed = true;
        spinButton.SetActive(false);
    }
}
