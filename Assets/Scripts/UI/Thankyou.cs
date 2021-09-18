using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kai
{
    public class Thankyou : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (!SaveManager.Instance.GetStageClear(1, 12)) {
                gameObject.SetActive(false);
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}