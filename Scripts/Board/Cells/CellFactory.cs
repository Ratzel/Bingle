using System.Diagnostics;

namespace Dafhne.Board
{
    public static class CellFactory
    {
        public static Cell SpawnCell(Stage.StageInfo stageInfo, int nRow, int nCol)
        {
            Debug.Assert(stageInfo != null);
            Debug.Assert(nRow < stageInfo.MaxRow && nCol < stageInfo.MaxCol);
            return SpawnCell(stageInfo.GetCellType(nRow, nCol));
        }

        public static Cell SpawnCell(CellType cellType)
        {
            return new Cell(cellType);
        }
    }
}
