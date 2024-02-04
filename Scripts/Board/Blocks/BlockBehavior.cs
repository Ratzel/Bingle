using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
namespace Dafhne.Board{
    public class BlockBehavior : MonoBehaviour
    {
        Block _block;
        SpriteRenderer _spriteRenderer;

        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

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
        }
    }
}
