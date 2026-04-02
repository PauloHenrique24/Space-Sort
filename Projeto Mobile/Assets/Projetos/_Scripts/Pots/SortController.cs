using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SortController : MonoBehaviour
{
    public static SortController current;

    void Awake(){
        current = current == null ? this : current;
        GeneratePots();
    }

    [Header("Components")]
    public GameObject balls_obj;
    public GameObject efx_explosion;

    [Header("EFX")]
    public ParticleSystem efx_complete;

    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI _Leveltxt;

    [Header("Configs")]
    [SerializeField] private GameObject pot_obj;
    [SerializeField] private Transform pot_parent;
    public int qtdPots = 6;

    [Header("Sounds")]
    public AudioClip soundBall;
    public AudioClip soundBallTouch;

    [Space]
    public AudioClip complete_sound;
    public AudioClip victory_sound;

    [HideInInspector] public bool selectedPot = false;
    [HideInInspector] public SortPots sortSelected;

    [Header("Movimentação")]
    public static float velocityBall = .3f;

    [Header("Potes")]
    [HideInInspector] public List<SortPots> potes;
    int potsComplete;

    void Start()
    {
        potsComplete = 0;
        SetupLevel(potes);

        _Leveltxt.text = "" + GameController.Level_global.ToString("00");
    }

    void GeneratePots()
    {
        for(int i = 0; i < qtdPots; i++)
        {
            var pot = Instantiate(pot_obj,pot_parent);

            if(i >= qtdPots - 2)
                pot.GetComponent<SortPots>()._isBalls = true;

            pot.GetComponent<SortPots>().id = i.ToString("00");

            if(SaveSystem.Exists($"Store {TypeStore.Tubo}")){
                foreach(var j in GameController.instance.itensStore)
                {
                    if(j.id == SaveSystem.Load<int>($"Store {TypeStore.Tubo}"))
                    {
                        pot.GetComponent<SpriteRenderer>().sprite = j.sprite_item;
                        break;
                    }
                }
            }

            potes.Add(pot.GetComponent<SortPots>());
        }
    }

    void SetupLevel(List<SortPots> potes)
    {
        int qtdPotes = potes.Count - 2;

        var dist = Gerar(qtdPotes); // List<List<int>>

        for (int i = 0; i < qtdPotes; i++)
        {
            if(!potes[i]._isBalls){
                potes[i].balls_.Clear();
                if(SaveSystem.Exists($"Pote {potes[i].id}"))
                {
                    List<int> colorIds = SaveSystem.Load<List<int>>($"Pote {potes[i].id}"); 
                    GenerateBallsFromIds(colorIds,potes[i]);
                }else{
                    GenerateBallsFromIds(dist[i],potes[i]);
                    SaveBalls(dist[i],potes[i].id);
                }
            }
        }
    }

    public Vector3 AlturaPos(SortPots pot)
    {
        float altura = 0f;

        // Envia a informação de qtd for para quando for mudar o script para o controlador
        for (int i = 0; i < pot.balls_.Count - 1; i++)
        {
            altura += balls_obj
                .GetComponent<SpriteRenderer>()
                .bounds.size.y;
        }

        return pot._pointer_ball.position + Vector3.up * altura;
    }

    #region BallInPots

    public void GenerateBallsFromIds(List<int> corIds, SortPots pot)
    {
        pot._altura = 0f;

        for (int i = 0; i < corIds.Count; i++)
        {
            float y = pot._pointer_ball.position.y;

            if (i > 0)
            {
                pot._altura += balls_obj.GetComponent<SpriteRenderer>().bounds.size.y;
                y += pot._altura;
            }

            var go = Instantiate(
                balls_obj,
                new Vector3(pot._pointer_ball.position.x, y),
                Quaternion.identity,
                pot.transform
            );

            var ball = go.GetComponent<Ball>();

            // pega cor pelo id (ajusta do jeito que vc guarda)
            var t = corIds[i];

            var c = GameController.instance.itensStore[GameController.instance.IdStore(SaveSystem.Load<int>("Store bola"))]
            .item_ball.ballsColor.ElementAt(t);
            
            
            ball.Init(c);
            pot.balls_.Add(ball);
        }
    }

    public static List<List<int>> Gerar(int qtdPotes)
    {
        int coresBolas = qtdPotes;
        int bolasPorCor = 4;

        // lista final de potes
        List<List<int>> potes = new();
        for (int i = 0; i < qtdPotes; i++)
            potes.Add(new List<int>());

        // cria lista de todas as bolas
        List<ColorType> bolas = new();
        for (int cor = 0; cor < coresBolas; cor++)
        {
            for (int i = 0; i < bolasPorCor; i++)
                bolas.Add((ColorType)cor);
        }

        // embaralha
        bolas = bolas.OrderBy(x => Random.value).ToList();

        // distribui
        foreach (int cor in bolas.Select(v => (int)v))
        {
            // potes que ainda não tem 4 bolas
            var candidatos = potes
                .Where(p => p.Count < bolasPorCor && !p.All(c => c == cor))
                .ToList();

            // fallback caso todos tenham mesma cor
            if (candidatos.Count == 0)
                candidatos = potes.Where(p => p.Count < bolasPorCor).ToList();

            int index = Random.Range(0, candidatos.Count);
            candidatos[index].Add(cor);
        }

        return potes;
    }

    public void GenerateBalls(int qtd, SortPots pot)
    {
        for (int i = 0; i < qtd; i++)
        {
            Vector3 pos;

            if (i == 0) pos = pot._pointer_ball.position;
            else
            {
                pot._altura += balls_obj.GetComponent<SpriteRenderer>().bounds.size.y;
                pos = pot._pointer_ball.position + Vector3.up * pot._altura;
            }

            var ball = Instantiate(
                balls_obj,
                new Vector3(pot._pointer_ball.position.x, pos.y),
                Quaternion.identity,
                pot.transform
            );

            pot.balls_.Add(ball.GetComponent<Ball>());
        }
    }


    public void Complete(Vector3 pos)
    {
        // Aqui o pote foi completo.
        SimpleAudioPlayer.Instance.Play(complete_sound);

        Instantiate(
            efx_complete,
            pos,
            Quaternion.Euler(-90, 0, 0)
        );

        AddPotsComplete();
        return;
    }

    public void DesselectPots()
    {
        sortSelected.selected = false;
        sortSelected = null;
        selectedPot = false;
    }

    #endregion

    public void AddPotsComplete()
    {
        potsComplete++;
        if(potsComplete >= potes.Count - 2) NextLevel();
    }

    public void NextLevel()
    {
        // Aqui vai ficar todo o script para passar de nível!
        GameController.instance.Next_Level();
        
        foreach(var p in potes)
            SaveSystem.Delete($"Pote {p.id}");
    }

    public void SaveBalls(List<int> data, string id)
    {
        //Salvando Lista de bolas no pote
        SaveSystem.Save($"Pote {id}",data);
    }
}
