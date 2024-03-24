
using Dafhne.Board;
using UnityEngine;

namespace Dafhne.Scriptable
{
    [CreateAssetMenu(menuName ="Bingle/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public float[] dropSpeed;
        public Sprite[] basicBlockSprites;
        public GameObject explosion;
        public Color[] blockColors;

        public GameObject GetExplosionObject(BlockQuestType questType)
        {
            switch(questType)
            {
                case BlockQuestType.CLEAR_SIMPLE:
                    return Instantiate(explosion);
                
                default:
                    return Instantiate(explosion);
            }
        }

        public Color GetBlockColor(BlockElement blockElement)
        {
            return blockColors[(int)blockElement];
        }
    }
}

