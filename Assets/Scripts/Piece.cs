﻿using System.Collections;
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
        IRON = 5
    }
    
    public class Piece : MonoBehaviour
    {

        
        [SerializeField]
        List<Sprite> _pieceSprites = default;

        public PieceType _pieceType = default;

        public Vector2Int _pieceCood = default;

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

            // 画像設定
            GetComponent<Image>().sprite = _pieceSprites[(int)_pieceType];
            
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
            _mouseDownCood = new Vector2Int(_pieceCood.x, _pieceCood.y);

            float rad = Mathf.Atan2(MousePositionInAnchoredPos().x, MousePositionInAnchoredPos().y);

            _gameController._selectedRing = GetRingNum();
            _tan = 0f;
            _gameController._beforeMoveBlockAmount = 0;
            _previousRad = rad;
            

            _gameController._currentGameStatus = GameStatus.Rotating;
        }

        public void MouseDrag()
        {
            float rad = Mathf.Atan2(MousePositionInAnchoredPos().x, MousePositionInAnchoredPos().y);
            _tan += Mathf.Tan(rad - _previousRad);
            _gameController._moveAmount = _tan;

            _previousRad = rad;
        }

        public void MouseDragEnd()
        {
            _gameController._currentGameStatus = GameStatus.Idle;
            _gameController._moveAmount = 0f;
            _previousRad = 0f;
        }
    }
}