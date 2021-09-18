using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Kai
{
    /// <summary>
    /// ボタン押下時に文字もへこませる
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ButtonHekomi : MonoBehaviour
    {

        [SerializeField] float _hekomiAmount = -20f;

        private Button _button;
        private Vector2 _beforePos;
        private bool _beforeEnable;
        private bool _beforeInteractive;


        void Start()
        {
            _beforePos = GetComponent<RectTransform>().anchoredPosition;

            _button = transform.parent.gameObject.GetComponent<Button>();
            _beforeEnable = _button.enabled;
            _beforeInteractive = _button.interactable;
            var trigger = _button.gameObject.AddComponent<EventTrigger>();

            var mouseDown = new EventTrigger.Entry();
            mouseDown.eventID = EventTriggerType.PointerDown;
            mouseDown.callback.AddListener(data => OnMouseDown());
            trigger.triggers.Add(mouseDown);
            
            var mouseUp = new EventTrigger.Entry();
            mouseUp.eventID = EventTriggerType.PointerUp;
            mouseUp.callback.AddListener(data => OnMouseUp());
            trigger.triggers.Add(mouseUp);
        }

        void Update()
        {
            if (_beforeEnable != _button.enabled) {
                if (_button.enabled) {
                    OnMouseUp();
                }
                else {
                    OnMouseDown();
                }
                _beforeEnable = _button.enabled;
            }

            if (_beforeInteractive != _button.interactable) {
                if (_button.interactable) {
                    OnMouseUp();
                }
                else {
                    OnMouseDown();
                }
                _beforeInteractive = _button.interactable;
            }
        }

        public void OnMouseDown()
        {
            if (_button.interactable && _button.enabled)
                GetComponent<RectTransform>().anchoredPosition = _beforePos + new Vector2(0f, _hekomiAmount);
        }

        public void OnMouseUp()
        {
            if (_button.interactable && _button.enabled)
                GetComponent<RectTransform>().anchoredPosition = _beforePos;
        }
    }
}
