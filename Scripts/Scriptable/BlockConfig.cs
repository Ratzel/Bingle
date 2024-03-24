
using UnityEngine;

namespace Dafhne.Scriptable
{
    [CreateAssetMenu(menuName ="Bingle/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public float[] dropSpeed;
        public Sprite[] basicBlockSprites;
    }
}

