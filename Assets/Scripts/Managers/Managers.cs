using UnityEngine;
using Zenject;

public class Managers : MonoBehaviour
{
    public static AudioManager AudioManager { get; private set; }
    
    public GameDifficultyManager GameDifficultyManager { get { return gameDifficultyManager; } private set { } }
    [Inject] private GameDifficultyManager gameDifficultyManager;

    private void Awake()
    {
        AudioManager = GetComponent<AudioManager>();
    }
}
