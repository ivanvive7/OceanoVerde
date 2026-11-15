using UnityEngine;

public static class Sfx
{
    static AudioSource source;

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        if (source == null)
        {
            var go = new GameObject("SfxPlayer");
            Object.DontDestroyOnLoad(go);
            source = go.AddComponent<AudioSource>();
            source.spatialBlend = 0f;   // 2D: mismo volumen siempre
            source.playOnAwake = false;
        }

        source.PlayOneShot(clip, volume);
    }
}