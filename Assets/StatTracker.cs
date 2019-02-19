using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

struct Stats
{
    public int score, coins, spikes;
    public List<float> TFD, TTFF;
    public List<int> fixations;
}

class StatTracker
{
    private Dictionary<TECHNIQUE, Stats> stats;

    string filePath;

    StatTracker()
    {
        stats = new Dictionary<TECHNIQUE, Stats>();
        filePath = "PlayerStatsReedSpooners.csv";
        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath);
        }
    }

    void AddTechnique(TECHNIQUE technique, Stats _stats)
    {
        stats.Add(technique, _stats);
    }

    void SaveStats()
    {
        foreach (var technique in stats.Values)
        {
            string playerText = technique.score.ToString();
            playerText += ',';
            playerText += technique.coins.ToString();
            playerText += ',';
            playerText += technique.spikes.ToString();
            playerText += ',';

            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += technique.TFD[i].ToString();
                playerText += ',';
            }
            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += technique.TTFF[i].ToString();
                playerText += ',';
            }
            for (int i = 0; i < GameManager.instance.nrOfHighlightedObjects; ++i)
            {
                playerText += technique.fixations[i].ToString();
                playerText += ',';
            }

            using (System.IO.StreamWriter sw = System.IO.File.AppendText(filePath))
            {
                sw.Write(playerText);
            }
        }
    }
}
