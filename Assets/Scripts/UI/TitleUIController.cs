using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kai
{
    public class TitleUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.PlayBgm("title");
        }

        public void OnClickStartButton()
        {
            SoundManager.Instance.PlaySe("select");
            if (SaveManager.Instance.GetFlag(SaveKeys.INTRODUCTION, 0) == 0) {
                SaveManager.Instance.SetFlag(SaveKeys.INTRODUCTION, 1);
                SceneManager.LoadScene("Opening");
            }
            else {
                SceneManager.LoadScene("WorldSelect");
            }
        }
    }

}
