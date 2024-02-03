using UnityEngine;

namespace Dafhne.Board
{
    public class Cell
    {
        protected CellType _cellType;
        public CellType Type
        {
            get { return _cellType; }
            set { _cellType = value; }
        }

        protected CellBehavior _cellBehavior;
        public CellBehavior CellBehavior
        {
            get{ return _cellBehavior; }
            set
            {
                _cellBehavior = value;
                _cellBehavior.SetCell(this);
            }
        }

        public Cell(CellType cellType)
        {
            _cellType = cellType;
        }

        public Cell InstantiateCellObj(GameObject cellPrefab, Transform rootObj)
        {
            //1. Cell 오브젝트 생성
            GameObject newObj = Object.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);

            //2.컨테이너(Board)의 차일드로 Cell을 포함시킨다. 
            newObj.transform.parent = rootObj;

            //3. cell 오브젝트에 적용되어있는 CellBehavior 컴토넌트를 보관한다. 
            this.CellBehavior = newObj.transform.GetComponent<CellBehavior>();

            return this;
        }

        public void Move(float x, float y)
        {
            CellBehavior.transform.position = new Vector3(x, y);
        }
    }
}
