using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Kai
{
    
    public enum GameStatus
    {
        Idle,
        Rotating,
        Moving
    }

    public class GameController : MonoBehaviour
    {

        public static int _worldNum = 1;
        public static int _stageNum = 1;

        public const int MAX_LENGTH = 7;
        public const float PIECE_SIZE = 100f;

        public string _csvPath = "1-1";

        public float _moveAmount = 0; // ドラッグ回転量， 6で1周
        public int _beforeMoveBlockAmount = 0;
        public int _selectedRing = 0;
        public int _prohibitedRing = 0; // 回転禁止リング
        public int _bulletNum = 0; // 弾薬数
        public GameStatus _currentGameStatus = GameStatus.Idle;



        public List<int[]> _board = new List<int[]>();
        public List<GameObject[]> _gBoard = new List<GameObject[]>();
        public List<int[]> _PlaceBoard = new List<int[]>(); // その場所の特性


        List<List<Vector2Int>> _transformList = new List<List<Vector2Int>>();

        [SerializeField]
        GameObject _piecePrefab, _bulletPrefab, _fireButton;

        [SerializeField]
        public MainUIController _mainUIController;

        [SerializeField]
        List<GameObject> _howToPlay = new List<GameObject>();

        string _stageSubTitle = "";
        GameObject _bullet;




        void Start()
        {
            StartGame();

            // transformListを初期化
            InitTransList();
        }

        void Update()
        {

            if (_currentGameStatus == GameStatus.Rotating) {
                int moveBlockAmount = (int)(_moveAmount / (Mathf.PI*2f / (((2*_selectedRing + 1) - 1) * 4)));
                
                
                if (moveBlockAmount != _beforeMoveBlockAmount) {
                    //Debug.Log(moveBlockAmount);
                    int count = 0;
                    int ippen = 2*_selectedRing;
                    
                    // 増えた場合
                    if (moveBlockAmount < _beforeMoveBlockAmount) {
                        foreach (var pos in _transformList[_selectedRing]) {
                            if (count != _transformList[_selectedRing].Count - 1) {
                                MovePiece(_gBoard[pos.x][pos.y], _transformList[_selectedRing][count+1]);
                            }
                            else {
                                MovePiece(_gBoard[pos.x][pos.y], _transformList[_selectedRing][0]);
                            }
                            count++;
                        }
                    }
                    // 減った場合
                    else {
                        foreach (var pos in _transformList[_selectedRing]) {
                            if (count == 0) {
                                MovePiece(_gBoard[pos.x][pos.y], _transformList[_selectedRing][_transformList[_selectedRing].Count - 1]);
                            }
                            else {
                                MovePiece(_gBoard[pos.x][pos.y], _transformList[_selectedRing][count - 1]);
                            }
                            count++;
                        }
                    }

                    // 配列へ設定
                    List<GameObject> temp = new List<GameObject>();
                    foreach (var list in _gBoard) {
                        foreach (var gm in list) {
                            temp.Add(gm);
                        }
                    }

                    SoundManager.Instance.PlaySe("move");

                    foreach (var gm in temp) {
                        gm.GetComponent<Piece>().SetBoard();
                        
                        if (gm.GetComponent<Piece>()._pieceType == PieceType.CANNON) {
                            if (_PlaceBoard[gm.GetComponent<Piece>()._pieceCood.x][gm.GetComponent<Piece>()._pieceCood.y] != 0) {
                                _mainUIController.FireButtonToggle(false);
                                gm.GetComponent<Piece>().SetProhibited(true);
                            }
                            else {
                                _mainUIController.FireButtonToggle(true);
                                gm.GetComponent<Piece>().SetProhibited(false);
                            }
                        }
                    }
                    _beforeMoveBlockAmount = moveBlockAmount;
                }
            }
            
        }

        /// <summary>
        /// ピースの移動
        /// </summary>
        /// <param name="oldPos"></param>
        /// <param name="newPos"></param>
        void MovePiece(GameObject gm, Vector2Int newPos)
        {
            gm.GetComponent<RectTransform>().anchoredPosition = new Vector2(100f * newPos.x - 300f, 100f * newPos.y - 300f);
            gm.GetComponent<Piece>()._pieceCood = newPos;
        }

        public void PlayBGM()
        {
            if (_stageNum > 11) {
                SoundManager.Instance.PlayBgm("opening");
                return;
            }
            // sound
            int soundnum = 1;
            Debug.Log(_stageNum);
            if (_stageNum > 6) {
                soundnum = 2;
            }
            if (_stageNum > 9) {
                soundnum = 3;
            }

            Debug.Log("stageNum: "+ _stageNum);
            SoundManager.Instance.PlayBgm("stage" + soundnum.ToString());

        }

        /// <summary>
        /// ゲームを開始
        /// </summary>
        public void StartGame()
        {
            PlayBGM();
            _csvPath = _worldNum.ToString() + "-" + _stageNum.ToString();

            ReadCSV("Stages/" + _csvPath);

            if (_csvPath=="1-1") {
                ShowHowToPlay(0);
            }
            else if (_csvPath=="1-3") {
                ShowHowToPlay(1);
            }
            else if (_csvPath=="1-4") {
                ShowHowToPlay(2);
            }
            else if (_csvPath=="1-6") {
                ShowHowToPlay(3);
            }
            else if (_csvPath=="1-7") {
                ShowHowToPlay(4);
            }
        }

        public void ResetGame()
        {
            // if (_currentGameStatus != GameStatus.Idle) {
            //     SoundManager.Instance.PlaySe("cancel");
            //     return;
            // }
            if (_bullet != null) {
                Destroy(_bullet);
            }

            SoundManager.Instance.PlaySe("select");
            _csvPath = _worldNum.ToString() + "-" + _stageNum.ToString();
            foreach (var golist in _gBoard) {
                foreach (var go in golist) {
                    Destroy(go);
                }
            }
            _gBoard.Clear();
            _board.Clear();
            _PlaceBoard.Clear();

            _moveAmount = 0; // ドラッグ回転量， 6で1周
            _beforeMoveBlockAmount = 0;
            _selectedRing = 0;
            _prohibitedRing = 0; // 回転禁止リング
            _bulletNum = 0; // 弾薬数
            _currentGameStatus = GameStatus.Idle;

            ReadCSV("Stages/" + _csvPath);

        }

        /// <summary>
        /// ステージ情報読み込み
        /// </summary>
        void ReadCSV(string path)
        {
            Debug.Log(path);
            var csvFile = Resources.Load(path) as TextAsset;
            StringReader reader = new StringReader(csvFile.text);
            List<int[]> optionBoard = new List<int[]>();

            int linecount = 0;

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                var arr = line.Split(',');

                if (linecount < MAX_LENGTH) {
                    int[] res = new int[MAX_LENGTH];
                    int[] op = new int[MAX_LENGTH];
                    int[] place = new int[MAX_LENGTH];
                    int count = 0;
                    foreach (string txt in arr) {
                        // オプションを分割
                        var typeAndOption = txt.Split('-');
                        res[count] = int.Parse(typeAndOption[0]);
                        
                        if (typeAndOption.Length == 2) {
                            op[count] = int.Parse(typeAndOption[1]);
                            place[count] = 0;
                        }
                        else if (typeAndOption.Length == 3) {
                            op[count] = int.Parse(typeAndOption[1]);
                            place[count] = int.Parse(typeAndOption[2]);
                        }
                        else {
                            op[count] = 0;
                            place[count] = 0;
                        }
                        count++;
                    }

                    _board.Add(res);
                    optionBoard.Add(op);
                    _PlaceBoard.Add(place);
                    _gBoard.Add(new GameObject[MAX_LENGTH]);
                }
                else if (linecount == MAX_LENGTH) {
                    // タイトル
                    _stageSubTitle = arr[0];
                    _mainUIController.SetStageTitle(_stageNum.ToString(), _stageSubTitle);
                }
                else if (linecount == MAX_LENGTH+1) {
                    // 弾薬数
                    _bulletNum = int.Parse(arr[0]);
                    _mainUIController.UpdateBulletNum();
                }
                else if (linecount == MAX_LENGTH+2) {
                    // 回転禁止リング
                    _prohibitedRing = int.Parse(arr[0]);
                }


                linecount++;
            }

            // ブロック配置
            for (int i = 0; i < MAX_LENGTH; i++) {
                for (int j = 0; j < MAX_LENGTH; j++) {
                    GameObject newPiece = Instantiate(_piecePrefab);
                    newPiece.transform.SetParent(transform);
                    newPiece.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)i * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE, (float)j * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE);
                    newPiece.transform.localScale = new Vector3(1f, 1f, 1f);
                    newPiece.GetComponent<Piece>()._pieceType = (PieceType)_board[i][j];
                    newPiece.GetComponent<Piece>()._pieceCood = new Vector2Int(i, j);
                    newPiece.GetComponent<Piece>()._pieceOption = optionBoard[i][j];
                    _gBoard[i][j] = newPiece;
                }
            }

        }

        /// <summary>
        /// 弾丸を配置
        /// </summary>
        public void FireBullet()
        {
            if (_currentGameStatus != GameStatus.Idle || _bulletNum == 0){
                SoundManager.Instance.PlaySe("cancel");
                return;
            } 



            for(int i = 0; i < MAX_LENGTH; i++) {
                for (int j = 0; j < MAX_LENGTH; j++) {
                    if (_board[i][j] == (int)PieceType.CANNON) {
                        // 砲台禁止エリアの確認
                        if (_PlaceBoard[i][j] != 0) return;

                        SoundManager.Instance.PlaySe("fire");
                        _bulletNum -= 1;
                        _mainUIController.UpdateBulletNum();

                        // 弾丸を配置
                        GameObject newBullet = Instantiate(_bulletPrefab);
                        newBullet.transform.SetParent(transform);
                        newBullet.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)i * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE, (float)j * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE);
                        newBullet.GetComponent<Bullet>()._bulletCood = new Vector2Int(i, j);
                        newBullet.GetComponent<Bullet>()._gameController = this;
                        newBullet.GetComponent<RectTransform>().localScale = new Vector3(1f,1f,1f);
                        _bullet = newBullet;
                        // 大砲の方向によって初期速度を変更
                        int option = _gBoard[i][j].GetComponent<Piece>()._pieceOption;
                        switch(option) {
                            case 0:
                                 newBullet.GetComponent<Bullet>()._bulletVelocity = new Vector3(0f, 1f, 0f);
                                 break;
                            case 1:
                                 newBullet.GetComponent<Bullet>()._bulletVelocity = new Vector3(-1f, 0f, 0f);
                                 break;
                            case 2:
                                 newBullet.GetComponent<Bullet>()._bulletVelocity = new Vector3(0f, -1f, 0f);
                                 break;
                            case 3:
                                 newBullet.GetComponent<Bullet>()._bulletVelocity = new Vector3(1f, 0f, 0f);
                                 break;
                        }

                        _currentGameStatus = GameStatus.Moving;
                        newBullet.GetComponent<Bullet>().StartMove();

                        break;
                    }
                }
            }

        }

        public void ShowHowToPlay(int num)
        {
             _howToPlay[num].SetActive(true);
        }


        private void InitTransList()
        {
            _transformList.Add(new List<Vector2Int>() {new Vector2Int(3,3)});
            _transformList.Add(new List<Vector2Int>() {new Vector2Int(2,2), new Vector2Int(3,2), new Vector2Int(4,2), new Vector2Int(4,3), new Vector2Int(4,4), new Vector2Int(3,4), new Vector2Int(2,4), new Vector2Int(2,3) });
            _transformList.Add(new List<Vector2Int>() {
                new Vector2Int(1,1), new Vector2Int(2,1), new Vector2Int(3,1), new Vector2Int(4,1), new Vector2Int(5,1), new Vector2Int(5,2), new Vector2Int(5,3), new Vector2Int(5,4), new Vector2Int(5,5),
                new Vector2Int(4,5), new Vector2Int(3,5), new Vector2Int(2,5), new Vector2Int(1,5), new Vector2Int(1,4), new Vector2Int(1,3), new Vector2Int(1,2)
            });
            _transformList.Add(new List<Vector2Int>() {
                new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0), new Vector2Int(4,0), new Vector2Int(5,0), new Vector2Int(6,0),
                new Vector2Int(6,1), new Vector2Int(6,2), new Vector2Int(6,3), new Vector2Int(6,4), new Vector2Int(6,5), new Vector2Int(6,6),
                new Vector2Int(5,6), new Vector2Int(4,6), new Vector2Int(3,6), new Vector2Int(2,6), new Vector2Int(1,6), new Vector2Int(0,6),
                new Vector2Int(0,5), new Vector2Int(0,4), new Vector2Int(0,3), new Vector2Int(0,2), new Vector2Int(0,1)
            });
            
        }

    }
}