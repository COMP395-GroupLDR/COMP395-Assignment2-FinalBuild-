using UnityEngine;

public class CarAudioManager : MonoBehaviour
{
    public enum AudioType
    {
        NONE = 0,
        HANDBRAKE = 1,
        SEATBELT = 2,
        KEY_TURN = 3,
        DRIVING = 4,
        ENGINE_IDLE = 5,
        ENGINE_START = 6,
        ENGINE_STOP = 7,
        HORN = 8,
        DIRECTIONAL_LIGHT_ON = 9,
        MODE_BUTTON = 10,
        CAR_STOPPING = 11,
        ENGINE_RUN = 12,
    }

    [Tooltip("Please assign according to the audio type order")]
    [SerializeField] private AudioSource[] audioSources;

    public void PlayAudio(AudioType type, bool loop, bool replayWhenPlaying = true)
    {
        ControlAudio(type, loop, true, replayWhenPlaying);
    }

    public void StopAudio(AudioType type, bool replayWhenPlaying = true)
    {
        ControlAudio(type, false, false, replayWhenPlaying);
    }

    private void ControlAudio(AudioType type, bool loop, bool play, bool replayWhenPlaying)
    {
        if (audioSources.Length > (int)type)
        {
            AudioSource audioSource = audioSources[(int)type];
            if (audioSource != null)
            {
                if (play)
                {
                    audioSource.loop = loop;
                    if (replayWhenPlaying ||
                        (!replayWhenPlaying && !audioSource.isPlaying))
                    {
                        audioSource.Play();
                    }
                }
                else
                {
                    audioSource.Stop();
                }
            }
            else
            {
                Debug.LogWarning($"Please assign audio source for {type} in order to take effect.");
            }
        }
        else
        {
            Debug.LogWarning($"Please assign audio source for {type} in order to take effect.");
        }
    }
}
