using System.Collections;
using UnityEngine;

namespace Dafhne.Util
{
    public class Action2D : MonoBehaviour
    {
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool isSelfRemove = false)
        {
            Vector2 startPos = target.transform.position;

            float elapsed = 0.0f;
            while(elapsed  < duration)
            {
                elapsed += Time.smoothDeltaTime;
                target.transform.position = Vector2.Lerp(startPos, to, elapsed /duration);

                yield return null;
            }

            target.transform.position = to;

            if(isSelfRemove)
            {
                Object.Destroy(target.gameObject, 0.1f);
            }

            yield break;
        }
        
        //parm toScale 커지는(줄어드는) 크기, 예를 들어, 0.5인 경우 현재 크기에서 절반으로 줄어든다. 
        //parm speed 초당 커지는 속도, 예를 들어, 2인 경우 초당 2배 만큼 커지거나 줄어든다. 
        public static IEnumerator Scale(Transform target, float toScale, float speed)
        {
            //1. 방향 결정 : 커지는 방향이면 +, 줄어드는 방향이면 -
            bool isInc = target.localScale.x < toScale;
            float fDir = isInc ? 1 : -1;

            float factor;
            while(true)
            {
                factor = Time.deltaTime * speed * fDir;
                target.localScale = new Vector3(target.localScale.x + factor, target.localScale.y + factor, target.localScale.z);

                if(!isInc && target.localScale.x <= toScale || isInc && target.localScale.x >= toScale)
                {
                    break;
                }

                yield return null; 
            }

            yield break;
        }
    }
}

