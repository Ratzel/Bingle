using UnityEngine;
using Dafhne.Quest;

namespace Dafhne.Board
{
    public class Block
    {
        public BlockStatus _status;
        public BlockQuestType _questType;
        public MatchType _matchType = MatchType.NONE;
        public short _matchCount;

        BlockActionBehavior _blockActionBehavior;

        int _nDurability; //내구도, 0이되면 제거 
        public virtual int Durability
        {
            get { return _nDurability; }
            set { _nDurability = value; }
        }

        private BlockType _blockType;
        public BlockType Type
        {
            get{ return _blockType; }
            set{ _blockType = value; }
        }
        protected BlockBehavior _blockBehavior;
        public BlockBehavior BlockBehavior
        {
            get { return _blockBehavior; }
            set
            {
                _blockBehavior = value;
                _blockBehavior.SetBlock(this);
            }
        }
        protected BlockElement _blockElement;
        public BlockElement BlockElement
        {
            get{ return _blockElement; }
            set 
            {
                _blockElement = value;
                _blockBehavior?.UpdateView(true);
            }
        }

        public Transform BlockObj
        {
            get { return _blockBehavior?.transform; }
        }

        Vector2Int _vecDuplicate; //블럭 중복 개수, Shuffle시에 중복 검사에 사용 

        public int HorizonDuplicate //가로방향 중복 검사시 사용
        {
            get { return _vecDuplicate.x; }
            set { _vecDuplicate.x = value; }
        }

        public int VerticalDuplicate
        {
            get { return _vecDuplicate.y; }
            set { _vecDuplicate.y = value; }
        }
        
        public void ResetDuplicationInfo()
        {
            _vecDuplicate.x = 0;
            _vecDuplicate.y = 0;
        }

        public Block(BlockType blockType)
        {
            _blockType = blockType;

            _status = BlockStatus.NORMAL;
            _questType = BlockQuestType.CLEAR_SIMPLE;
            _matchType = MatchType.NONE;
            _blockElement = BlockElement.NONE;

            _nDurability = 1;
        }

        public bool IsEqual(Block target)
        {
            if (IsMatchableBlock() && this.BlockElement == target.BlockElement)
            {
                return true;
            }
            return false;
        }
        public bool IsMatchableBlock()
        {
            return !(Type == BlockType.EMPTY);
        }
       

        internal Block InstantiateBlockObj(GameObject blockPrefab, Transform rootObj)
        {
            //유효하지 않은 블럭인 경우 , Block GameObject를 생성하지 않는다. 
            if(IsValidate() == false)
            {
                return null;
            }

            //1. 블록 오브젝트를 생성한다. 
            GameObject newObject = Object.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity);

            //2. 컨테이너(Board)의 차일드로 Block을 포함시킨다.
            newObject.transform.parent = rootObj;

            //3. Block 오브젝트에 적용된 BlockBehavior 컴포넌트를 보관한다. 
            this.BlockBehavior = newObject.GetComponent<BlockBehavior>();
            _blockActionBehavior = newObject.transform.GetComponent<BlockActionBehavior>();
            
            return this;
        }

        internal void Move(float x, float y)
        {
            BlockBehavior.transform.position = new Vector3(x,y);
        }

        public bool IsValidate()
        {
            return Type != BlockType.EMPTY;
        }

        public void MoveTo(Vector3 to, float duration)
        {
            _blockBehavior.StartCoroutine(Util.Action2D.MoveTo(BlockObj, to, duration));
        }

        public bool IsSwipeable(Block baseBlock)
        {
            return true;
        }

        public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
        {
            Debug.Assert(boardEnumerator != null, $"({nRow},{nCol})");

            if(!IsEvaluatable())
            {
                return false;
            }

            //1. 매치 상태(클리어 조건 충족)인 경우
            if(_status == BlockStatus.MATCH)
            {
                //TODO : cagetype cell  조건이 필요한가?
                if(_questType == BlockQuestType.CLEAR_SIMPLE || boardEnumerator.isCageTypeCell(nRow,nCol))
                {
                    Debug.Assert(_nDurability > 0, $"durability is zero : {_nDurability}");

                    //보드에 블럭 클리어 이벤트를 전달한다. 
                    //블럭 클리어 후에 보드에 미치는 영향을 반영한다. 
                    // if(boardEnumerator.SendMessageToBoard(BlockStatus.CLEAR, nRow, nCol));
                    Durability--;
                }
                else
                {
                    //특수 블럭인 경우 true 리턴
                    return true;
                }

                if(_nDurability == 0)
                {
                    _status = BlockStatus.CLEAR;
                    return false;
                }
            }
            
            //2. 클리어 조건에 아직 도달하지 않는 경우 NORMAL 상태로 복귀
            _status = BlockStatus.NORMAL;
            _matchType = MatchType.NONE;
            _matchCount = 0;

            return false;
        }

        public void UpdateBlockStatusMatched(MatchType matchType, bool isAccumulate = true)
        {
            this._status = BlockStatus.MATCH;

            if(matchType == MatchType.NONE)
            {
                this._matchType = matchType;
            }
            else
            {
                this._matchType = isAccumulate ? matchType.Add(matchType) : matchType; // 기존에 매치 상태인 경우, 누적(Accumulate)모드이면 Add하고 그렇지 않으면 새로운 매치타입으로 대치한다. 
            }

            _matchCount = (short)matchType;
        }        

        public bool IsEvaluatable()
        {
            //이미 처리 완료(Clear) 되었거나, 현재 처리중인 블럭인 경우
            if(_status == BlockStatus.CLEAR || !IsMatchableBlock())
            {
                return false;
            }

            return true;
        }

        public virtual void Destroy()
        {
            Debug.Assert(BlockObj != null, $"{_matchType}");
            BlockBehavior.DoActionClear();
        }

        public bool isMoving
        {
            get
            {
                return BlockObj != null && _blockActionBehavior.isMoving;
            }
        }

        public Vector2 dropDistance
        {
            set
            {
                _blockActionBehavior?.MoveDrop(value);
            }
        }
        
    }
}

