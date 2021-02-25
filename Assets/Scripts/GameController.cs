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
        Rotating
    }

    public class GameController : MonoBehaviour
    {

        public const int MAX_LENGTH = 7;
        public const float PIECE_SIZE = 100f;

        public string _csvPath = "Stages/0101";

        public float _moveAmount = 0; // ドラッグ回転量， 6で1周
        public int _beforeMoveBlockAmount = 0;
        public int _selectedRing = 0;
        public GameStatus _currentGameStatus = GameStatus.Idle;



        public List<int[]> _board = new List<int[]>();
        public List<int[]> _optionBoard = new List<int[]>();
        public List<GameObject[]> _gBoard = new List<GameObject[]>();

        List<List<Vector2Int>> _transformList = new List<List<Vector2Int>>();

        [SerializeField]
        GameObject _piecePrefab;




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



                    foreach (var gm in temp) {
                        gm.GetComponent<Piece>().SetBoard();
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

        /// <summary>
        /// ゲームを開始
        /// </summary>
        void StartGame()
        {
            ReadCSV(_csvPath);

        }

        /// <summary>
        /// ステージ情報読み込み
        /// </summary>
        void ReadCSV(string path)
        {
            var csvFile = Resources.Load(path) as TextAsset;
            StringReader reader = new StringReader(csvFile.text);

            while (reader.Peek() != -1)
            {
                string line = reader.ReadLine();
                var arr = line.Split(',');
                int[] res = new int[MAX_LENGTH];
                int[] op = new int[MAX_LENGTH];
                int count = 0;
                foreach (string txt in arr) {
                    // オプションを分割
                    var typeAndOption = txt.Split('-');
                    res[count] = int.Parse(typeAndOption[0]);
                    
                    if (typeAndOption.Length > 1) {
                        op[count] = int.Parse(typeAndOption[1]);
                    }
                    else {
                        op[count] = 0;
                    }
                    count++;
                }

                _board.Add(res);
                _optionBoard.Add(op);
                _gBoard.Add(new GameObject[MAX_LENGTH]);
            }

            // ブロック配置
            for (int i = 0; i < MAX_LENGTH; i++) {
                for (int j = 0; j < MAX_LENGTH; j++) {
                    GameObject newPiece = Instantiate(_piecePrefab);
                    newPiece.transform.SetParent(transform);
                    newPiece.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)i * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE, (float)j * PIECE_SIZE - MAX_LENGTH / 2 * PIECE_SIZE);
                    newPiece.GetComponent<Piece>()._pieceType = (PieceType)_board[i][j];
                    newPiece.GetComponent<Piece>()._pieceCood = new Vector2Int(i, j);
                    newPiece.GetComponent<Piece>()._pieceOption = _optionBoard[i][j];
                    _gBoard[i][j] = newPiece;
                }
            }

        }

        public void MouseButtonDowned()
        {

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