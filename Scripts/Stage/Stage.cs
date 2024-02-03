using System.Collections;
using System.Collections.Generic;
using Dafhne.Board;
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
