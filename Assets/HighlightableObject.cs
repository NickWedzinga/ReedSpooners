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
    public float TTFF;
    public float TFD;
    public int fixations;

    public TYPE type;

    Subset owner;
    GazeAware gazeAware;

    // Start is called before the first frame update
    void Start()
    {
        gazeAware = gameObject.AddComponent<GazeAware>();
        gazeAware.enabled = false;
        TTFF = -1;
        TFD = 0;
        fixations = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(gazeAware.HasGazeFocus)
        {
            fixations++;

            if (TTFF < 0)
                TTFF = Time.time;
            
            TFD += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision CollisionInfo)
    {
        // If the object collides with the player
        if(CollisionInfo.gameObject.GetComponent<InputManager>())
        {
            GameManager.instance.UpdateScore(type);
            gameObject.SetActive(false);
        }
    }

    public void GazeAware(bool on)
    {
        gazeAware.enabled = on;
    }
}
