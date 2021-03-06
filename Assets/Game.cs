﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

public enum SCENARIO
{
    POSITIVE,
    NEGATIVE,
    TRAINING
}

public class Game : MonoBehaviour
{
    public static Game instance;
    public StatTracker statTracker { get; private set; }

    public int round { get; set; } //How many vis vars the participant has been through
    private bool _FirstScenario;
    public int _FirstRound;
    public bool _GameOver;

    public Subset subset;
    public int[] visVarOrder;// = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

    public Text Announcer;
    private float _AnnouncerTextTimer;
    private Color _OriginalTextColor;

    public int Score;
    public Text ScoreText;

    public SCENARIO scenario { get; private set; }
    
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
        _FirstRound = 0;
        _GameOver = false;

        // First scenario randomized
        //int startScenario = (int)(Random.value*2.0f);
        //if (startScenario == 0)
        //    scenario = SCENARIO.NEGATIVE;
        //else
        //    scenario = SCENARIO.POSITIVE;
        scenario = SCENARIO.TRAINING;

        //visVarOrder = new int[] /*{ 4 };*/{ 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        visVarOrder = new int[] { 0 }; 

        subset = gameObject.GetComponent<Subset>();
        subset.visVar = (VISUAL_VARIABLE)visVarOrder[0];

        _OriginalTextColor = Announcer.color;
        _AnnouncerTextTimer = Time.deltaTime;
        //Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
        Announcer.text = "TRAINING ROUND";
        Announcer.fontSize = 40;

        Score = 0;
        ScoreText.text = "Score: " + Score;

        TobiiAPI.GetGazePoint();
    }

    // Update is called once per frame
    void Update()
    {
        #region Announcer text
        if(InputManager.instance._approachRate == 0.0f && !_GameOver)
        {
            Announcer.fontSize = 20;
            Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, Screen.height/2.5f);
            if(scenario == SCENARIO.POSITIVE)
            {
                Announcer.text = "You will be approached by SPIKES and COINS.\n Some of the COINS will be HIGHLIGHTED in various ways.\n Your mission is to AVOID THE SPIKES AND COLLECT ALL COINS.\n" +
                    "\nInstructions: \nUse Left Arrow to move left\nUse Right Arrow to move right.\n\nPress SPACE to begin.";
            }
            else if(scenario == SCENARIO.NEGATIVE)
            {
                Announcer.text = "You will be approached by SPIKES and COINS.\n Some of the SPIKES will be HIGHLIGHTED in various ways.\n Your mission is to AVOID THE SPIKES AND COLLECT ALL COINS.\n" +
                    "\nInstructions: \nUse Left Arrow to move left\nUse Right Arrow to move right.\n\nPress SPACE to begin.";
            }
            else
            {
                Announcer.text = "TRAINING SESSION\n\nYou will be approached by SPIKES and COINS.\nYour mission is to AVOID THE SPIKES AND COLLECT ALL COINS.\n" +
                    "\nInstructions: \nUse Left Arrow to move left\nUse Right Arrow to move right.\n\nPress SPACE to begin.";
            }
        }
        else if (_AnnouncerTextTimer > 0)
        {
            _AnnouncerTextTimer += Time.deltaTime;

            if(_AnnouncerTextTimer > 2 && _AnnouncerTextTimer < 4)
            {
                Announcer.color = Color.Lerp(_OriginalTextColor, Color.clear, Mathf.Min(1, (_AnnouncerTextTimer-2.0f) / 2.0f));
            }
            else if (_AnnouncerTextTimer >= 4.0f)
            {
                _AnnouncerTextTimer = 0.0f;
                Announcer.gameObject.SetActive(false);
                Announcer.color = _OriginalTextColor;

                Announcer.fontSize = 40;
                Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f);
            }

        }
        ScoreText.text = "Score: " + (Score + subset.stats.score).ToString();
        #endregion

        if (Input.GetKey(KeyCode.Return) && _GameOver)
        {
                statTracker.SaveStats(Score);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
           
        }
    }

    public void ResetVariable()
    {
        if(scenario != SCENARIO.TRAINING)
        {
            // if last round was not training, save data
            if(_FirstRound > 0)
            {
                statTracker.AddVariable(subset, scenario);
                Score += subset.stats.score;
                ++round;
            }

            // for each round
            if (round < visVarOrder.Length)
            {
                
                if (_FirstRound == 0)
                    _FirstRound++;

                subset.visVar = (VISUAL_VARIABLE)visVarOrder[round];

                subset.ResetRound(scenario);

                Announcer.fontSize = 60;
                Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f);
                Announcer.text = "GET READY FOR ROUND " + (round).ToString();
                if (_FirstRound == 1)
                    Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
                Announcer.gameObject.SetActive(true);
                _AnnouncerTextTimer += Time.deltaTime;
            }
            else if (_FirstScenario)
            {
                _FirstRound = 2;
                _FirstScenario = false;

                round = 0;
                Shuffle(visVarOrder);
                subset.visVar = (VISUAL_VARIABLE)visVarOrder[round];

                if (scenario == SCENARIO.POSITIVE)
                    scenario = SCENARIO.NEGATIVE;
                else
                    scenario = SCENARIO.POSITIVE;

                subset.ResetRound(scenario);
                InputManager.instance._approachRate = 0.0f;
                Announcer.text = "GET READY FOR ROUND " + (round + 1).ToString();
                Announcer.gameObject.SetActive(true);
                _AnnouncerTextTimer += Time.deltaTime;
            }
            else
            {
                Announcer.fontSize = 60;
                Announcer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, Screen.height / 2.5f);
                Announcer.text = "Thank you for participating!.\nPress ENTER to exit.";
                Announcer.gameObject.SetActive(true);

                _GameOver = true;
                InputManager.instance._approachRate = 0.0f;
            }
        }
        else
        {
            int startScenario = (int)(Random.value*2.0f);
            if (startScenario == 0)
                scenario = SCENARIO.NEGATIVE;
            else
                scenario = SCENARIO.POSITIVE;

            InputManager.instance._approachRate = 0.0f;

            //Randomize variable order
            visVarOrder = new int[] /*{ 1, 2 };*/{ 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            Shuffle(visVarOrder);

            ResetVariable();
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

}
