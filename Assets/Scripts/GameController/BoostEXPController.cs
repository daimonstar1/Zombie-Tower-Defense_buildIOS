using UnityEngine;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
public class BoostEXPController : MonoBehaviour
{
    private static BoostEXPController _instance;
    public static BoostEXPController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.FindObjectsOfTypeAll<BoostEXPController>()[0];
            }
            return _instance;
        }
    }

    public static int levelCanBuyBoostEXP = 4;

    public bool IsBoosting
    {
        get
        {
            if (TimeRemaining <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public int BoostType
    {
        get
        {
            return ObscuredPrefs.GetInt("BoostEXPType", 0);
        }
        set
        {
            ObscuredPrefs.SetInt("BoostEXPType", value);
            ObscuredPrefs.Save();
        }
    }

    public string DateTimeEndBoostingString
    {
        get
        {
            return ObscuredPrefs.GetString("BoostEXPDateTimeEnd");
        }
        set
        {
            ObscuredPrefs.SetString("BoostEXPDateTimeEnd", value);
            ObscuredPrefs.Save();
        }
    }

    public float TimeRemaining//second
    {
        get
        {
            if (DateTimeEndBoostingString == "")
            {
                return 0;
            }
            else
            {
                DateTime dateTimeEndBoosting = DateTime.ParseExact(DateTimeEndBoostingString, "yyyy-MM-dd HH:mm:ss", null);
                DateTime currentDateTime = DateTime.Now;

                if (currentDateTime.CompareTo(dateTimeEndBoosting) >= 0)
                {
                    return 0;
                }
                else
                {
                    return (int)(dateTimeEndBoosting - currentDateTime).TotalSeconds;
                }
            }
        }
    }



    public void SetBoosting(int boostType, int day = 1)
    {
        DateTime currentDateTime = DateTime.Now.AddDays(day);
        DateTimeEndBoostingString = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        BoostType = boostType;
    }

    public string getTimeRemainingString()
    {
        TimeSpan time = TimeSpan.FromSeconds(TimeRemaining);
        string str = time.Hours + ":" + time.Minutes + ":" + time.Seconds;
        return str;
    }

}
