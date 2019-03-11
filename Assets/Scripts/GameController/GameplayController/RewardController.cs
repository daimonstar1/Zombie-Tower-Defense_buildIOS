using UnityEngine;
using System.Collections;

public class RewardController : MonoBehaviour
{

    // Use this for initialization

    //for star
    static int starFirst = 50;
    static int starIncreasePerLevel = 50;
    static float starIncreasePercentPerStarGot = 10; //%
    //static float increaseStarPercentPerLevel = 20; //%
    //static float increaseStarPercentPerStarGot = 5; //%

    //for gem
    static int gemFirst = 3;
    static float gemIncreasePerLevel = 0.5f;
    static float gemIncreasePercentPerStarGot = 5; //%
    //static int increaseGemPercenetPerLevel = 1; //%
    //static float increaseGemPercentPerStarGot = 10; //%

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static int GetStarReward(int level, int starGotAtLevel)
    {
        float star = starFirst;
        star += (level - 1) * starIncreasePerLevel;
        star = (int)Master.IncreaseValues(star, starGotAtLevel, starIncreasePercentPerStarGot);

        if (GameplayController.gameDifficult == GameplayController.GameDifficult.Hard)
        {
            star = star * 2;
            if (level <= Master.LevelData.lastLevelHard)
            {
                star = (int)star * 0.5f;
            }
            Debug.Log(star);

        }
        else
        {

            if (level <= Master.LevelData.lastLevel)
            {
                star = (int)(star * 0.5f);
            }
        }
        Master.Stats.Star += (int)star;
        return (int)star;
    }

    public static int GetGemReward(int level, int starGotAtLevel)
    {
        float gem = gemFirst;
        gem += level * gemIncreasePerLevel;
        gem = Master.IncreaseValues(gem, starGotAtLevel, gemIncreasePercentPerStarGot);

        if (GameplayController.gameDifficult == GameplayController.GameDifficult.Hard)
        {
            gem = gem * 2;
            if (level <= Master.LevelData.lastLevelHard)
            {
                gem = gem * 0.5f;
            }
            Debug.Log(gem);
        }
        else
        {
            if (level <= Master.LevelData.lastLevel)
            {
                gem = gem * 0.5f;
            }
        }

        Master.Stats.Gem += (int)gem;
        return (int)gem;
    }

}
