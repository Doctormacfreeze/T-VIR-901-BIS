using UnityEngine;

public class GameManager : MonoBehaviour
{

	public GameManager Instance;
	public GameData CurrentGame;
	public bool isPlaying = false;

	void Awake(){
	if(Instance == null){
		Instance = this;
		DontDestroyOnLoad(gameObject);
		StartGame();
	}else
		Destroy(gameObject);
	}

	public void StartGame(){
		isPlaying = true;
		CurrentGame = new GameData();
		ObjectiveManager.Instance.StartNextObjective();
	}
}
