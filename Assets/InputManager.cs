using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    float _currentTime;
    public float _approachRate { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        print("Start running");
        _currentTime = Time.deltaTime;

        // Change to increase speed
        _approachRate = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && transform.position.x < 1)
        {
            transform.Translate(4.75f, 0, 0);
        }
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && transform.position.x > -1)
        {
            transform.Translate(-4.75f, 0, 0);
        }
        transform.Translate(0, 0, Time.deltaTime * _approachRate);
    }
}
