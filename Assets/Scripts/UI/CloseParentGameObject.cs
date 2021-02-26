using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kai
{
    public class CloseParentGameObject : MonoBehaviour
    {
        public void OnCLoseButtonClicked()
        {
            SoundManager.Instance.PlaySe("cancel");
            transform.parent.gameObject.SetActive(false);
        }
    }
}