using System.Collections;
using System.Collections.Generic;
using Dafhne.Scriptable;
using UnityEngine;  

namespace Dafhne.Board
{
    public class BlockBehavior : MonoBehaviour
    {
        [SerializeField] BlockConfig _blockConfig;

        Block _block;
        SpriteRenderer _spriteRenderer;

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateView(false);
        }

        internal void SetBlock(Block block)
        {
            _block = block;
        }

        public void UpdateView(bool isValueChanged)
        {
            if(_block.Type == BlockType.EMPTY)
            {
                _spriteRenderer.sprite = null;
            }
            else if(_block.Type == BlockType.BASIC)
            {
                 int elementIndex = (int)_block.BlockElement;
                _spriteRenderer.sprite = _blockConfig.basicBlockSprites[elementIndex];
            }
        }

        public void DoActionClear()
        {
            Destroy(gameObject);
        }
    }
}
