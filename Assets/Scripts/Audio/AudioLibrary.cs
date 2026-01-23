using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public AudioID id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public List<AudioEntry> entries;

    private Dictionary<AudioID, AudioEntry> _lookup;

    public AudioEntry Get(AudioID id)
    {
        if (_lookup == null)
        {
            _lookup = new Dictionary<AudioID, AudioEntry>();
            foreach (var e in entries)
                _lookup[e.id] = e;
        }
        return _lookup[id];
    }
}
