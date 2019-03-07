﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private bool paused = false;

    public float _approachRate { get; set; }
    public LANE lane {
        get
        {
            if (transform.position.x > 0)
                return LANE.RIGHT;
            else if (transform.position.x < 0)
                return LANE.LEFT;
            else
                return LANE.MIDDLE;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        print("WHY AH U RUNNING?!");

        // Change to increase speed
        _approachRate = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _approachRate == 0.0f)
            _approachRate = 5.0f;
        if (_approachRate > 0.0f)
        {
            if (transform.position.z > GameManager.instance.lastObjectPositionZ + 50)
            {
                ResetVariable();
            }
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && transform.position.x < 1)
            {
                transform.Translate(4.75f, 0, 0);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && transform.position.x > -1)
            {
                transform.Translate(-4.75f, 0, 0);
            }
            transform.Translate(0, 0, Time.deltaTime * _approachRate);
            _approachRate += Time.deltaTime;
        }
    }

    private void ResetVariable()
    {
        Game.instance.ResetVariable();
        _approachRate = 5.0f;
        gameObject.transform.position = new Vector3(0, 1, 0.5f);
    }

    public void OnCollisionEnter(Collision bigColliderBoi)
    {
        if (bigColliderBoi.gameObject.name == "Spike")
        {
            //Destroy(bigColliderBoi.gameObject);
            //bigColliderBoi.gameObject.transform.position = new Vector3(bigColliderBoi.gameObject.transform.position.x, 10, bigColliderBoi.gameObject.transform.position.z);
            bigColliderBoi.gameObject.SetActive(false);
        }
        if (bigColliderBoi.gameObject.name == "Coin")
        {
            //Destroy(bigColliderBoi.gameObject);
            //bigColliderBoi.gameObject.transform.position = new Vector3(bigColliderBoi.gameObject.transform.position.x, 10, bigColliderBoi.gameObject.transform.position.z);
            bigColliderBoi.gameObject.SetActive(false);
        }
    }
}
