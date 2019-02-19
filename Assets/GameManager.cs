using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    Subset subset;
    int technique;

    public int lastObjectPositionZ { get { return subset.objectManager.lastObjectPositionZ; } }
    public int nrOfHighlightedObjects { get { return subset.objectManager.nrOfHighlightedObjects; } }

    // Start is called before the first frame update
    void Start()
    {
        subset = gameObject.AddComponent<Subset>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Check conditions for swapping technique
        //SwapTechnique((TECHNIQUE)technique++)
    }

    void SwapTechnique(TECHNIQUE technique)
    {
        if (technique != TECHNIQUE.TECHNIQUES)
        { 
            subset.technique = technique;
        }
    }
}
