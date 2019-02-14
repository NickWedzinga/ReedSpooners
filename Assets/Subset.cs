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

struct Stats
{
    public int score, coins, spikes;
    public List<float> TFD, TTFF;
}

public class Subset : MonoBehaviour
{
    List<HighlightableObject> HighlightableObjects = new List<HighlightableObject>();
    Tobii.Gaming.GazePoint gazePoint = new Tobii.Gaming.GazePoint();
    new Camera camera = FindObjectOfType<Camera>();
    bool active = false;
    Stats stats = new Stats();
    int objects = 10;
    int lookingAt = -1;
    public TECHNIQUE technique;

    // Start is called before the first frame update
    void Start()
    {
        stats.score = 0;
        stats.coins = 0;
        stats.spikes = 0;
        stats.TFD = new List<float>(new float[objects]);
        stats.TTFF = new List<float>(new float[objects]);
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            GazeCheck();
        }
    }

    // Gaze intersection check
    void GazeCheck()
    {
        Vector3 screenGaze = new Vector3(gazePoint.Screen.x, gazePoint.Screen.y, 0);
        Vector3 worldGaze = camera.ScreenToWorldPoint(screenGaze);
        Ray ray = new Ray(worldGaze, InputManager.instance.transform.forward);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo);
        if(hit)
        {
            HighlightableObject obj = hitInfo.collider.gameObject.GetComponent<HighlightableObject>();
            if (HighlightableObjects.Contains(obj))
            {
                if (stats.TTFF[obj.ID] < 0)
                    stats.TTFF[obj.ID] = gazePoint.Timestamp;

                if (lookingAt != obj.ID)
                    lookingAt = obj.ID;
                else
                    stats.TFD[lookingAt] += Time.deltaTime;
            }
        }
    }

    // Updates score, called during collision in a HighlightableObject
    public void UpdateScoreBoard(TYPE type)
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
}
