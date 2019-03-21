using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private Dictionary<SCENARIO, Dictionary<VISUAL_VARIABLE, Stats>> playerStats;

    string filePath;

    public StatTracker()
    {
        playerStats = new Dictionary<SCENARIO, Dictionary<VISUAL_VARIABLE, Stats>>();
        playerStats.Add(SCENARIO.POSITIVE, new Dictionary<VISUAL_VARIABLE, Stats>());
        playerStats.Add(SCENARIO.NEGATIVE, new Dictionary<VISUAL_VARIABLE, Stats>());
        filePath = "PlayerStatsReedSpooners.csv";
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath);
        }
    }

    public void AddTechnique(Subset subset, SCENARIO scenario)
    {
        subset.UpdateGazeData();
        if(!playerStats[scenario].ContainsKey(subset.visVar))
            playerStats[scenario].Add(subset.visVar, subset.stats);
    }

    public void SaveStats(int score)
    {
        string playerText = score.ToString();
        playerText += ',';
        foreach (var scenario in playerStats)
        {
            foreach (var visVar in scenario.Value)
            {
                playerText += visVar.Key.ToString();
                playerText += ',';
                playerText += visVar.Value.ToString();
            }
        }
        playerText += '\n';

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(filePath))
        {
            sw.Write(playerText);
        }
    }
}
