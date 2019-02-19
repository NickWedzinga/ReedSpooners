using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TECHNIQUE
{
    CONTROL,
    HUE_BLUE,
    HUE_RED,
    GLOW,
    MOTION,
    TEXTURE,
    VALUE,
    FLASH,
    TECHNIQUES,
}

public class Subset : MonoBehaviour
{
    List<HighlightableObject> HighlightableObjects = new List<HighlightableObject>();
    public ObjectManager objectManager;
    Tobii.Gaming.GazePoint gazePoint = new Tobii.Gaming.GazePoint();
    new Camera camera = FindObjectOfType<Camera>();
    bool active = false;
    Stats stats = new Stats();
    int objects = 10;
    int lookingAt = -1;
    TECHNIQUE _technique;
    public TECHNIQUE technique { get { return _technique; } set { Reset(); _technique = value; } }

    // Start is called before the first frame update
    void Start()
    {
        objectManager = gameObject.GetComponent<ObjectManager>();
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
                stats.score += 50;
                break;
            case TYPE.SPIKE:
                stats.spikes++;
                stats.score -= 50;
                break;
        }
    }

    public void UpdateGazeStuff(float TFD, float TTFF, int fixations, int ID)
    {
        stats.TFD[ID] = TFD;
        stats.TTFF[ID] = TTFF;
        stats.fixations[ID] = fixations;
    }

    void Reset()
    {
        ResetScore();
        objectManager.Reset();
    }

    void ResetScore()
    {
        stats.score = 0;
        stats.coins = 0;
        stats.spikes = 0;
        stats.TFD = new List<float>(new float[objects]);
        stats.TTFF = new List<float>(new float[objects]);
    }
}
