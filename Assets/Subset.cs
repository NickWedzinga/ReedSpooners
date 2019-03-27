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
    int objects = 10;   
    public VISUAL_VARIABLE visVar;
    bool lameAssHack = true;

    int storageCounter = 0;

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
    public void UpdateScore(TYPE type, float approachRate)
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
    }

    public void UpdateGazeData()
    {
        objectManager.SendGazeData();
    }

    public void UpdateGazeData(ObjectStats objectStats)
    {
        if(storageCounter < objects)
        {
            stats.objects[storageCounter] = objectStats;
            ++storageCounter;
        }
    }

    public void ResetRound(SCENARIO scenario)
    {
        ResetScore();
        //if (GameManager.instance.round < (int)VISUAL_VARIABLE.VIS_VARS)
        objectManager.Reset(visVar, scenario);
    }

    void ResetScore()
    {
        storageCounter = 0;
        stats.score = 0;
        stats.coins = 0;
        stats.spikes = 0;
        stats.objects = new List<ObjectStats>(new ObjectStats[objects]);
    }
}
