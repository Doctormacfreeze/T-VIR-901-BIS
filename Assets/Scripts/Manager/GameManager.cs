using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance;
	public GameData CurrentGame;
	public DisplayObjScript displayObj;
	public bool isPlaying = false;

	void Awake(){
	if(Instance == null){
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}else
		Destroy(gameObject);
	}

	public void StartGame(){
		isPlaying = true;
		CurrentGame = new GameData();
	}
}
