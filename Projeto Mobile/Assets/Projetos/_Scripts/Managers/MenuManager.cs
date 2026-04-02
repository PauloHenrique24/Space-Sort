using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string scenePlay = "Game";
    [SerializeField] private string sceneStore = "Store";

    [SerializeField] private TextMeshProUGUI _Leveltxt;
    [SerializeField] private AudioClip btn_clip;

    void Start()
    {
        _Leveltxt.text = "LEVEL " + SaveSystem.Load("Level",1).ToString("00");
    }

    public void BtnClickSound()
    {
        SimpleAudioPlayer.Instance.Play(btn_clip);
    }

    public void Play(){
        GameController.instance.TransitionGame(scenePlay);
        BtnClickSound();
    }

    public void Store(){
        GameController.instance.TransitionGame(sceneStore);
        BtnClickSound();
    }

    public void Anuncios(){
        print("Comprar versão sem anuncios!");
        BtnClickSound();
    }
}
