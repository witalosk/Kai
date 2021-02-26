using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kai
{
    
    public class WorldSelectUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.PlayBgm("opening");
        }

        public void StartWorld(int world)
        {
            SoundManager.Instance.PlaySe("select");
            GameController._worldNum = world;
            GameController._stageNum = 1;
            SceneManager.LoadScene("Main");
        }

        public void ToTitle()
        {
            SceneManager.LoadScene("Title");
        }
    }

}