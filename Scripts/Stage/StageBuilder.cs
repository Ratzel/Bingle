using Dafhne.Board;

namespace Dafhne.Stage
{
    public class StageBuilder
    {
        int _nStage;

        public StageBuilder(int nStage)
        {
            _nStage = nStage;
        }

        public Stage ComposeStage(int row, int col)
        {
            //Stage객체를 생성한다. 
            Stage stage = new Stage(this, row, col);
            for(int nRow = 0; nRow < row; nRow++)
            {
                for(int nCol = 0; nCol < col; nCol++)
                {
                    stage.Blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.Cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }
            return stage;
        }

        Block SpawnBlockForStage(int nRow, int nCol)
        {
            return new Block(BlockType.BASIC);
        }

        Cell SpawnCellForStage(int nRow, int nCol)
        {
            return new Cell(CellType.BASIC);
        }
        
        public static Stage BuildStage(int nStage, int nRow, int nCol)
        {
            StageBuilder stageBuilder = new StageBuilder(0);
            Stage stage = stageBuilder.ComposeStage(nRow, nCol);

            return stage;
        }
    }
}

