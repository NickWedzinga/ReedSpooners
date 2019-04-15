using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public enum TYPE
{
    COIN,
    SPIKE
}

public enum HIGHLIGHT
{
    NO = 0,
    HIGHLIGHTEDCOIN = 1,
    HIGHLIGHTEDSPIKE = 2
}

public struct ObjectStats
{
    public LANE lane;
    public LANE playerLane;
    public LANE FFLane;

    public float TFD;
    public float TTFF;
    public int   fixations;

    public float approachRateFF;
    public float approachRateHit;

    public float timeToChangeFromFF;
    public float timeToChangeFromEnter;

    public override string ToString()
    {
        string output = "";

        output += lane.ToString();
        output += ",";
        output += playerLane.ToString();
        output += ",";
        output += FFLane.ToString();
        output += ",";

        output += TFD.ToString();
        output += ",";
        output += TTFF.ToString();
        output += ",";
        output += fixations.ToString();
        output += ",";

        output += approachRateFF.ToString();
        output += ",";
        output += approachRateHit.ToString();
        output += ",";

        output += timeToChangeFromFF.ToString();
        output += ",";
        output += timeToChangeFromEnter.ToString();
        output += ",";

        return output;
    }
}

public class HighlightableObject : MonoBehaviour
{
    public ObjectStats stats;

    public float enteredViewAt; //timestamp
    public bool hasEnteredView = false;
    public Vector2 gazePos = new Vector2(0, 0);

    public TYPE type;
    public HIGHLIGHT highlight;

    public Subset owner;
    GazeAware gazeAware;
    //bool highlighted { get { return (type == (TYPE)Game.instance.scenario); } }

    // Start is called before the first frame update
    void Start()
    {
        owner = FindObjectOfType<Subset>();
        gazeAware = gameObject.AddComponent<GazeAware>();
#if UNITY_EDITOR
        gazeAware.runInEditMode = true;
#endif
        gazeAware.enabled = false;

        stats.TTFF = -1;
        stats.TFD = 0;
        stats.fixations = 0;
        stats.approachRateFF = 0;
        stats.lane = LANE.NOT_SET;
        stats.playerLane = LANE.NOT_SET;

        if (type == TYPE.COIN)
        {
            gazeAware.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (/*highlighted*/highlight != HIGHLIGHT.NO)
        {
            if ((transform.position.z - InputManager.instance.transform.position.z) < 27.05 && !hasEnteredView)
            {
                hasEnteredView = true;
                enteredViewAt = Time.time;
                stats.playerLane = InputManager.instance.lane;
                gazePos = TobiiAPI.GetGazePoint().GUI;
            }

            if (hasEnteredView && InputManager.instance.lane == stats.lane && stats.timeToChangeFromEnter == 0)
            {
                stats.timeToChangeFromEnter = Time.time - enteredViewAt;
                stats.timeToChangeFromFF = Time.time - stats.TTFF;
            }

            if (gazeAware.HasGazeFocus)
            {
                stats.fixations++;

                if (stats.TTFF < 0)
                {
                    stats.TTFF = Time.time - enteredViewAt;
                    stats.approachRateFF = InputManager.instance._approachRate;
                    stats.FFLane = InputManager.instance.lane;
                }

                stats.TFD += Time.deltaTime;
            }
        }
    }

    void OnCollisionEnter(Collision CollisionInfo)
    {
        InputManager inputManager = CollisionInfo.gameObject.GetComponent<InputManager>();
        // If the object collides with the player
        if (inputManager)
        {
            owner.UpdateScore(type, inputManager._approachRate);
            gameObject.SetActive(false);
            stats.approachRateHit = inputManager._approachRate;
        }
    }

    public void SendGazeData()
    {
        if (/*highlighted*/highlight != HIGHLIGHT.NO)
        {
            if (transform.position.x > 0)
                stats.lane = LANE.RIGHT;
            else if (transform.position.x < 0)
                stats.lane = LANE.LEFT;
            else
                stats.lane = LANE.MIDDLE;
            owner.UpdateGazeData(stats);
        }
    }
}
