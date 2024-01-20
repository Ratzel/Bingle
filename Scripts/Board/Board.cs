using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Board
{
    public class Board
    {
        private int _numberCol;
        private int _numberRow;

        public int MaxCol { get { return _numberCol; } }
        public int MaxRow { get { return _numberRow; } }

        //cell

        //block

        public Board(int numberCol, int numberRow)
        {
            _numberCol = numberCol;
            _numberRow = numberRow;

            //create cell


            //creak block
        }
    }
}

