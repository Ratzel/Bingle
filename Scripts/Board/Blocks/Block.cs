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

        public Block(BlockType blockType)
        {
            _blockType = blockType;
        }
    }
}

