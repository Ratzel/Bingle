namespace Dafhne.Board
{
    public static class BlockFactory
    {
        public static Block SpwanBlock(BlockType blockType)
        {
            Block block = new Block(blockType);

            //Set Element
            if(blockType == BlockType.BASIC)
            {
                block.BlockElement = (BlockElement) UnityEngine.Random.Range(0,6);
            }
            else if(blockType == BlockType.EMPTY)
            {
                block.BlockElement = BlockElement.NONE;
            }

            return block;
        }
    }
}
