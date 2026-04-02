using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controla cada pote do puzzle de bolas (Sort Ball).
/// Responsável por gerar bolas, selecionar pote,
/// mover bolas entre potes e detectar conclusão.
/// </summary>
public class SortPots : MonoBehaviour
{
    [Header("Nome")]
    public string id;

    [Header("Configuração de Bolas")]
    public Transform _pointer_ball;
    public Transform pointer_up;

    [HideInInspector] public bool selected;
    [HideInInspector] public bool _isBalls;
    [HideInInspector] public List<Ball> balls_ = new();

    [HideInInspector] public float _altura;

    int _maxPot = 4;
    bool complete;

    public void OnInteractPote() => SelecionarPote();

    public bool Limite() => !(balls_.Count < _maxPot);

    // =========================================================
    // SELEÇÃO E LÓGICA PRINCIPAL
    // =========================================================

    void SelecionarPote()
    {
        if (complete) return;

        var sc = SortController.current;

        // ===============================
        // Clique no mesmo pote selecionado
        // ===============================
        if (selected){
            ReturnBall(ref balls_);
            return;
        }

        // ===============================
        // Nenhum pote selecionado ainda
        // ===============================
        if (!sc.selectedPot)
        {
            BallMove(pointer_up.position, ref balls_);

            selected = true;
            sc.selectedPot = true;
            sc.sortSelected = this;
            return;
        }

        // ===============================
        // Já existe pote selecionado
        // ===============================
        if (!Limite()) // Se há espaço no pote destino
        {
            if (balls_.Count > 0)
            {
                // Verifica se a cor da bola destino combina
                if (sc.sortSelected != null &&
                    IndexEndBall(ref balls_).cor ==
                    IndexEndBall(ref sc.sortSelected.balls_).cor)
                {
                    MoveBall();
                }
                else
                {
                    // Cor diferente → bola volta ao pote original
                    ReturnBall(ref sc.sortSelected.balls_);

                    BallMove(pointer_up.position, ref balls_);

                    selected = true;
                    sc.selectedPot = true;
                    sc.sortSelected = this;
                    return;
                }
            }
            else
            {
                // Pote vazio → movimento permitido
                MoveBall();
            }

            SortController.current.DesselectPots();
        }
        else
        {
            // Pote cheio → devolve bola
            ReturnBall(ref sc.sortSelected.balls_);

            BallMove(pointer_up.position, ref balls_);

            selected = true;
            sc.selectedPot = true;
            sc.sortSelected = this;
            return;
        }
    }

    // =========================================================
    // MOVIMENTAÇÃO
    // =========================================================

    /// <summary>
    /// Move a última bola do pote selecionado para este pote
    /// </summary>
    void MoveBall()
    {
        var sc = SortController.current;

        // Anima subida e transição
        BallMove(pointer_up.position, ref sc.sortSelected.balls_, this, sc.sortSelected);

        // Troca o parent da bola para o novo pote
        IndexEndBall(ref sc.sortSelected.balls_).transform.SetParent(transform);

        // Adiciona no novo pote
        balls_.Add(IndexEndBall(ref sc.sortSelected.balls_));

        // Remove do pote antigo
        sc.sortSelected.balls_.RemoveAt(sc.sortSelected.balls_.Count - 1);
    }

    void ReturnBall(ref List<Ball> ball)
    {
        BallBack(ref ball);
        IndexEndBall(ref ball).BackMove();

        selected = false;
        SortController.current.DesselectPots();
    }

    // =========================================================
    // COMPLETAR POTE
    // =========================================================

    public void CompleteLimite()
    {
        if (!Limite()) return;

        ColorType cor = balls_[0].cor;

        // Verifica se todas as bolas são da mesma cor
        foreach (var b in balls_)
        {
            if (b.cor != cor)
                return;
        }

        // Pote concluído
        complete = true;
        SortController.current.Complete(transform.position);
    }

    public void BallContinuos(SortPots sortDestiny, ColorType cor, SortPots sortBack)
    {
        if (balls_.Count == 0) return;

        if (IndexEndBall(ref balls_).cor == cor)
        {
            var ball = IndexEndBall(ref balls_);
            BallMove(sortDestiny, sortBack, ref ball);
        }
    }

    public bool BallContinuos(ColorType cor)
    {
        if (balls_.Count == 0) return false;

        if (IndexEndBall(ref balls_).cor == cor)
        {
            return true;
        }
        
        return false;
    }

    // Dar uma atenção a isso aqui
    void BallBack(ref List<Ball> ball)
    {
        if (IndexEndBall(ref ball) != null)
            IndexEndBall(ref ball).MovimentBack();
    }

    void BallMove(Vector3 mov, ref List<Ball> ball)
    {
        if (IndexEndBall(ref ball) != null)
            IndexEndBall(ref ball).MovimentBall(mov);
    }

    void BallMove(Vector3 mov, ref List<Ball> ball, SortPots sort, SortPots sortBack)
    {
        if (IndexEndBall(ref ball) != null)
            IndexEndBall(ref ball).MovimentBall(mov, sort, sortBack);
    }

    void BallMove(SortPots sortDestiny, SortPots sortBack, ref Ball ball){
        ball.MovimentBallContinuos(sortDestiny, sortBack);
    }

    /// <summary>
    /// Retorna sempre a última bola da lista
    /// </summary>
    Ball IndexEndBall(ref List<Ball> ballList)
    {
        if (ballList.Count > 0)
            return ballList[^1];

        return null;
    }
}
