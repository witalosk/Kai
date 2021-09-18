

using UnityEngine;
using System.Collections;

namespace Kai
{
    public class BackgroundScroller : MonoBehaviour
    {
        [SerializeField]
        float _speed = -0.1f;

        [SerializeField]
        float _width = 1920f;

        Vector3 _initPos;
        void Start()
        {
            _initPos = GetComponent<RectTransform>().anchoredPosition;
            Debug.Log(Screen.width);
        }

        void Update () {
            transform.Translate (_speed, 0, 0);
            if (GetComponent<RectTransform>().anchoredPosition.x < -_width) {
                GetComponent<RectTransform>().anchoredPosition = new Vector3 (_width, _initPos.y, 0);
            }
            // else if (_speed > 0f && transform.position.x > _width) {
            //     transform.position = new Vector3 (-_width, _initPos.y, 0);
            //     Debug.Log(Screen.width);


            // }
        }
    }
}