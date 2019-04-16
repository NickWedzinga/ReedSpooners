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

        output += TFD.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
        output += ",";
        output += TTFF.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
        output += ",";
        output += fixations.ToString();
        output += ",";

        output += approachRateFF.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
        output += ",";
        output += approachRateHit.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
        output += ",";

        output += timeToChangeFromFF.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
        output += ",";
        output += timeToChangeFromEnter.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture);
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

    public const float PlayerToFarPlaneDistance = 22.95f;
    
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
    
    void Update()
    {
        if (highlight != HIGHLIGHT.NO)
        {
            // If object has entered the view (i.e. passed through far plane)
            if ((transform.position.z - InputManager.instance.transform.position.z) < PlayerToFarPlaneDistance && !hasEnteredView)
            {
                hasEnteredView = true;
                enteredViewAt = Time.time;
                stats.playerLane = InputManager.instance.lane;
                gazePos = TobiiAPI.GetGazePoint().GUI;
            }

            // Executes the first time the player changes to the same lane as the highlighted object.
            if (hasEnteredView && InputManager.instance.lane == stats.lane && stats.timeToChangeFromEnter == 0)
            {
                stats.timeToChangeFromEnter = Time.time - enteredViewAt;
                stats.timeToChangeFromFF = Time.time - stats.TTFF;
            }

            if (gazeAware.HasGazeFocus)
            {
                stats.fixations++;

                // If it's the first time the player is viewing the object.
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
        //Update gaze data for object in subset if it _is_ highlighted
        if (highlight != HIGHLIGHT.NO)
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
