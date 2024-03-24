using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dafhne.Quest;
using Dafhne.Stage;
using Dafhne.Util;
using UnityEngine;


namespace Dafhne.Board
{    
    // Board는 Cell 과 Block으로 구성
    // Cell은 움직이지 않는 요소(ex:JellyType, Empty)
    // Block은 실제로 움직이거나 제거되는 구성요소
    public class Board
    {
        //보드의 정보 정의
        private int _maxCol; //행 수
        private int _maxRow; //열 수

        public int MaxCol { get { return _maxCol; } }
        public int MaxRow { get { return _maxRow; } }

        //cell
        Cell[,] _cells;
        public Cell[,] Cells { get { return _cells; } }

        //block
        Block[,] _blocks;
        public Block[,] Blocks{ get { return _blocks; } }
        
        Transform _rootObj;
        GameObject _cellPrefab;
        GameObject _blockPrefab;
        StageBuilder _stageBuilder;

        BoardEnumerator _enumerator;

        public Board(int maxCol, int maxRow)
        {
            _maxCol = maxCol;
            _maxRow = maxRow;

            //create cell
            _cells = new Cell[maxCol, maxRow];

            //creak block
            _blocks = new Block[maxCol, maxRow];

            _enumerator = new BoardEnumerator(this);
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform rootObj, StageBuilder stageBuilder)
        {
            //1. 스테이지 구성에 필요한 Cell, Block, rootObj(Board) 정보를 저장한다. 
            _cellPrefab = cellPrefab;
            _blockPrefab = blockPrefab;
            _rootObj = rootObj;
            _stageBuilder = stageBuilder;

            //2. 3매치된 블럭이 없도록 섞는다.
            BoardShuffler shuffler = new BoardShuffler(this, true);
            shuffler.Shuffle();

            //2. Cell, Block Prefab 을 이용해서 Board 에 Cell/Block GameObject를 추가한다. 
            float initX = CalcInitX(0.5f);
            float initY = CalcInitY(0.5f);
            for(int nRow =0; nRow < _maxRow; nRow++)
            {
                for(int nCol = 0; nCol < _maxCol; nCol++)
                {
                    Cell cell = _cells[nRow, nCol]?.InstantiateCellObj(cellPrefab, rootObj);
                    cell?.Move(initX + nCol, initY + nRow);

                    Block block = _blocks[nRow, nCol]?.InstantiateBlockObj(blockPrefab, rootObj);
                    block?.Move(initX + nCol, initY + nRow);
                    
                }
            }


        }

        public float CalcInitX(float offset = 0)
        {
            return -_maxCol / 2.0f + offset;
        }
        public float CalcInitY(float offset = 0)
        {
            return -_maxRow / 2.0f + offset;
        }

        public bool CanShuffle(int nRow, int nCol, bool isLoading)
        {
            if(_cells[nRow, nCol].Type.IsBlockMoveableType())
                return false;
            
            return true;
        }

        public void ChangeBlock(Block block, BlockElement notAllowedElement)
        {
            BlockElement generateElement;

            while(true)
            {
                generateElement = (BlockElement) UnityEngine.Random.Range(0,6);

                if (notAllowedElement == generateElement)
                    continue;

                break;
            }

            block.BlockElement = generateElement;
        }

        public bool IsSwipeable(int nRow, int nCol)
        {
            return _cells[nRow, nCol].Type.IsBlockMoveableType();
        }

        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            //1. 모든 블럭의 매칭 정보(개수, 상태, 내구도 등)를 계산한후, 3매치 블럭이 있으면 true리턴
            bool isMatchedBlockFound = UpdateAllBlocksMatchedStatus();

            //2. 3매칭 블럭이 없는 경우 
            if(isMatchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }
            
            //3. 3매칭 블럭이 있는 경우 
            //3.1 첫번째 Phase
            //매치된 블럭에 지정된 액션을 수행한다. 
            //ex)가로줄의 블럭 전체가 클리어 되는 블럭인 경우에 처리 등등 

            for(int nRow = 0; nRow < _maxRow; nRow++)
            {
                for(int nCol = 0; nCol < _maxCol; nCol++)
                {
                    Block block = _blocks[nRow, nCol];

                    block?.DoEvaluation(_enumerator, nRow, nCol);
                }
            }

            //3.2. 두번째 Phase
            //첫번째 Phase에서 반영된 블럭의 상태값에 따라서 블럭의 최종상태를 반영한다. 
            List<Block> clearBlocks = new List<Block>();

