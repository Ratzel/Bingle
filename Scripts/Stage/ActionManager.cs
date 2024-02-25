using UnityEngine;
using Dafhne.Util;
using System.Collections;
using System;

namespace Dafhne.Stage
{
    public class ActionManager
    {
        Transform _rootObj; //컨테이너 (Board GameObject)
        Stage _stage; 
        MonoBehaviour _monoBehaviour; //코루틴 호출시 필요한 MonoBehaviour
        bool _isRunning; //액션 실행 상태 : 싱행중인 경우 true

        public ActionManager(Transform rootObj, Stage stage)
        {
            _rootObj = rootObj;
            _stage = stage;

            _monoBehaviour = rootObj.gameObject.GetComponent<MonoBehaviour>();
        }

        //코루틴 wrapper 메소드
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _monoBehaviour.StartCoroutine(routine);
        }

        //스와이프 액션을 시작한다. 
        //@param nRow, nCol - 블럭 위치
        //@swipeDir - 스와이프 방향

        public void DoSwipeAction(int nRow, int nCol, Swipe swipeDir)
        {
            Debug.Assert(nRow >= 0 && nRow < _stage.MaxRow && nCol >= 0 && nCol < _stage.MaxCol);
            
            if(_stage.IsValideSwipe(nRow,nCol,swipeDir))
            {
                StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
            }
        }

        //스와이프 액션을 수행하는 코루틴 
         IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir)
         {
            if(!_isRunning) //다른 액션이 수행 중이면 Pass
            {
                _isRunning = true; //액션 실행 상태 On

                //1. swipe action 수행
                Returnable<bool> isSwipeBlock = new Returnable<bool>(false);
                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, isSwipeBlock);

                _isRunning = false; //액션 실행 상태 off 
            }
         }
    }
}

