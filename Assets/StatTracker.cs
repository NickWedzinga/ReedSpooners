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
}

public class StatTracker
{
    private Dictionary<TECHNIQUE, Stats> stats;

    string filePath;

    public StatTracker()
    {
        stats = new Dictionary<TECHNIQUE, Stats>();
        filePath = "PlayerStatsReedSpooners.csv";
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath);
        }
    }

    public void AddTechnique(TECHNIQUE technique, Stats _stats)
    {
        stats.Add(technique, _stats);
    }

    public void SaveStats()
    {
        string playerText = "";
        foreach (var techniqueStats in stats)
        {
            playerText += techniqueStats.Key.ToString();
            playerText += ',';
            var stats = techniqueStats.Value;
            playerText += stats.score.ToString();
            playerText += ',';
            playerText += stats.coins.ToString();
            playerText += ',';
            playerText += stats.spikes.ToString();
            playerText += ',';

            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += stats.TFD[i].ToString();
                playerText += ',';
            }
            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += stats.TTFF[i].ToString();
                playerText += ',';
            }
            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += stats.fixations[i].ToString();
                playerText += ',';
            }
        }

        using (System.IO.StreamWriter sw = System.IO.File.AppendText(filePath))
        {
            sw.Write(playerText);
        }
    }
}
