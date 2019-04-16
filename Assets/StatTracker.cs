using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public struct Stats
{
    public int score, coins, spikes;
    public List<ObjectStats> objects;

    public override string ToString()
    {
        string output = "";

        output += score.ToString();
        output += ",";
        output += coins.ToString();
        output += ",";
        output += spikes.ToString();
        output += ",";
        foreach (var obj in objects)
        {
            output += obj.ToString();
        }

        return output;
    }
}

public class StatTracker
{
    // Dict over the two scenarios, who's values are dicts over the variables who's values are the stats for the given variable
    private Dictionary<SCENARIO, Dictionary<VISUAL_VARIABLE, Stats>> playerStats;

    string statFilePath;

    public StatTracker()
    {
        // Init dicts
        playerStats = new Dictionary<SCENARIO, Dictionary<VISUAL_VARIABLE, Stats>>();

        playerStats.Add(SCENARIO.POSITIVE, new Dictionary<VISUAL_VARIABLE, Stats>());
        playerStats.Add(SCENARIO.NEGATIVE, new Dictionary<VISUAL_VARIABLE, Stats>());

        statFilePath = "PlayerStatsReedSpooners.csv";
        // Create file if needed
        if (!System.IO.File.Exists(statFilePath))
        {
            System.IO.File.Create(statFilePath);
        }
    }
    
    // Adds the recently completed scenario
    public void AddVariable(Subset subset, SCENARIO scenario)
    {
        subset.UpdateGazeData();
        if(!playerStats[scenario].ContainsKey(subset.visVar))
            playerStats[scenario].Add(subset.visVar, subset.stats);
    }

    public void SaveStats(int score)
    {
        //Total score
        string statText = score.ToString();
        statText += ',';

        foreach (var scenario in playerStats)
        {
            //Scenario
            statText += scenario.Key.ToString();
            statText += ',';

            foreach (var visVar in scenario.Value)
            {
                //Variable
                statText += visVar.Key.ToString();
                statText += ',';
                //Stats per highlighted object
                statText += visVar.Value.ToString();
            }
        }
        statText += '\n';

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(statFilePath))
        {
            sw.Write(statText);
        }
    }
}
