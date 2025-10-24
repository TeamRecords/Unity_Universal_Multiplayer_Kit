using UnityEngine;

namespace UMK.Core.Audio
{
    /// <summary>
    /// Component that can emit audio clips. Other systems can acquire a reference to an IAudioEmitter
    /// and call its methods to play local or networked sounds. Network replication can be achieved by
    /// extending this class and using NetBehaviourBase to send RPCs when Mirror is present.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioEmitter : MonoBehaviour, IAudioEmitter
    {
        private AudioSource _source;
        void Awake() { _source = GetComponent<AudioSource>(); }
        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (!_source || !clip) return;
            _source.PlayOneShot(clip, volume);
            // Publish event for other listeners (e.g., AI, remote clients)
            EventBus.Publish(new AudioEvent { clip = clip, volume = volume, position = transform.position, emitter = this });
        }
        public void PlayLoop(AudioClip clip, float volume = 1f)
        {
            if (!_source || !clip) return;
            _source.loop = true;
            _source.clip = clip;
            _source.volume = volume;
            _source.Play();
        }
        public void StopLoop()
        {
            if (_source) _source.Stop();
        }
        public struct AudioEvent
        {
            public AudioClip clip;
            public float volume;
            public Vector3 position;
            public AudioEmitter emitter;
        }
    }
}