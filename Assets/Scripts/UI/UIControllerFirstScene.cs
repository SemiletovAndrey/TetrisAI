using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControllerFirstScene : MonoBehaviour
{
    public void StartGameTetris()
    {
        SceneManager.LoadScene("TetrisGame");
    }
    public void StartGameTetrisAI()
    {
        
        SceneManager.LoadScene("MLTetrisGameAI");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
