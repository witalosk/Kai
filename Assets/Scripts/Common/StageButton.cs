using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Kai
{
    public class StageButton : MonoBehaviour
    {
        [SerializeField]
        int _world, _stage;

        [SerializeField]
        GameObject _clearedText, _button, _stageNumText;

        void Start()
        {
            bool isCleared = SaveManager.Instance.GetStageClear(_world, _stage);
            bool isBeforeStageCleared = SaveManager.Instance.GetStageClear(_world, _stage - 1);

            _clearedText.SetActive(isCleared);
            _button.GetComponent<Button>().interactable = isBeforeStageCleared;
            _button.GetComponent<Button>().onClick.AddListener(OnButtonClicked);

            if (_stage == 1) {
                _button.GetComponent<Button>().interactable = true;
            }

            _stageNumText.GetComponent<TextMeshProUGUI>().text = _stage.ToString();
        }

        public void OnButtonClicked()
        {
            SoundManager.Instance.PlaySe("select");
            GameController._worldNum = _world;
            GameController._stageNum = _stage;
            SceneManager.LoadScene("Main");
        }
        
    }
}