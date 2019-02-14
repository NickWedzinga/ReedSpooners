using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // A list of hits per object per technique
    Dictionary<TECHNIQUE, Stats> subsetStats = new Dictionary<TECHNIQUE, Stats>();
    Subset subset;

    // Start is called before the first frame update
    void Start()
    {
        subset = gameObject.AddComponent<Subset>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < (int)TECHNIQUE.TECHNIQUES; ++i)
        {
            subset.enabled = true;
            subset.technique
            //while(Condition for changing what technique)
            {
                // Do stuff with object manager
            }
            subset.enabled = false;
        }
    }
}
