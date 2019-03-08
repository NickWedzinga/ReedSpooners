using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct Stats
{
    public int score, coins, spikes;
    public List<float> TFD, TTFF;
    public List<int> fixations;
    public List<LANE> playerLanes;
    public List<LANE> highlightLanes;
    public List<float> approachRateFF;
    public List<float> approachRateHit;
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
                var stats = visVar.Value;
                playerText += stats.score.ToString();
                playerText += ',';
                playerText += (stats.coins / 2).ToString();
                playerText += ',';
                playerText += (stats.spikes / 2).ToString();
                playerText += ',';

                playerText += "TFD";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.TFD[i].ToString();
                    playerText += ',';
                }
                playerText += "TTFF";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.TTFF[i].ToString();
                    playerText += ',';
                }
                playerText += "Fixations";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.fixations[i].ToString();
                    playerText += ',';
                }

                playerText += "Player lanes";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.playerLanes[i].ToString();
                    playerText += ',';
                }
                playerText += "Object lanes";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.highlightLanes[i].ToString();
                    playerText += ',';
                }

                playerText += "Approach rate FF";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.approachRateFF[i].ToString();
                    playerText += ',';
                }
                playerText += "Approach rate hit";
                playerText += ',';
                for (int i = 0; i < (int)scenario.Key; ++i)
                {
                    playerText += stats.approachRateHit[i].ToString();
                    playerText += ',';
                }
            }
        }
        playerText += '\n';

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(filePath))
        {
            sw.Write(playerText);
        }
    }
}
