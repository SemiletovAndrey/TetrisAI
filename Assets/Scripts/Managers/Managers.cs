using UnityEngine;
using Zenject;

public class Managers : MonoBehaviour
{
    public static AudioManager AudioManager { get; private set; }
    
    public GameDifficultyManager GameDifficultyManager { get { return gameDifficultyManager; } private set { } }
    private GameDifficultyManager gameDifficultyManager;

    [Inject]
    public void Construct([InjectOptional] GameDifficultyManager gameDifficultyManager)
    {
        this.gameDifficultyManager = gameDifficultyManager;
    }

    private void Awake()
    {
        AudioManager = GetComponent<AudioManager>();
    }
}
