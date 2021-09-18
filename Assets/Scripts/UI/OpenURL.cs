using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kai
{
    public class OpenURL : MonoBehaviour
    {
        public void OnButtonClicked()
        {
            string text = "「お宝なんてぶっコワせ！」で、全ステージクリアしました！！\nhttps://unityroom.com/games/otakowa";
            string[] hashtags = { "otakowa", "unity1week" };
            Application.OpenURL($"https://twitter.com/intent/tweet?text={UnityWebRequest.EscapeURL(text)}&hashtags={UnityWebRequest.EscapeURL(string.Join(",", hashtags))}");
        }
    }
}