using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kai
{
    public class SaveKeys
    {
        public const string STAGE_CLEAR_ = "STAGE_CLEAR_";
        public const string INTRODUCTION = "INTRODUCTION";
    }

    public class SaveManager : SingletonMonoBehaviour<SaveManager>
    {
        override protected void doAwake()
        {

        }

        public void SetStageClear(int world, int stage)
        {
            PlayerPrefs.SetInt(SaveKeys.STAGE_CLEAR_ + world.ToString() + "_" + stage.ToString(), 1);
            PlayerPrefs.Save();
        }

        public bool GetStageClear(int world, int stage)
        {
            return PlayerPrefs.GetInt(SaveKeys.STAGE_CLEAR_ + world.ToString() + "_" + stage.ToString(), 0) == 1;
        }

        public void SetFlag(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public int GetFlag(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }
}