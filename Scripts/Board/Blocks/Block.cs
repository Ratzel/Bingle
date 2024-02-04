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
    }
}

