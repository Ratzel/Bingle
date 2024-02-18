using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Dafhne.Core;

namespace Dafhne.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;

    public class BoardShuffler 
    {
        Board _board;
        bool _isLoadingMode;

        SortedList<int, BlockVectorKV> _orgBlocks = new SortedList<int, BlockVectorKV>();
        IEnumerator<KeyValuePair<int, BlockVectorKV>> _it;
        Queue<BlockVectorKV> _unusedBlocks = new Queue<BlockVectorKV>();
        bool _isListComplete;

        public BoardShuffler(Board board, bool isLoadingMode)
        {
            _board = board;
            _isLoadingMode = isLoadingMode;
        }

        public void Shuffle(bool doAnimation = false)
        {
            //1. 셔플 시작전에 각 블럭의 매칭 정보를 업데이트한다. 
            PreparedDuplicationDatas();

            //2. 셔플 대상 블럭을 별도 리스트에 보관한다. 
            PrepareShuffleBlocks();

            //3. 1,2 에서 준비한 데이터를 이용하여 셔플을 수행한다. 
            RunShuffle(doAnimation);
        }

        void PreparedDuplicationDatas()
        {
            for(int nRow = 0; nRow < _board.MaxRow; nRow++)
            {
                for(int nCol = 0; nCol < _board.MaxCol; nCol++)
                {
                    Block block = _board.Blocks[nRow, nCol];

                    if(block == null)
                    {
                        continue;
                    }

                    if(_board.CanShuffle(nRow, nCol, _isLoadingMode))
                    {
                        block.ResetDuplicationInfo();
                    }
                    else //움직이지 못하는 블럭(밧줄에 묶인 경우 등 현재상태에서 이동불가한 블럭)의 매칭 정보를 계산한다. 
                    {
                        block.HorizonDuplicate = 1;
                        block.VerticalDuplicate = 1;

                        //좌하 위치에 셔플 미대상(즉, 움직이지 못하는 블럭) 인 블럭의 매치 상태를 반영한다. 
                        //(3개이상 매치되는 경우는 발생하지 않기 때문에 인접한 블럭만 검사하면 된다)
                        //Note: 좌하만 계산해도 전체 블럭을 모두 검사할 수 있다. 
                        if(nCol > 0 && !_board.CanShuffle(nRow, nCol - 1, _isLoadingMode) && _board.Blocks[nRow, nCol - 1].IsSafeEqual(block))
                        {
                            block.HorizonDuplicate = 2;
                            _board.Blocks[nRow, nCol - 1].HorizonDuplicate = 2;
                        }

                        if(nRow > 0 && !_board.CanShuffle(nRow - 1, nCol, _isLoadingMode) && _board.Blocks[nRow - 1, nCol].IsSafeEqual(block))
                        {
                            block.VerticalDuplicate = 2;
                            _board.Blocks[nRow -1, nCol].VerticalDuplicate = 2;
                        }
                    }
                }
            }
        }

        void PrepareShuffleBlocks()
        {
            for(int nRow = 0; nRow < _board.MaxRow; nRow++)
            {
                for(int nCol = 0; nCol < _board.MaxCol; nCol++)
                {
                    if(_board.CanShuffle(nRow, nCol, _isLoadingMode))
                    {
                        continue;
                    }

                    //Sorted List에 순서를 정하기 위해서 중복값이 없도록 랜덤 값을 생성한후 키값으로 저장한다.
                    while(true)
                    {
                        int nRandom = UnityEngine.Random.Range(0, 10000);
                        //detect key duplication
                        if(_orgBlocks.ContainsKey(nRandom))
                        {
                            continue;
                        }

                        _orgBlocks.Add(nRandom, new BlockVectorKV(_board.Blocks[nRow, nCol], new Vector2Int(nCol,nRow)));
                        break;
                    }
                }
            }

            _it = _orgBlocks.GetEnumerator();
        }

        void RunShuffle(bool doAnimation)
        {
            for(int nRow = 0; nRow < _board.MaxRow; nRow++)
            {
                for(int nCol = 0; nCol < _board.MaxCol; nCol++)
                {
                    //1. 셔플 미대상 블럭은 PASS
                    if(_board.CanShuffle(nRow, nCol, _isLoadingMode))
                    {
                        continue;
                    }

                    //2. 셔플 대상 블럭은 새로 배치할 블럭을 리턴받아서 저장한다. 
                    _board.Blocks[nRow, nCol] = GetShuffledBlock(nRow, nCol);
                }

                
            }
        }

        Block GetShuffledBlock(int nRow, int nCol)
        {
            BlockElement prevElement = BlockElement.NONE; //처음 비교시에 종류를 저장
            Block firstBlock = null; //리스트를 전부 처리하고 큐만 남은 경우에 중복 체크를 위해 사용( 큐에서 꺼낸 첫번째 블럭)

            bool isUsedQueue = true; //ture : 큐에서 꺼냄 false : 리스트에서 꺼냄
            while(true)
            {
                //1. Queue에서 블럭을 하나 꺼낸다. 첫번째 후보다.
                BlockVectorKV blockInfo = NextBlock(isUsedQueue);
                Block block = blockInfo.Key;

                //2.리스트에서 블럭을 전부 처리한 경우 : 전체루프(for 문 포함 )에서 1회만 발생 
                if(block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }  

                Debug.Assert(block != null, $"block can't be null : queue count -> {_unusedBlocks.Count}");

                if(prevElement == BlockElement.NONE)
                {
                    prevElement = block.BlockElement;
                }

                //3. 리스트를 모두 처리한 경우
                if(_isListComplete)
                {
                    if(firstBlock == null)
                    {
                        //3.1 전체 리스트를 처리하고, 처ㅡㅁ으로 큐에서 꺼낸경우
                        firstBlock = block; //큐에서 꺼낸 첫번째 블럭 
                    }
                    else if(System.Object.ReferenceEquals(firstBlock, block))
                    {
                        //3.2 처음 보았던 블럭을 다시 처리하는 경우, 
                        //즉, 큐에 들어있는 모든 블럭이 조건에 맞지 않는 경우 (남은 블럭중에 조건에 맞는게 없는경우)
                        _board.ChangeBlock(block,prevElement);
                    }
                }

                //4. 상하좌우 인접 블럭과 겹치는 개수를 개산한다. 
                Vector2Int vecDup = CalcDuplications(nRow, nCol, block);

                //5. 2개 이상 매치되는 경우, 현재 위치에 해당 블럭이 올수 없으므로 큐에 보관하고 다음 블럭 처리하도록 continue한다. 
                if(vecDup.x > 2 || vecDup.y >2)
                {
                    _unusedBlocks.Enqueue(blockInfo);
                    isUsedQueue = _isListComplete || !isUsedQueue;

                    continue;
                }

                //6. 블럭이 위치할 수 있는 경우ㅡ 찾은 위치로 Block GameObject를 이동시킨다.
                block.VerticalDuplicate = vecDup.y;
                block.HorizonDuplicate = vecDup.x;
                if(block.BlockObj != null)
                {
                    float initX = _board.CalcInitX(Constants.BLOCK_ORG);
                    float initY = _board.CalcInitY(Constants.BLOCK_ORG);

                    block.Move(initX + nCol, initY + nRow);
                }

                return block;
            }
        }

        BlockVectorKV NextBlock(bool isUsedQueue)
        {
            if(isUsedQueue && _unusedBlocks.Count > 0)
            {
                return _unusedBlocks.Dequeue();
            }

            if(!_isListComplete && _it.MoveNext())
            {
                return _it.Current.Value;
            }

            _isListComplete = true;

            return new BlockVectorKV(null, Vector2Int.zero);
        }

        Vector2Int CalcDuplications(int nRow, int nCol, Block block)
        {
            int colDup = 1;
            int rowDup = 2;

            if(nCol > 0 && _board.Blocks[nRow, nCol - 1].IsSafeEqual(block))
            {
                colDup += _board.Blocks[nRow, nCol - 1].HorizonDuplicate;
            }
            if(nRow > 0 && _board.Blocks[nRow - 1, nCol].IsSafeEqual(block))
            {
                rowDup += _board.Blocks[nRow - 1, nCol].VerticalDuplicate;
            }

            if(nCol < _board.MaxCol -1 && _board.Blocks[nRow, nCol + 1].IsSafeEqual(block))
            {
                Block rightBlock = _board.Blocks[nRow, nCol + 1];
                colDup += rightBlock.HorizonDuplicate;

                //셔플 미대상블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 엄데이트 한다. 
                if(rightBlock.HorizonDuplicate == 1)
                {
                    rightBlock.HorizonDuplicate = 2;
                }
            }

            if(nRow < _board.MaxRow - 1 && _board.Blocks[nRow + 1, nCol].IsSafeEqual(block))
            {
                Block upperBlock = _board.Blocks[nRow + 1, nCol];
                rowDup += upperBlock.VerticalDuplicate;

                //셔플 미대상블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 업데이트한다.
                if(upperBlock.VerticalDuplicate == 1)
                {
                    upperBlock.VerticalDuplicate = 2;
                }
            }

            return new Vector2Int(colDup, rowDup);
        }
    }
}
