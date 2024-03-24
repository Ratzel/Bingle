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
            // Destroy(gameObject);
            StartCoroutine(CoStartSimpleExplosion(true));
        }

        IEnumerator CoStartSimpleExplosion(bool isDestroy = true)
        {
            yield return Util.Action2D.Scale(transform, Core.Constants.BLOCK_DESTORY_SCALE, 4f);

            //1. 폭파싴키는 효과 연출 : 블럭 자체의 Clear효과를 연출한다. (모든 블럭 동일)
            GameObject explosionObj = _blockConfig.GetExplosionObject(BlockQuestType.CLEAR_SIMPLE);
            ParticleSystem.MainModule newModule = explosionObj.GetComponent<ParticleSystem>().main;
            newModule.startColor = _blockConfig.GetBlockColor(_block.BlockElement);
            explosionObj.SetActive(true);
            explosionObj.transform.position = this.transform.position;

            yield return new WaitForSeconds(0.1f);

            //2. 블럭 GameObject 갳체 삭제 or make size zero
            if(isDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Assert(false, "Unknown Action : Gameobject No Destory After Particle");
            }
        }
    }
}
