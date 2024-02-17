using Dafhne.Board;
using UnityEngine;

namespace Dafhne.Stage
{
    [System.Serializable]
    public class StageInfo
    {
        public int MaxRow;
        public int MaxCol;
        public int[] Cells;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        public CellType GetCellType(int nRow, int nCol)
        {
            Debug.Assert(Cells != null && Cells.Length > (nRow * MaxCol) + nCol);

            //읽어드린 json의 뷰잉과 출력의 뷰잉이 뒤집혀서 보여지는 이슈가 있음
            //스테이지 편집툴없이 직접 json을 수정해야하여 편의상 아래에 변환 로직을 추가
            // if(Cells.Length > (nRow * MaxCol) + nCol)
            // {
            //     return (CellType) Cells[(nRow * MaxCol) + nCol];
            // }

            int revicedRow = (MaxRow - 1) - nRow;
            if(Cells.Length > (revicedRow * MaxCol) + nCol)
            {
                return (CellType) Cells[(revicedRow * MaxCol) + nCol];
            }

            Debug.Assert(false);

            return CellType.EMPTY;
        }
        
        public bool isValidated()
        {
            Debug.Assert(Cells.Length == MaxRow * MaxCol);
            Debug.Log($"cellLenth {Cells.Length}, row, col =({MaxRow}, {MaxCol}) ");

            if(Cells.Length != MaxRow * MaxCol)
            {
                return false;
            }

            return true;
        }
    }
}
