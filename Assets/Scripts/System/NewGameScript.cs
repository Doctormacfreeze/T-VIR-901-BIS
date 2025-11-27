using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class NewGameScript : MonoBehaviour
{

    public DisplayObjScript displayObj;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        launchNewGame();
    }

    public void launchNewGame()
    {
        GameManager.Instance.StartGame();
        SceneManager.LoadScene("mainscene");
        SceneManager.UnloadScene("lobby");
        displayObj.Refresh();
    }
}
