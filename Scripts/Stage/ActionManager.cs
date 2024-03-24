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

                SoundManager.instance.PlayOneShot(Clip.Chomp);

                //1. swipe action 수행
                Returnable<bool> isSwipeBlock = new Returnable<bool>(false);
                yield return _stage.CoDoSwipeAction(nRow, nCol, swipeDir, isSwipeBlock);

                //2. 스와이프 성공한 경우 보드를 평가(매치블럭삭제, 빈블럭 드롭, 새블럭 Spawn 등)한다.
                if(isSwipeBlock.value)
                {
                    Returnable<bool> isMatchBlock = new Returnable<bool>(false);
                    yield return EvaluteBoard(isMatchBlock);

                    //스와이프한 블럭이 매치되지 않은 경우에 원상태 복귀
                    if(!isMatchBlock.value)
                    {
                        yield return _stage.CoDoSwipeAction(nRow,nCol, swipeDir, isSwipeBlock);
                    }
                }

                _isRunning = false; //액션 실행 상태 off 
            }
         }

         IEnumerator EvaluteBoard(Returnable<bool> matchResult)
         {
            bool isFirst = true;

            while(true) //매칭된 블럭이 있는 경우 반복 수행한다.
            {
                //1. 매치 블럭 제거 
                Returnable<bool> isBlockMatched = new Returnable<bool>(false);
                yield return StartCoroutine(_stage.Evaluate(isBlockMatched));

                //2. 3매치 블럭이 있는 경우 후처리 실행(블럭 드롭 등)
                if(isBlockMatched.value)
                {
                    matchResult.value = true;
                    SoundManager.instance.PlayOneShot(Clip.BlockClear);
                    
                    //매칭 블럭 제거 후 빈블럭 드롭 후 새 블럭 생성
                    yield return StartCoroutine(_stage.PostProcessAfterEvaluate());
                }
                else
                {
                    //3. 3매치 블럭이 없는 경우 while문 종료 
                    break;
                }

                
            }

            yield break;
        }
    }
}

