using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace Kai
{
    public class MainUIController : MonoBehaviour
    {
        [SerializeField]
        GameController _gameController;

        [SerializeField]
        GameObject _bulletSpace, _gameClearCanvas, _fireButton;

        [SerializeField]
        TextMeshProUGUI _stageText, _subtitleText, _clearStageText, _fireText;

        [SerializeField]
        GameObject _bulletPrefab;


        List<GameObject> _bulletList = new List<GameObject>();

        /// <summary>
        /// 弾薬表示をアップデートする
        /// </summary>
        public void UpdateBulletNum()
        {
            foreach(var bullet in _bulletList) {
                Destroy(bullet);
            }

            for (int i = 0; i < _gameController._bulletNum; i++) {
                var bullet = Instantiate(_bulletPrefab);
                bullet.transform.SetParent(_bulletSpace.transform);
                _bulletList.Add(bullet);
            }
        }

        /// <summary>
        /// ステージタイトルを設定
        /// </summary>
        public void SetStageTitle(string worldName, string subtitle)
        {
            _stageText.text = "Stage " + worldName;
            _subtitleText.text = subtitle;
            _clearStageText.text = "Stage " + worldName + " " + subtitle;
        }

        /// <summary>
        /// ゲームクリア演出
        /// </summary>
        public void GameClear()
        {
            SoundManager.Instance.StopBgm();
            _gameClearCanvas.SetActive(true);
            SoundManager.Instance.PlaySe("stageclear");
            Debug.Log("GameClear!");
        }

        /// <summary>
        /// 次のステージへ
        /// </summary>
        public void NextStage()
        {
            _gameClearCanvas.SetActive(false);
            SoundManager.Instance.PlaySe("select");
            SoundManager.Instance.PlayBgm("stage" + GameController._worldNum.ToString());
            GameController._stageNum++;
            _gameController.ResetGame();

            if (_gameController._csvPath=="1-3") {
                _gameController.ShowHowToPlay(1);
            }
            else if (_gameController._csvPath=="1-4") {
                _gameController.ShowHowToPlay(2);
            }
        }

        public void FireButtonToggle(bool isEnable)
        {
            _fireButton.GetComponent<Button>().interactable = isEnable;
            if (isEnable) {
                _fireText.text = "Fire!";
            }
            else {
                _fireText.text = "うてない！";
            }
        }

        public void GoToTitle()
        {
            SoundManager.Instance.PlaySe("cancel");
            SceneManager.LoadScene("WorldSelect");
        }

    }
}