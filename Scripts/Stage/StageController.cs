using Dafhne.Board;
using Dafhne.Util;
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
        InputManager _inputManager;
        
        //Members for Event 
        bool _isTouchDown; //입력상태 처리 플래그, 유효한 블럭을 클릭한 경우 true
        BlockPos _blcokDownPos; //블럭 위치 (보드에 저장된 위치)
        Vector3 _clickPos; //DOWN 위치(보드 기준 Local 좌표)
        

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
            _inputManager = new InputManager(_rootObj);

            //최초스테이지 
            BuildStage();

            //디버깅
            // _stage.PrintAll();
        }

        void BuildStage()
        {
            //1. Stage 를 구성한다. 
            _stage = StageBuilder.BuildStage(nStage : 1);

            //2. 생성한 stage 정보를 이용하여 씬ㅇㄹ 구성한다. 
            _stage.ComposeStage(_cellPrefab, _blockPrefab, _rootObj);
            
        }
        
        void Update()
        {
            if(!_isInit)
                return;

            OnInputHandler();
        }

        void OnInputHandler()
        {
            //TouchDown
            if(!_isTouchDown && _inputManager.isTouchDown)
            {
                //1.1 보드 기준 Local 좌표를 구한다. 
                Vector2 point = _inputManager.touch2BoardPosition;

                //1.2 Play 영역(보드)에서 클릭하지 않는 경우 무시
                if(!_stage.IsInsideBoard(point))
                {
                    return;
                }

                //1.3 클릭한 위치의 블럭을 구한다. 
                BlockPos blockPos;
                if(_stage.IsOnValideBlcok(point, out blockPos))
                {
                    //1.3.1 유효한(스와이프가 가능한) 블럭에서 클릭한 경우 
                    _isTouchDown = true; // 클릭 상태 플래그 ON
                    _blcokDownPos = blockPos; // 클릭한 블럭의 위치(row, col) 저장
                    _clickPos = point; // 클릭한 Local 좌표 저장
                    // Debug.Log($"Mouse Down In Board : {blockPos}");
                }
                // Debug.Log($"Input Down = {point}, local = {_inputManager.touch2BoardPosition }");
            }
            else if(_isTouchDown && _inputManager.isTouchUp) //2.Touch UP : 유효한 블럭 위에서 Down 후에 발생하는 경우
            {
                //2.1 보드 기준 Local 좌표를 구한다. 
                Vector2 point = _inputManager.touch2BoardPosition;

                //2.2 스와이프 방향을 구한다. 
                Swipe swipeDir = _inputManager.EvalSwipeDir(_clickPos, point);
                
                Debug.Log($"Swipe = {swipeDir}, Block = {_blcokDownPos}");
                
                _isTouchDown = false;
                
            }
        }
    }
}


