using System.Collections;
using System.Collections.Generic;
using Dafhne.Board;
using Dafhne.Util;
using Dafhne.Core;
using UnityEngine;

namespace Dafhne.Stage
{
    public class Stage
    {
        //Board 크기 저장용 설정
        private int _nCol; //행
        private int _nRow; //열

        public int MaxCol { get { return _nCol; } }
        public int MaxRow { get { return _nRow; } }

        //Board 저장용
        Board.Board _board;
        public Board.Board Board { get { return _board; } }
        
        StageBuilder _stageBuilder;
        
        public Block[,] Blocks { get { return _board.Blocks; } }
        public Cell[,] Cells { get { return _board.Cells; } }
        
        //Stage 생성자
        public Stage(StageBuilder stageBuilder, int row, int col)
        {
            _nRow = row;
            _nCol = col;

            _stageBuilder = stageBuilder;

            _board = new Board.Board(row, col);
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform rootObj)
        {
            _board.ComposeStage(cellPrefab, blockPrefab, rootObj);
        }

        public bool IsOnValideBlcok(Vector2 point, out BlockPos blockPos)
        {
            //1. Local 좌표 -> 보드의 블럭 인덱스로 변환한다. 
            Vector2 pos = new Vector2(point.x + (MaxCol / 2.0f), point.y + (MaxRow / 2.0f));
            int nRow = (int)pos.y;
            int nCol = (int)pos.x;

            //리턴할 블럭 인덱스 생성
            blockPos = new BlockPos(nRow, nCol);

            //2. 스와이프 가능한지 체크한다. 
            return Board.IsSwipeable(nRow, nCol);
        }

        public bool IsInsideBoard(Vector2 pointOrigin)
        {
            //계산의 편의를 위해서 (0,0)을 기준으로 좌표를 이동한다. 
            // 8 x 8 보드인 경우 : x(-4 ~  +4), y(-4 ~ +4) -> x(0 ~ +8), y(0 ~ +8)
            Vector2 point = new Vector2(pointOrigin.x + (MaxCol / 2.0f), pointOrigin.y + (MaxRow / 2.0f));

            if(point.y < 0 || point.x < 0 || point.y > MaxRow || point.x > MaxCol)
            {
                return false;
            }

            return true;
        }

        public IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false; //코루틴 리턴값 Reset 

            //1. 스와이프 되는 상대 블럭 위치를 구한다. (using SwipeDir Extension Method)
            int nSwipeRow = nRow;
            int nSwipeCol = nCol;
            nSwipeRow += swipeDir.GetTargetRow(); //Right : + 1, LEFT : -1
            nSwipeCol += swipeDir.GetTargetCol(); //UP : +1, DOWN : -1

            Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, $"Invalid Swipe : {nSwipeRow}, {nSwipeCol}");
            Debug.Assert(nSwipeRow >= 0 && nSwipeRow < MaxRow && nSwipeCol >= 0 && nSwipeCol < MaxCol, $"Swipe 타겟 블럭 인덱스 오류 = ({nSwipeRow},{nSwipeCol})");

            //2. 스와이프가 가능한 블럭인지 체크한다ㅣ (인덱스 Validation은 호출 전에 검증됨)
            if(_board.IsSwipeable(nSwipeRow, nSwipeCol))
            {
                //2.1 스와이프 대상 블럭(소스, 타겟)과 각 블럭의 이동전 위치를 저장한다. 
                Block targetBlock = Blocks[nSwipeRow, nSwipeCol];
                Block baseBlock = Blocks[nRow, nCol];
                Debug.Assert(baseBlock != null && targetBlock != null);

                Vector3 basePos = baseBlock.BlockObj.transform.position;
                Vector3 targetPos = targetBlock.BlockObj.transform.position;

                //2.2 스와이프 액션을 실행한다. 
                if(targetBlock.IsSwipeable(baseBlock))
                {
                    //2.2.1 상대방의 블럭 위치로 이동하는 애니메이션을 수행한다. 
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    //2.2.2 Board에 저장된 블럭의 위치를 교환한다
                    Blocks[nRow, nCol] = targetBlock;
                    Blocks[nSwipeRow, nSwipeCol] = baseBlock;

                    actionResult.value = true;
                }
            }

            yield break;
        }

        public bool IsValideSwipe(int nRow, int nCol, Swipe swipeDir)
        {
            switch (swipeDir)
            {
                case Swipe.DOWN : return nRow > 0;
                case Swipe.UP : return nRow < MaxRow - 1;
                case Swipe.LEFT : return nCol > 0;
                case Swipe.RIGHT : return nCol < MaxCol -1;
                default : return false;
            }
        }
        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            yield return _board.Evaluate(matchResult);
        }
        //디버그용 : 각 객체 생성및 생성 데이터 확인 
        public void PrintAll()
        {
            System.Text.StringBuilder strCells = new System.Text.StringBuilder();
            System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

            for(int nRow = MaxRow -1; nRow >= 0; nRow--)
            {
                for(int nCol = 0; nCol < MaxCol; nCol++)
                {
                    strBlocks.Append($"{Blocks[nRow,nCol].Type }, ");
                    strCells.Append($"{Cells[nRow,nCol].Type }, ");
                }
                strBlocks.Append("\n");
                strCells.Append("\n");
            }

            Debug.Log(strBlocks.ToString());
            Debug.Log(strCells.ToString());
        }
    }
}
