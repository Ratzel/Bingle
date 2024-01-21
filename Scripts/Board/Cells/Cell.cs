
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

        public Cell(CellType cellType)
        {
            _cellType = cellType;
        }
    }
}

