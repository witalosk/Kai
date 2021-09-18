using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kai
{
    public enum PieceType
    {
        NONE = 0,
        CANNON = 1,
        TRESURE = 2,
        MIRROR = 3,
        STONE = 4,
        IRON = 5,
        SLIME = 6,
        DOKAN = 7,
        WAREMIRROR = 8,
    }
    
    public class Piece : MonoBehaviour
    {

        // SerializeField
        [SerializeField]
        List<Sprite> _pieceSprites = default;

        [SerializeField]
        GameObject _prohibitedMark;

        // public
        public PieceType _pieceType = default;
        public Vector2Int _pieceCood = default;
        public int _pieceOption = default;


        // private
        GameController _gameController;
        Vector2Int _mouseDownCood;
        float _previousRad = 0f;
        float _tan = 0f;

        /// <summary>
        /// 開始時
        /// </summary>
        void Start()
        {
            _gameController = transform.parent.gameObject.GetComponent<GameController>();

            SetPieceType(_pieceType);
            
        }

        /// <summary>
        /// ピースタイプを設定
        /// </summary>
        /// <param name="pt"></param>
        public void SetPieceType(PieceType pt)
        {
            _pieceType = pt;

            // 画像設定
            GetComponent<Image>().sprite = _pieceSprites[(int)pt];

            switch(pt)
            {
                case PieceType.CANNON:
                case PieceType.MIRROR:
                case PieceType.DOKAN:
                case PieceType.WAREMIRROR:
                    if (_pieceOption != 0) {
                        transform.localRotation = Quaternion.Euler(0f, 0f, 90f * _pieceOption);
                    }
                    break;
            }

            _gameController._board[_pieceCood.x][_pieceCood.y] = (int)pt;

            if (GetRingNum() != 0 && _gameController._prohibitedRing == GetRingNum()) {
                SetProhibited(true);
            }

        }

        public void SetProhibited(bool isProhibited)
        {
            _prohibitedMark.SetActive(isProhibited);
        }

        /// <summary>
        /// 座標をもとにBoardを設定
        /// </summary>
        public void SetBoard()
        {
            _gameController._board[_pieceCood.x][_pieceCood.y] = (int)_pieceType;
            _gameController._gBoard[_pieceCood.x][_pieceCood.y] = gameObject;
        }

        /// <summary>
        /// ボード上の座標でマウス位置を返す
        /// </summary>
        /// <returns></returns>
        Vector2 MousePositionInAnchoredPos()
        {
            RectTransform rect = transform.parent.gameObject.GetComponent<RectTransform>();

            // Canvasにセットされているカメラを取得
            var graphic = GetComponent<Graphic>();
            var camera = graphic.canvas.worldCamera;

            // Overlayの場合はScreenPointToLocalPointInRectangleにnullを渡さないといけないので書き換える
            if (graphic.canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                camera = null;
            } 

            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, camera, out result);

            Vector2 offset = new Vector2(Screen.width * (0.5f - rect.anchorMin.x), Screen.height * (0.5f - rect.anchorMin.y));

            return result + offset;
        }

        int GetRingNum()
        {
            return Mathf.Max( Mathf.Abs(_pieceCood.x - (GameController.MAX_LENGTH / 2)), Mathf.Abs(_pieceCood.y - (GameController.MAX_LENGTH / 2)) );
        }

        public void MouseDown()
        {
            if (_gameController._prohibitedRing == GetRingNum()) return;
            // Debug.Log(_pieceCood);
            // Debug.Log(_pieceType);
            // Debug.Log((PieceType)_gameController._board[_pieceCood.x][_pieceCood.y]);
            SoundManager.Instance.PlaySe("move");
            
            _mouseDownCood = new Vector2Int(_pieceCood.x, _pieceCood.y);
            Debug.Log(_mouseDownCood);
            float rad = Mathf.Atan2(MousePositionInAnchoredPos().x, MousePositionInAnchoredPos().y);

            _gameController._selectedRing = GetRingNum();
            _tan = 0f;
            _gameController._beforeMoveBlockAmount = 0;
            _previousRad = rad;
            

            _gameController._currentGameStatus = GameStatus.Rotating;
        }

        public void MouseDrag()
        {
            if (_gameController._prohibitedRing == GetRingNum()) return;

            float rad = Mathf.Atan2(MousePositionInAnchoredPos().x, MousePositionInAnchoredPos().y);
            _tan += Mathf.Tan(rad - _previousRad);
            _gameController._moveAmount = _tan;

            _previousRad = rad;
        }

        public void MouseDragEnd()
        {
            if (_gameController._prohibitedRing == GetRingNum()) return;

            _gameController._currentGameStatus = GameStatus.Idle;
            _gameController._moveAmount = 0f;
            _previousRad = 0f;
        }
    }
}