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

public class Subset : MonoBehaviour
{
  
    public ObjectManager objectManager;
    new Camera camera;
    public Stats stats;
    int objects = 10;
    public int _Scenario; 
    VISUAL_VARIABLE _visVar;
    public VISUAL_VARIABLE visVar { get { return _visVar; } set { _visVar = value; } }

    // Start is called before the first frame update
    void Start()
    {
        _Scenario = (int)(Random.value * 2);
        stats = new Stats();

        objectManager = FindObjectOfType<ObjectManager>();
        objectManager.SpawnObjects(visVar);
        Reset();

        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Updates score, called during collision in a HighlightableObject
    public void UpdateScore(TYPE type)
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

    public void UpdateGazeData(float TFD, float TTFF, int fixations, int ID)
    {
        stats.TFD[ID] = TFD;
        stats.TTFF[ID] = TTFF;
        stats.fixations[ID] = fixations;
    }

    public void Reset()
    {
        ResetScore();
        //if (GameManager.instance.round < (int)VISUAL_VARIABLE.VIS_VARS)
        objectManager.Reset(visVar, _Scenario);

    }

    void ResetScore()
    {
        stats.score = 0;
        stats.coins = 0;
        stats.spikes = 0;
        stats.TFD = new List<float>(new float[objects]);
        stats.TTFF = new List<float>(new float[objects]);
        stats.fixations = new List<int>(new int[objects]);
    }
}
