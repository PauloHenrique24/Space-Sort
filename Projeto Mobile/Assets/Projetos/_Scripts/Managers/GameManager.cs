using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    void Awake()
    {
        if(current == null)
            current = this;
    }

    [Header("Mudanças")]
    [SerializeField] private Image bg;

    [Header("Balls Remember")]
    private int qtdRemember = 5;
    public List<RememberBall> remember = new(); 

    void Start()
    {
        if(SaveSystem.Exists($"Store {TypeStore.Fundo}")){
            foreach(var i in GameController.instance.itensStore)
            {
                if(i.id == SaveSystem.Load<int>($"Store {TypeStore.Fundo}"))
                {
                    bg.sprite = i.sprite_item;
                    break;
                }
            }
        }
    }

    public void AddMemoryBall(Ball ball, SortPots pot,SortPots potAtual, bool continuos)
    {
        if(InterfaceManager.qtdRemember <= 0)
            return;

        if(remember.Count >= qtdRemember){
            remember.RemoveAt(0);
        }

        RememberBall rb = new(){
            ball = ball,
            pot = pot,
            potAtual = potAtual,
            continuos = continuos
        };

        remember.Add(rb);
    }

    public void ReturnBall()
    {
        if(remember.Count <= 0 || InterfaceManager.qtdRemember <= 0)
            return;

        RememberBall rb = remember[^1];
        rb.ball.MovimentBallContinuos(rb.pot,rb.potAtual,rb.continuos);

        remember.Remove(rb);
        InterfaceManager.current.Remember_btn();
    }
}

[Serializable]
public struct RememberBall{
    public Ball ball;
    public SortPots pot;
    public SortPots potAtual;

    public bool continuos;
}
