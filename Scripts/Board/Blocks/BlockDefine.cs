namespace Dafhne.Board
{
    public enum BlockType
    {
        EMPTY = 0,
        BASIC = 1
    }
    public enum BlockElement
    {
        NONE = -1,
        ELEMENT_DEEPWATER = 0,
        ELEMENT_CLOVER,
        ELEMENT_HEART,
        ELEMENT_MOON,
        ELEMENT_STAR,
        ELEMENT_WATER,
    }

    public static class BlockMethod
    {
        public static bool IsSafeEqual(this Block block, Block targetBlock)
        {
            if(block == null)
            {
                return false;
            }

            return block.IsEqual(targetBlock);
        }
    }
}

