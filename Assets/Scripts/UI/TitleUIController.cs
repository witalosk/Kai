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
            SceneManager.LoadScene("Opening");
        }
    }

}