            for(int nRow = 0; nRow < _maxCol; nRow++)
            {
                for(int nCol = 0; nCol < _maxCol; nCol++)
                {
                    Block block = _blocks[nRow, nCol];

                    if(block != null)
                    {
                        if(block._status == BlockStatus.CLEAR)
                        {
                            //보드에서 블록제거
                            clearBlocks.Add(block);
                            _blocks[nRow, nCol] = null;
                        }
                    }
                }
            }

            //3.3 매칭된 블럭을 제거한다. 
            clearBlocks.ForEach((block) => block.Destroy());

            //3.4 3매칭 블럭이 있는경우 true 설정 
            matchResult.value = true;

            yield break;
        }

        public bool UpdateAllBlocksMatchedStatus()
        {
            //for GC
            List<Block> matchedBlockList = new List<Block>(); 
            int nCount = 0;
            for(int nRow = 0; nRow < MaxRow; nRow++)
            {
                for(int nCol = 0; nCol < MaxCol; nCol++)
                {
                    //개별 블록 매칭정보 계산
                    if(EvalBlocksIfMatched(nRow, nCol, matchedBlockList))
                    {
                        nCount++;
                    }
                }
            }
            return nCount > 0;
        }

        public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
        {
            bool isFound = false;

            Block baseBlock = _blocks[nRow, nCol];
            if(baseBlock == null)
            {
                return false;
            }

            if(baseBlock._matchType != Dafhne.Quest.MatchType.NONE || !baseBlock.IsValidate() || _cells[nRow, nCol].IsObstracle())
            {
                return false;
            }

            //검사하는 자신을 매칭 리스트에 우선 보관한다. 
            matchedBlockList.Add(baseBlock);

            //1. 가로 블럭 검색
            Block block;
            //1.1오른쪽 방향 
            for(int i = nCol  +1; i< MaxCol; i++)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Add(block);
            }
            //1.2왼쪽방향
            for(int i = nCol  -1; i >= 0; i--)
            {
                block = _blocks[nRow, i];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Insert(0, block);
            }
            //1.3 매치된 상태인지 판단한다. 
            //기준 블럭(baseBlock)을 제외하고 좌우에 2개이상이면 기준 블럭 포함해서 3개이상 매치되는 경우로 판단할 수 있다.
            if(matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                isFound = true;
            }

            matchedBlockList.Clear();

            //2.세로 블럭 검색
            matchedBlockList.Add(baseBlock);
            //2.1 위쪽 검사
            for(int i = nRow + 1; i < MaxRow; i++)
            {
                block = _blocks[i, nCol];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Add(baseBlock);
            }
            //2.2 아래 검사
            for(int i = nRow - 1; i >= 0; i--)
            {
                block = _blocks[i, nCol];
                if(!block.IsSafeEqual(baseBlock))
                {
                    break;
                }

                matchedBlockList.Insert(0,baseBlock);
            }

            //2.3 매치된 상태인지 판단한다.
            //기준 블럭(baseBlock)을 제외하고 상하에 2개이상이면 기준블럭 포함해서 3개이상 매치되는 경우로 판단할 수 있다
            if(matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                isFound = true;
            }

            //계산을 위해 리스트에 저장한 블럭 제거 
            matchedBlockList.Clear();

