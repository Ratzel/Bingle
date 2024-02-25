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
    }
}

