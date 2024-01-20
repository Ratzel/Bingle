using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Stage
{
    public class Stage
    {
        //게임판 크기 저장용 설정
        private int _numberCol; //행
        private int _numberRow; //열

        public int MaxCol { get { return _numberCol; } }
        public int MaxRow { get { return _numberRow; } }

        //게임판 저장용
        Board.Board _board;
        public Board.Board Board { get { return _board; } }
    }
}
