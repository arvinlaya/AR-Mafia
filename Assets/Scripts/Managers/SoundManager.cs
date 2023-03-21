using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] public AudioSource homeSource;
    [SerializeField] public AudioSource gameSource;
    [SerializeField] public AudioClip[] clips;
    // Start is called before the first frame update

    public const int DAY_PHASE_START = 0;
    public const int NIGHT_PHASE_START = 1;
    public const int MAFIA_SKILL = 2;
    public const int DETECTIVE_SKILL = 3;
    public const int DOCTOR_SKILL = 4;
    public const int DOOR_OPEN_CLOSE = 5;
    public const int TIME_ENDING = 6;
    public const int TIME_ENDS = 7;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator playGameClip(int clipIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        gameSource.PlayOneShot(clips[clipIndex]);
    }
}
