using UnityEngine;
using Dafhne.Board;

namespace Dafhne.Board
{

    public class BoardEnumerator : MonoBehaviour
    {
        Board _board;
        public BoardEnumerator(Board board)
        {
            this._board = board;
        }

        public bool isCageTypeCell(int nRow, int nCol)
        {
            return false;
        }
    }

}
