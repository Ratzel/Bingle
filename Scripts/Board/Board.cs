using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Dafhne.Board
{
    // Board는 Cell 과 Block으로 구성
    // Cell은 움직이지 않는 요소(ex:JellyType, Empty)
    // Block은 실제로 움직이거나 제거되는 구성요소
    public class Board
    {
        //보드의 정보 정의
        private int _maxCol; //행 수
        private int _maxRow; //열 수

        public int MaxCol { get { return _maxCol; } }
        public int MaxRow { get { return _maxRow; } }

        //cell
        Cell[,] _cells;
        public Cell[,] Cells { get { return _cells; } }

        //block
        Block[,] _blocks;
        public Block[,] Blocks{ get { return _blocks; } }
        
        Transform _rootObj;
        GameObject _cellPrefab;
        GameObject _blockPrefab;

        public Board(int maxCol, int maxRow)
        {
            _maxCol = maxCol;
            _maxRow = maxRow;

            //create cell
            _cells = new Cell[maxCol, maxRow];

            //creak block
            _blocks = new Block[maxCol, maxRow];
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform rootObj)
        {
            //1. 스테이지 구성에 필요한 Cell, Block, rootObj(Board) 정보를 저장한다. 
            _cellPrefab = cellPrefab;
            _blockPrefab = blockPrefab;
            _rootObj = rootObj;

            //2. Cell, Block Prefab 을 이용해서 Board 에 Cell/Block GameObject를 추가한다. 
            float initX = CalcInitX(0.5f);
            float initY = CalcInitY(0.5f);
            for(int nRow =0; nRow < _maxRow; nRow++)
            {
                for(int nCol = 0; nCol < _maxCol; nCol++)
                {
                    Cell cell = _cells[nRow, nCol]?.InstantiateCellObj(cellPrefab, rootObj);
                    if(nRow == nCol) // 테스트 코드
                    {
                        cell.Type = CellType.EMPTY;
                    }
                    cell?.Move(initX + nCol, initY + nRow);

                    Block block = _blocks[nRow, nCol]?.InstantiateBlockObj(blockPrefab, rootObj);
                    block?.Move(initX + nCol, initY + nRow);
                    
                }
            }


        }

        public float CalcInitX(float offset = 0)
        {
            return -_maxCol / 2.0f + offset;
        }
        public float CalcInitY(float offset = 0)
        {
            return -_maxRow / 2.0f + offset;
        }
    }
}

