using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public enum TYPE
{
    COIN = 0,
    SPIKE = 1
}

public class HighlightableObject : MonoBehaviour
{
    public int ID;

    public float TTFF;
    public float TFD;
    public int fixations;
    public LANE lane = LANE.NOT_SET;
    public LANE playerLane = LANE.NOT_SET;
    public float approachRateFF;
    public float enteredViewAt; //timestamp
    public bool hasEnteredView = false;
    public float timeToChangeLaneFromEnter = 0;
    public float timeToChangeLaneFromFF = 0;
    public Vector2 gazePos = new Vector2(0, 0);

    public TYPE type;

    public Subset owner;
    GazeAware gazeAware;

    // Start is called before the first frame update
    void Start()
    {
        gazeAware = gameObject.AddComponent<GazeAware>();
        gazeAware.runInEditMode = true;
        gazeAware.enabled = false;
        TTFF = -1;
        TFD = 0;
        fixations = 0;
        approachRateFF = 0;

        if (type == TYPE.COIN)
        {
            gazeAware.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.z - InputManager.instance.transform.position.z) < 27.05 && !hasEnteredView)
        {
            hasEnteredView = true;
            enteredViewAt = Time.time;
            playerLane = InputManager.instance.lane;
            gazePos = TobiiAPI.GetGazePoint().GUI;
        }

        if(hasEnteredView && InputManager.instance.lane == lane && timeToChangeLaneFromEnter == 0)
        {
            timeToChangeLaneFromEnter = Time.time - enteredViewAt;
            timeToChangeLaneFromFF = Time.time - TTFF;
        }

        if (gazeAware.HasGazeFocus)
        {
            fixations++;

            if (TTFF < 0)
            {
                TTFF = Time.time - enteredViewAt;
                approachRateFF = InputManager.instance._approachRate;
            }
            
            TFD += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision CollisionInfo)
    {
        InputManager inputManager = CollisionInfo.gameObject.GetComponent<InputManager>();
        // If the object collides with the player
        if (inputManager)
        {
            owner.UpdateScore(type, inputManager._approachRate, ID);
            gameObject.SetActive(false);
        }
    }

    public void SendGazeData()
    {
        owner.UpdateGazeData(TFD, TTFF, fixations, approachRateFF, lane, playerLane, timeToChangeLaneFromEnter, timeToChangeLaneFromFF, ID);
    }
}
