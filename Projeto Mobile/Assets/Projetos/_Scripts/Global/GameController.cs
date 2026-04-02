using System.Collections;
using System.Collections.Generic;
using EasyTransition;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static int Level_global = 1;

    [Header("Store")]
    public List<StoreItem> itensStore = new();
    [Space] public TransitionSettings transition;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);

        Level_global = SaveSystem.Load("Level",1);
    }

    public void Next_Level()
    {
        StartCoroutine(Victory());
    }

    public IEnumerator Victory()
    {
        yield return new WaitForSeconds(1f);
        //Ativa o menu de victory!

        SimpleAudioPlayer.Instance.Play(SortController.current.victory_sound);
        InterfaceManager.current.MenuObj(true);
    }

    public int IdStore(int id)
    {
        for (int i = 0; i < itensStore.Count; i++)
        {
            if(itensStore[i].id == id)
                return i;
        }
        
        return 0;
    }

    public void TransitionGame(string scene)
    {
        TransitionManager.Instance().Transition(scene,transition,0f);
    }
}

[System.Serializable]
public struct BallColor
{
    public Sprite spr;
    public ColorType color;
}

public enum ColorType
{
    white,
    blue,
    green,
    purple,
    red,
    orange,
    yellow,
    black
}
