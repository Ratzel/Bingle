using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Stage
{
    public class StageController : MonoBehaviour
    {
        bool _isInit;
        Stage _stage;

        void Start()
        {
            InitStage();
        }

        void InitStage()
        {
            //초기화 체크
            if (_isInit)
            {
                return;
            }
            _isInit = true;

            //최초스테이지 
            BuildStage();

            //디버깅
            _stage.PrintAll();
        }

        void BuildStage()
        {
            _stage = StageBuilder.BuildStage(nStage : 1, nRow : 9, nCol : 9);
        }
        
    }
}


