using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace Dafhne.Util
{
    public enum Clip
    {
        Chomp = 0,
        BlockClear = 1,
    };

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        private AudioSource[] sfx;

        //시작시 컴포넌트를 구해서 singleton으로 저장한다. 
        //어디에서나 SoundManager.PlayOnShot()으로 사운드를 플레이 할수 있다. 

        void Start()
        {
            instance = GetComponent<SoundManager>();
            sfx = GetComponents<AudioSource>();
        }

        public void PlayOneShot(Clip audioClip)
        {
            sfx[(int)audioClip].Play();
        }

        public void PlayOneShot(Clip audioClip, float volumeScale)
        {
            AudioSource source = sfx[(int)audioClip];
            source.PlayOneShot(source.clip, volumeScale);
        }
    }

}