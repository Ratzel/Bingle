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
    public enum BlockStatus
    {
        NORMAL, //기본상태
        MATCH, //매칭 블럭 있는 상태
        CLEAR // 클리어 예정 상태
    }
    public enum BlockQuestType
    {
        NONE = -1, 
        CLEAR_SIMPLE = 0, //단일 블럭 제거
        CLEAR_HORIZONTAL = 1, //세로줄 블럭 제거(내구도 -1) -> 4 match 가로형
        CLEAR_VERTICAL = 2, //가로줄 블럭제거 -> 4 match 세로형
        CLEAR_CIRCLE = 3, //인접한 주변영역 블럭 제거 -> T L 매치 (3x3, 4x3)
        CLEAR_LAZER = 4, //지정된 블럭과 동일한 블럭 전체 제거 -> 5 match
        CLEAR_HORZ_BUFF = 5, //HORZ + CIRCLE 조합
        CLEAR_VERT_BUFF = 6, //VERT + CIRCLE 조합
        CLEAR_CIRCLE_BUFF = 7, //CIRCLE + CIRCLE 조합
        CLEAR_LAZER_BUFF = 8 //LAZER + LAZER 조합
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

