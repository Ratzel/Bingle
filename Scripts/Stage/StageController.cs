using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Stage
{
    public class StageController : MonoBehaviour
    {
        [SerializeField] Transform _rootObj;
        [SerializeField] GameObject _cellPrefab;
        [SerializeField] GameObject _blockPrefab;


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
            // _stage.PrintAll();
        }

        void BuildStage()
        {
            //1. Stage 를 구성한다. 
            _stage = StageBuilder.BuildStage(nStage : 0, nRow : 3, nCol : 3);

            //2. 생성한 stage 정보를 이용하여 씬ㅇㄹ 구성한다. 
            _stage.ComposeStage(_cellPrefab, _blockPrefab, _rootObj);
            
        }
        
    }
}


