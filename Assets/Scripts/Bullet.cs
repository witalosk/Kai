using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Kai
{



    /// <summary>
    /// 弾丸用
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        const float THRESHOLD = 10.0f;
        const float SPEED = 300f;

        /// <summary>
        /// 座標
        /// </summary>
        public Vector2Int _bulletCood;

        /// <summary>
        /// 速度
        /// </summary>
        public Vector3 _bulletVelocity = new Vector3(0f, 0f, 0f);

        bool _isMoving = false;
        Vector2Int _lastPassedCell = new Vector2Int(-99, -99); // 最後に通ったマス


        public GameController _gameController;

        void Start()
        {

        }

        void Update()
        {
            if (_isMoving) {
                float offset = (float)(GameController.MAX_LENGTH / 2) * GameController.PIECE_SIZE;
                Vector3 anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                Vector2Int currentCood = new Vector2Int((int)((anchoredPosition.x + offset + GameController.PIECE_SIZE/2) / GameController.PIECE_SIZE), (int)((anchoredPosition.y + offset + GameController.PIECE_SIZE/2) / GameController.PIECE_SIZE));
                
                // セルの中央に来たことを判定する
                if ((anchoredPosition.x + 100f * GameController.MAX_LENGTH) % GameController.PIECE_SIZE <= THRESHOLD 
                && (anchoredPosition.y + 100f * GameController.MAX_LENGTH) % GameController.PIECE_SIZE <= THRESHOLD
                && currentCood != _lastPassedCell) {
                    GetComponent<RectTransform>().anchoredPosition = new Vector3(((float)currentCood.x * GameController.PIECE_SIZE - offset), (float)currentCood.y * GameController.PIECE_SIZE - offset, 0f);

                    if (currentCood.x >= GameController.MAX_LENGTH || currentCood.x < 0
                    || currentCood.y >= GameController.MAX_LENGTH || currentCood.y < 0 ) {
                        DeleteThis();
                        return;
                    }

                    // ピースの種類によって場合分け
                    switch ((PieceType)_gameController._board[currentCood.x][currentCood.y]) {
                        case PieceType.IRON:
                            SoundManager.Instance.PlaySe("break");
                            DeleteThis();
                            break;
                        case PieceType.STONE:
                            _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>().SetPieceType(PieceType.NONE);
                            _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>().SetBoard();
                            SoundManager.Instance.PlaySe("break");
                            DeleteThis();
                            break;
                        case PieceType.WAREMIRROR:
                            SoundManager.Instance.PlaySe("mirror");
                            int op3 = _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>()._pieceOption;
                            if (_bulletVelocity.x > 0) {
                                _bulletVelocity = new Vector3(0f, 1f, 0f);
                            }
                            else if (_bulletVelocity.x < 0) {
                                _bulletVelocity = new Vector3(0f, -1f, 0f);
                            }
                            else if (_bulletVelocity.y > 0) {
                                _bulletVelocity = new Vector3(1f, 0f, 0f);
                            }
                            else if (_bulletVelocity.y < 0) {
                                _bulletVelocity = new Vector3(-1f, 0f, 0f);
                            }
                            if (op3 == 1 || op3 == 3) {
                                _bulletVelocity *= -1;
                            }
                            _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>().SetPieceType(PieceType.NONE);
                            _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>().SetBoard();
                            SoundManager.Instance.PlaySe("break");
                            break;
                        case PieceType.MIRROR:
                            SoundManager.Instance.PlaySe("mirror");
                            int op = _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>()._pieceOption;
                            if (_bulletVelocity.x > 0) {
                                _bulletVelocity = new Vector3(0f, 1f, 0f);
                            }
                            else if (_bulletVelocity.x < 0) {
                                _bulletVelocity = new Vector3(0f, -1f, 0f);
                            }
                            else if (_bulletVelocity.y > 0) {
                                _bulletVelocity = new Vector3(1f, 0f, 0f);
                            }
                            else if (_bulletVelocity.y < 0) {
                                _bulletVelocity = new Vector3(-1f, 0f, 0f);
                            }
                            if (op == 1 || op == 3) {
                                _bulletVelocity *= -1;
                            }
                            
                            break;
                        case PieceType.TRESURE:
                            SoundManager.Instance.PlaySe("tresure");
                            DeleteThis();
                            SaveManager.Instance.SetStageClear(GameController._worldNum, GameController._stageNum);
                            _gameController._mainUIController.GameClear();
                            break;
                        case PieceType.SLIME:
                            SoundManager.Instance.PlaySe("slime");
                            _bulletVelocity*=-1;
                            break;
                        case PieceType.DOKAN:
                            int op2 = _gameController._gBoard[currentCood.x][currentCood.y].GetComponent<Piece>()._pieceOption;
                            if ((op2 == 0 || op2 == 2) && _bulletVelocity.x != 0f) {
                                SoundManager.Instance.PlaySe("break");
                                DeleteThis();
                            }
                            else if ((op2 == 1 || op2 == 3) && _bulletVelocity.y != 0f) {
                                SoundManager.Instance.PlaySe("break");
                                DeleteThis();
                            }
                            SoundManager.Instance.PlaySe("dokan");
                            break;

                    }


                    _lastPassedCell = currentCood;
                }

                // 移動
                GetComponent<RectTransform>().anchoredPosition = anchoredPosition + _bulletVelocity * SPEED * Time.deltaTime;
            }
        }

        /// <summary>
        /// 球を破壊
        /// </summary>
        private void DeleteThis()
        {
            _gameController._currentGameStatus = GameStatus.Idle;
            _isMoving = false;
            
            Destroy(gameObject);
        }

        public void StartMove()
        {
            _gameController._currentGameStatus = GameStatus.Moving;
            _isMoving = true;
        }
    }

}