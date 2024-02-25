using UnityEngine;

namespace Dafhne.Board
{
    public class Block
    {
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
        public Block(BlockType blockType)
        {
            _blockType = blockType;
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
    }
}

