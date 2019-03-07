﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum SCENARIO
{
    POSITIVE,
    NEGATIVE
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public StatTracker statTracker { get; private set; }

    public int round { get; set; } //How many vis vars the participant has been through
    private bool _FirstScenario;

    Subset subset;
    public int[] visVarOrder = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    public Text Announcer;
    private float _AnnouncerTextTimer;
    private Color _OriginalTextColor;

    public int Score;
    public Text ScoreText;

    public Text DebugText;

    private System.Random _random = new System.Random((int)System.DateTime.Now.Ticks);

    public int lastObjectPositionZ { get { if (subset) return subset.objectManager.lastObjectPositionZ; else return 0; } }
    public int nrOfHighlightedObjects { get { if (subset) return subset.objectManager.nrOfHighlightedObjects; else return 0; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        statTracker = new StatTracker();
        
        round = 0;
        _FirstScenario = true;
        //RANDOMIZE VISUALORDER
        Shuffle(visVarOrder);

        subset = gameObject.AddComponent<Subset>();
        subset.visVar = (VISUAL_VARIABLE)visVarOrder[0];

        _OriginalTextColor = Announcer.color;
        _AnnouncerTextTimer = Time.deltaTime;
        Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
        Announcer.fontSize = 40;

        Score = 0;
        ScoreText.text = "Score: " + Score;

        DebugText.enabled = false;
        DebugText.text = "Visual variable: " + subset.visVar.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance._approachRate == 0.0f)
        {
            Announcer.fontSize = 20;
            Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, Screen.height/3);
            if(subset._Scenario == 0)
            {
                Announcer.text = "You will be approached by SPIKES and COINS.\n The COINS will be HIGHLIGHTED in various ways.\n Your mission is to AVOID THE SPIKES AND COLLECT THE COINS.\n" +
                    "\nInstructions: \nUse A or Left Arrow to move left\nUse D or Right Arrow to move right.";
            }
            else
            {
                Announcer.text = "You will be approached by SPIKES and COINS.\n The SPIKES will be HIGHLIGHTED in various ways.\n Your mission is to AVOID THE SPIKES AND COLLECT THE COINS.\n" +
                    "\nInstructions: \nUse A or Left Arrow to move left\nUse D or Right Arrow to move right.";
            }
            
            
        }
        else if (_AnnouncerTextTimer > 0)
        {
            Announcer.color = Color.Lerp(_OriginalTextColor, Color.clear, Mathf.Min(1, _AnnouncerTextTimer / 3.0f));
            _AnnouncerTextTimer += Time.deltaTime;
            if (_AnnouncerTextTimer > 3.0f)
            {
                _AnnouncerTextTimer = 0.0f;
                Announcer.gameObject.SetActive(false);

                Announcer.fontSize = 40;
                Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f);
            }
        }
        ScoreText.text = "Score: " + (Score + subset.stats.score).ToString();

        if(Input.GetKeyUp(KeyCode.F1))
        {
            //Debug mode
            DebugText.enabled = !DebugText.enabled;
        }
    }

    void SwapTechnique()
    {
        subset.visVar = (VISUAL_VARIABLE)visVarOrder[round];
        statTracker.AddTechnique(subset.visVar, subset.stats);
        subset.Reset();

        DebugText.text = "Visual variable: " + subset.visVar.ToString();
    }
    
    public void Reset()
    {
        Score += subset.stats.score;
        // Announcer text reset and active
        ++round;
        if (round < (int)VISUAL_VARIABLE.VIS_VARS)
        {
            SwapTechnique();
            Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
            Announcer.gameObject.SetActive(true);
            _AnnouncerTextTimer += Time.deltaTime;
        }
        else if (_FirstScenario)
        {
            _FirstScenario = false;
            round = 0;
            if (subset._Scenario == 1)
                subset._Scenario = 0;
            else
                subset._Scenario = 1;

            SwapTechnique();
            Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
            Announcer.gameObject.SetActive(true);
            _AnnouncerTextTimer += Time.deltaTime;
        }
        else
        {
            statTracker.SaveStats();
            Application.Quit();
        }
    }

    void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            // find random index to swap
            int randomIndex = _random.Next(0, i);
            int temp = array[randomIndex];

            // swap
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }

    public void UpdateScore(TYPE type)
    {
        subset.UpdateScore(type);
    }
}
