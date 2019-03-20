using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VISUAL_VARIABLE
{
    CONTROL,
    HUE_BLUE,
    HUE_RED,
    GLOW,
    MOTION,
    TEXTURE,
    LUM_HI,
    LUM_LO,
    FLASH,
    VIS_VARS,
}

public enum LANE
{
    NOT_SET,
    LEFT,
    MIDDLE,
    RIGHT
}

public class Subset : MonoBehaviour
{
  
    public ObjectManager objectManager;
    new Camera camera;
    public Stats stats;
    int objects = 50;//{ get { return (int)Game.instance.scenario; } }
    public VISUAL_VARIABLE visVar;
    bool lameAssHack = true;

    // Start is called before the first frame update
    void Start()
    {
        stats = new Stats();
        objectManager = FindObjectOfType<ObjectManager>();

        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lameAssHack)
        {
            objectManager.SpawnObjects();
            ResetRound(Game.instance.scenario);
            lameAssHack = false;
        }
    }

    // Updates score, called during collision in a HighlightableObject
    public void UpdateScore(TYPE type, float approachRate, int ID)
    {
        switch (type)
        {
            case TYPE.COIN:
                stats.coins++;
                stats.score += 100;
                break;
            case TYPE.SPIKE:
                stats.spikes++;
                stats.score -= 100;
                break;
        }
        if (type == (TYPE)Game.instance.scenario)
            stats.approachRateHit[ID] = approachRate;
    }

    public void UpdateGazeData()
    {
        objectManager.SendGazeData();
    }

    public void UpdateGazeData(float TFD, float TTFF, int fixations, float approachRateFF, LANE objectLane, LANE playerLane, float timeToChangeFromEnter, float timeToChangeFromFF, int ID)
    {
        stats.TFD[ID] = TFD;
        stats.TTFF[ID] = TTFF;
        stats.fixations[ID] = fixations;
        stats.approachRateFF[ID] = approachRateFF;
        stats.highlightLanes[ID] = objectLane;
        stats.playerLanes[ID] = playerLane;
        stats.timeToChangeFromEnter[ID] = timeToChangeFromEnter;
        stats.timeToChangeFromFF[ID] = timeToChangeFromFF;
    }

    public void ResetRound(SCENARIO scenario)
    {
        ResetScore();
        //if (GameManager.instance.round < (int)VISUAL_VARIABLE.VIS_VARS)
        objectManager.Reset(visVar, scenario);
    }

    void ResetScore()
    {
        stats.score = 0;
        stats.coins = 0;
        stats.spikes = 0;
        stats.TFD = new List<float>(new float[objects]);
        stats.TTFF = new List<float>(new float[objects]);
        stats.fixations = new List<int>(new int[objects]);
        stats.highlightLanes = new List<LANE>(new LANE[objects]);
        stats.playerLanes = new List<LANE>(new LANE[objects]);
        stats.approachRateFF = new List<float>(new float[objects]);
        stats.approachRateHit = new List<float>(new float[objects]);
        stats.timeToChangeFromEnter = new List<float>(new float[objects]);
        stats.timeToChangeFromFF = new List<float>(new float[objects]);
    }
}