            return isFound;
        }

        void SetBlockStatusMatched(List<Block> blockList, bool isHorizon)
        {
            int nMatchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((MatchType)nMatchCount));
        }

        public IEnumerator ArrangeBlocksAfterClean(List<KeyValuePair<int, int>> unfilledBlocks, List<Block> movingBlocks)
        {
            SortedList<int, int> emptyBlocks = new SortedList<int, int>();
            List<KeyValuePair<int, int>> emptyRemainBlocks = new List<KeyValuePair<int, int>>();

            for(int nCol = 0; nCol < _maxCol; nCol++)
            {
                emptyBlocks.Clear();

                //1. 같은 열(col) 에 빈 블럭을 수집한다. 
                //현재 col의 다른 row 의 비어있는 블럭 인덱스를 수집한다. sortedList이므로 첫번째 노드가 가장 아래쪽 블럭 위치다.
                for(int nRow = 0; nRow < _maxRow; nRow++)
                {
                    if(CanBlockBeAllocatable(nRow, nCol))
                    {
                        emptyBlocks.Add(nRow,nRow);
                    }
                }

                //아래쪽에 비어있는 블럭이 없는 경우
                if(emptyBlocks.Count == 0)
                {
                    continue;
                }

                //2. 이동이 가능한 블럭을 비어있는 하단 위치로 이동한다. 

                //2.1 가장 아래쪽부터 비어있는 블럭을 처리한다. 
                KeyValuePair<int, int> first = emptyBlocks.First();

                //2.2 비어있는 블럭 위쪽 방향으로 이동 가능한 블럭을 탐색하면서 빈 블럭을 채워나간다. 
                for(int nRow = first.Value + 1; nRow < _maxRow; nRow++)
                {
                    Block block = _blocks[nRow, nCol];

                    //2.2.1 이동 가능한 아이템이 아닌 경우 Passs
                    if(block == null || _cells[nRow, nCol].Type == CellType.EMPTY) //ToDO EMPTY를 직접 체크하지 않고 이러한 부류를 함수로 체크
                    {
                        continue;
                    }

                    //2.2.3 이동이 필요한 블럭 발견
                    block.dropDistance = new Vector2(0, nRow - first.Value); // GameObject 에니메이션 이동
                    movingBlocks.Add(block);

                    //2.2.4 빈공간으로 이동
                    Debug.Assert(_cells[first.Value, nCol].IsObstracle() == false, $"{_cells[first.Value, nCol]}");
                    _blocks[first.Value, nCol] = block; // 이동될 위치로 Board에서 저장된 위치 이동

                    //2.2.5 다른 곳으로 이동했으므로 현재 위치는 비워둔다. 
                    _blocks[nRow, nCol] = null;

                    //2.2.6 비어있는 블럭 리스트에서 사용된 첫번째 노드(first)를 삭제한다. 
                    emptyBlocks.RemoveAt(0);

                    //2.2.7 현재 위치의 블럭이 다른 위치로 이동했으므로 현재 위치가 비어있게 된다. 
                    //그러므로 비어있는 블럭을 보관하는 emptyBlocks에 추가한다. 
                    emptyBlocks.Add(nRow, nRow);

                    //2.2.8 다음(Next) 비어있는 블럭을 처리하도록 기준을 변경한다. 
                    first = emptyBlocks.First();
                    nRow = first.Value; //Note : 빈곳 바로 위부터 처리하도록 위치 조정, for 문에서 nRow++ 하기때문에 +1을 하지 않는다.
                }
            }

            yield return null;

            //드롭으로 채워지지 않는 블럭이 잏는경우 (왼쪽 아래 순으로 들어가있음)
            if(emptyRemainBlocks.Count > 0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }

            yield break;
        }

        bool CanBlockBeAllocatable(int nRow, int nCol)
        {
            if(!_cells[nRow, nCol].Type.IsBlockAllocatableType())
            {
                return false;
            }
            return _blocks[nRow, nCol] == null;
        }

        public IEnumerator SpwanBlocksAfterClean(List<Block> movingBlocks)
        {
            for(int nCol = 0; nCol < MaxCol; nCol++)
            {
                for(int nRow = 0; nRow < MaxRow; nRow++)
                {
                    //비어있는 블럭이 있는경우, 상위 열은 모두 비어있거나, 장애물로 인해서 남아있음.
                    if(_blocks[nRow, nCol] == null)
                    {
                        int nTopRow = nRow;

                        int nSpwanBaseY = 0;
                        for(int y = nTopRow; y< MaxRow; y++)
                        {
                            if(_blocks[y, nCol] != null || !CanBlockBeAllocatable(y,nCol))
                            {
                                continue;
                            }

                            Block block = SpwanBlockWithDrop(y, nCol, nSpwanBaseY, nCol);
                            if(block != null)
                            {
                                movingBlocks.Add(block);
                            }

                            nSpwanBaseY++;
                        }

                        break;
                    }       
                }    
            }

            yield return null;
        }

         //블럭을 생성하고 목적지(nRow, nCol) 까지 드롭한다.
        //@parm nRow, nCol : 생성후 보드에 저장되는 위치
        //@parm nSpawnedRow, nSpawnedCol : 화면에 생성되는 위치, nRow, nCol 위치까지 드롭 액션이 연출된다.
        Block SpwanBlockWithDrop(int nRow, int nCol, int nSpawnedRow, int nSpawnedCol)
        {
            float fInitX = CalcInitX(Core.Constants.BLOCK_ORG);
            float fInitY = CalcInitY(Core.Constants.BLOCK_ORG) + MaxRow;

            Block block = _stageBuilder.SpawnBlock().InstantiateBlockObj(_blockPrefab, _rootObj);
            if(block != null)
            {
                _blocks[nRow, nCol] = block;
                block.Move(fInitX + (float)nSpawnedCol, fInitY + (float)nSpawnedRow);
                block.dropDistance = new Vector2(nSpawnedCol - nCol, MaxRow + (nSpawnedRow - nRow));
            }
            return block;
        }
    }
}

