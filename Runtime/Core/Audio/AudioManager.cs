using UnityEngine;

namespace UMK.Core.Audio
{
    /// <summary>
    /// Centralised audio manager supporting music, ambient and SFX channels. It controls global
    /// volume levels and plays audio clips on dedicated AudioSources. Extend this to add categories
    /// or network replication via NetBehaviourBase.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public AudioSource musicSource;
        public AudioSource ambientSource;
        public AudioSource sfxSource;
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float ambientVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        void Awake()
        {
            // Create sources if missing
            if (!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
            if (!ambientSource) ambientSource = gameObject.AddComponent<AudioSource>();
            if (!sfxSource) sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            ambientSource.loop = true;
        }
        void Update()
        {
            ApplyVolumes();
        }
        private void ApplyVolumes()
        {
            musicSource.volume = masterVolume * musicVolume;
            ambientSource.volume = masterVolume * ambientVolume;
            sfxSource.volume = masterVolume * sfxVolume;
        }
        public void PlayMusic(AudioClip clip)
        {
            if (!clip) return;
            musicSource.clip = clip;
            musicSource.Play();
            EventBus.Publish(new MusicEvent { clip = clip, manager = this });
        }
        public void PlayAmbient(AudioClip clip)
        {
            if (!clip) return;
            ambientSource.clip = clip;
            ambientSource.Play();
            EventBus.Publish(new AmbientEvent { clip = clip, manager = this });
        }
        public void PlaySFX(AudioClip clip)
        {
            if (!clip) return;
            sfxSource.PlayOneShot(clip);
            EventBus.Publish(new SFXEvent { clip = clip, manager = this });
        }
        // Events for bus
        public struct MusicEvent { public AudioClip clip; public AudioManager manager; }
        public struct AmbientEvent { public AudioClip clip; public AudioManager manager; }
        public struct SFXEvent { public AudioClip clip; public AudioManager manager; }
    }
}