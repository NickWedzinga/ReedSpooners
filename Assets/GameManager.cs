using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum SCENARIO
{
    POSITIVE,
    NEGATIVE
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public StatTracker statTracker { get; private set; }

    Subset subset;
    TECHNIQUE technique;

    public int lastObjectPositionZ { get { return subset.objectManager.lastObjectPositionZ; } }
    public int nrOfHighlightedObjects { get { return subset.objectManager.nrOfHighlightedObjects; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        statTracker = new StatTracker();

        subset = gameObject.AddComponent<Subset>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check conditions for swapping technique
    }

    void SwapTechnique(TECHNIQUE technique)
    {
        if (technique != TECHNIQUE.TECHNIQUES)
        {
            statTracker.AddTechnique(subset.technique, subset.stats);
            subset.technique = technique;
        }
    }
}
