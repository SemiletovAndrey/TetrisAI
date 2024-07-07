using UnityEngine;

public class Managers : MonoBehaviour
{
    public static AudioManager AudioManager { get; private set; }
    public static GameDifficultyManager GameDifficultyManager { get; private set; }

    private void Awake()
    {
        AudioManager = GetComponent<AudioManager>();
        GameDifficultyManager = GetComponent<GameDifficultyManager>();
    }
}
