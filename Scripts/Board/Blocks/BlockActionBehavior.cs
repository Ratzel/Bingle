using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dafhne.Scriptable;
using Dafhne.Util;
using Unity.PlasticSCM.Editor.WebApi;

namespace Dafhne.Board
{
    public class BlockActionBehavior : MonoBehaviour
    {
        [SerializeField] BlockConfig _blockConfig;
        public bool isMoving { get; set; }

        Queue<Vector3> _movementQueue = new Queue<Vector3>(); // x=col, y=row, z = acceleration

        //아래쪽으로 주어진 거리만큼 이동한다.
        //DropDistance : 이동할 스탭 수 즉, 거리 (uint)

        public void MoveDrop(Vector2 vecDropDistance)
        {
            _movementQueue.Enqueue(new Vector3(vecDropDistance.x, vecDropDistance.y, 1));
            
            if(!isMoving)
            {
                StartCoroutine(DoActionMoveDrop());
            }
        }

        IEnumerator DoActionMoveDrop(float acc = 1.0f)
        {
            isMoving = true;
             
            while(_movementQueue.Count > 0)
            {
                Vector2 vecDestination = _movementQueue.Dequeue();

                int dropIndex = System.Math.Min(9, System.Math.Max(1, (int)Mathf.Abs(vecDestination.y)));
                float duration = _blockConfig.dropSpeed[dropIndex-1];
                yield return CoStartDropSmooth(vecDestination, duration * acc);
            }

            isMoving = false;
            yield break;
        }

        IEnumerator CoStartDropSmooth(Vector2 vecDropDistance, float duration)
        {
            Vector3 to = new Vector3(transform.position.x + vecDropDistance.x, transform.position.y - vecDropDistance.y, transform.position.z);
            yield return Action2D.MoveTo(transform, to, duration);
        }
    }

}
