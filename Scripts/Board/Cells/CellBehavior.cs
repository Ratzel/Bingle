using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dafhne.Board
{

    public class CellBehavior : MonoBehaviour
    {
        Cell _cell;
        SpriteRenderer _spriteRenderer;

        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateView(false);
        }

        public void SetCell(Cell cell)
        {
            _cell = cell;
        }
        public void UpdateView(bool isValueChanged)
        {
            if(_cell.Type == CellType.EMPTY)
            {
                _spriteRenderer.sprite = null;
            }
        }
    }

}
