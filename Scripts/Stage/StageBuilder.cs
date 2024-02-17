using System.Diagnostics;
using Dafhne.Board;

namespace Dafhne.Stage
{
    public class StageBuilder
    {
        StageInfo _stageInfo;
        int _maxStage;

        public StageBuilder(int maxStage)
        {
            _maxStage = maxStage;
        }

        public Stage ComposeStage()
        {
            Debug.Assert(_maxStage > 0, $"invalidate stage {_maxStage}");
            //0. 스테이정보를 로드한다. 
            _stageInfo = StageReader.LoadStage(_maxStage);

            //1. Stage객체를 생성한다. 
            Stage stage = new Stage(this, _stageInfo.MaxRow, _stageInfo.MaxCol);

            //2. Cell, Block값을 초기화 한다. 
            for(int nRow = 0; nRow < _stageInfo.MaxRow; nRow++)
            {
                for(int nCol = 0; nCol < _stageInfo.MaxCol; nCol++)
                {
                    stage.Blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.Cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }
            return stage;
        }

        Block SpawnBlockForStage(int nRow, int nCol)
        {
            if(_stageInfo.GetCellType(nRow,nCol) == CellType.EMPTY)
            {
                return SpawnEmptyBlock();
            }
            return SpawnBlock();
        }

        Cell SpawnCellForStage(int nRow, int nCol)
        {
            Debug.Assert( _stageInfo != null);
            Debug.Assert(_stageInfo.MaxRow > nRow && _stageInfo.MaxCol > nCol);

            return CellFactory.SpawnCell(_stageInfo, nRow, nCol);
        }
        
        public Block SpawnBlock()
        {
            return BlockFactory.SpwanBlock(BlockType.BASIC);
        }

        public Block SpawnEmptyBlock()
        {
            return BlockFactory.SpwanBlock(BlockType.EMPTY);
        }
        public static Stage BuildStage(int nStage)
        {
            StageBuilder stageBuilder = new StageBuilder(nStage);
            Stage stage = stageBuilder.ComposeStage();

            return stage;
        }
    }
}

