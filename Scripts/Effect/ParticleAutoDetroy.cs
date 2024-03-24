using System.Collections;
using UnityEngine;

namespace Dafhne.Effect
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAutoDetroy : MonoBehaviour
    {
        private void OnEnable() 
        {
            
        }

        IEnumerator CoCheckAlive()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                if(!GetComponent<ParticleSystem>().IsAlive(true))
                {
                    Destroy(this.gameObject);

                    break;
                }
            }
        }
    }
}
