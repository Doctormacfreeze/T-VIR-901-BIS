using UnityEngine;
using TMPro;

public class LifeMenuScript : MonoBehaviour
{
    public TextMeshProUGUI lifeText;


    
    void OnEnable(){
        lifeText.text = "Vie pleine";
    }
}
