using EasyTransition;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager current;
    public static bool endGame = false;

    void Awake(){
        if(current == null)
            current = this;
    }

    [Header("Menu")]
    [SerializeField] private GameObject menu_obj;
    [SerializeField] private TextMeshProUGUI coins_txt;
    [SerializeField] private TextMeshProUGUI timer_txt;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_remember;
    
    float min, sec;

    public static int qtdRemember = 5;

    void Start()
    {
        min = 0;sec = 0;
        qtdRemember = 5;
        endGame = false;
    }

    void Update()
    {
        if(endGame) return;

        sec += Time.deltaTime;
        if(sec >= 60){
            min++;
            sec = 0;
        }
    }

    public void Restart()
    {
        // Recarregar a página
        TransitionManager.Instance().Transition(SceneManager.GetActiveScene().name,GameController.instance.transition,0f);
    }

    public void Menu()
    {
        // Ir até o menu
        TransitionManager.Instance().Transition("Menu",GameController.instance.transition,0f);
    }

    public void Next()
    {
        Restart();
        GameController.Level_global++;
        SaveSystem.Save("Level",GameController.Level_global);
    }

    public void MenuObj(bool enabled){
        menu_obj.SetActive(enabled);

        endGame = enabled;

        int m = (int)min;
        int s = (int)sec;

        if (enabled){
            timer_txt.text = m.ToString("00") + ":" + s.ToString("00");
        }
    }

    public void Remember_btn()
    {
        qtdRemember--;
        txt_remember.text = qtdRemember.ToString("D1");
    }
}
