using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TYPE
{
    COIN = 0,
    SPIKE = 1
}

public class HighlightableObject : MonoBehaviour
{
    Subset owner;
    TYPE type;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision CollisionInfo)
    {
        //If the object collides with the player
        if(CollisionInfo.gameObject.GetComponent<InputManager>())
        {
            owner.UpdateScoreBoard(type);
        }
    }
}
