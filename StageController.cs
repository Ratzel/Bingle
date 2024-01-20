using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Stage
{
    public class StageController : MonoBehaviour
    {
        bool _isInit;
        Stage _stage;


        // Start is called before the first frame update
        void Start()
        {
            InitStage();
        }

        void InitStage()
        {
            if (_isInit)
            {
                return;
            }
            _isInit = true;
        }
    }
}


