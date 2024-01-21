using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Board
{
    // Board는 Cell 과 Block으로 구성
    // Cell은 움직이지 않는 요소(ex:JellyType, Empty)
    // Block은 실제로 움직이거나 제거되는 구성요소
    public class Board
    {
        //보드의 정보 정의
        private int _nCol; //행 수
        private int _nRow; //열 수

        public int MaxCol { get { return _nCol; } }
        public int MaxRow { get { return _nRow; } }

        //cell
        Cell[,] _cells;
        public Cell[,] Cells { get { return _cells; } }

        //block
        Block[,] _blocks;
        public Block[,] Blocks{ get { return _blocks; } }
        

        public Board(int nCol, int nRow)
        {
            _nCol = nCol;
            _nRow = nRow;

            //create cell
            _cells = new Cell[nCol, nRow];

            //creak block
            _blocks = new Block[nCol, nRow];
        }
    }
}

