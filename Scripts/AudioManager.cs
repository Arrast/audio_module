using System.Collections.Generic;
using UnityEngine;
using versoft.asset_manager;

namespace versoft.audio_manager 
{
    public class AudioManager
    {
        private AudioSource musicAudioSource;
        private AudioSource sfxAudioSource;
        private AssetManager assetManager;

        private Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();
        private float defaultMusicVolume = 0.75f;

        public AudioManager()
        {
            // Let's create the audiosource.
            GameObject audioSourceGO = new GameObject(Const.AudioSourceGameObjectName);
            Object.DontDestroyOnLoad(audioSourceGO);

            // Add one source for the music
            musicAudioSource = audioSourceGO.AddComponent<AudioSource>();
            musicAudioSource.loop = true;
            SetMusicVolume();

            // Add one source for the SFX
            sfxAudioSource = audioSourceGO.AddComponent<AudioSource>();

            // Store the AssetManager
            assetManager = ServiceLocator.Instance.Get<AssetManager>();
        }

        public void PlayMusic(string clipId)
        {
            PlayAudio(clipId, musicAudioSource);
        }

        public void PlaySFX(string clipId)
        {
            PlayAudio(clipId, sfxAudioSource);
        }

        private async void PlayAudio(string clipId, AudioSource source, string fileExtension = Const.DefaultAudioFileExtension)
        {
            if (!audioClipCache.TryGetValue(clipId, out AudioClip audioClip))
            {
                // Load the clip
                audioClip = await assetManager.LoadAsset<AudioClip>(string.Format(Const.AudioFilePath, clipId), fileExtension);
            }

            if (audioClip != null)
            {
                source.Stop();
                source.clip = audioClip;
                source.PlayDelayed(0.0f);
            }
        }

        public void SetMusicVolume(float volume = float.MinValue)
        {
            if (volume < 0.0f)
            {
                volume = defaultMusicVolume;
            }
            musicAudioSource.volume = Mathf.Clamp01(volume);
        }
    }
}