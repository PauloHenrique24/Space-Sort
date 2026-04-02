using UnityEngine;

/// <summary>
/// Bola individual do puzzle Sort.
/// Controla cor, movimento e efeitos.
/// </summary>
public class Ball : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform _targetDown;
    [HideInInspector] public ColorType cor;

    Vector3 _pos, _back_move;

    // Estados
    bool _move, _movimentdown, _checkContinuos,
    _continuosball, _oneLoop, _moveBack, _remember;
    
    bool notRemember;
    bool continuos = false;

    // Referências
    SortPots _sort, _sortBack;
    SpriteRenderer _sr;

    // =========================================================

    public void Init(BallColor ball)
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();

        _sr.sprite = ball.spr;
        cor = ball.color;
    }

    void LateUpdate()
    {
        if (!_move)
            return;
        
        transform.position = Vector3.MoveTowards(transform.position, _pos, SortController.velocityBall);

        if (Vector2.Distance(transform.position, _pos) <= 0f && !_oneLoop)
        {
            _sr.sortingOrder = 2;
            _oneLoop = true;

            if (_moveBack) Retornar_Bola_Pote(); //Voltar ao pote
            else if (_continuosball) {
                Bolas_Continuas(); // Bolas da mesma cor
                return;
            }
            else if (_movimentdown){
                Bolas_Acima_Pote(); // Indo até acima do pote
                return;
            }
            else if (_checkContinuos) Destino_Final(); //Desceu até o pote
            else {
                // Indo para cima
                _sr.sortingOrder = 10;
                SimpleAudioPlayer.Instance.Play(SortController.current.soundBall); 
            }

            _move = false;
            _moveBack = false;
        }
    }

    public void EfxExplosion()
    {
        // Efx de quando a bola bate no pote
        Instantiate(SortController.current.efx_explosion, _targetDown.position, Quaternion.identity);
        SimpleAudioPlayer.Instance.Play(SortController.current.soundBallTouch);
    }

    #region Actions

    void Retornar_Bola_Pote()
    {
        //Voltar ao pote
        EfxExplosion();
    }

    void Bolas_Continuas()
    {
        // Bolas da mesma cor
        _pos = _sort.pointer_up.position;

        _sortBack.balls_.Remove(this);
        _sort.balls_.Add(this);

        transform.SetParent(_sort.transform);

        _move = true;
        _movimentdown = true;
        _oneLoop = false;

        _continuosball = false;
        _checkContinuos = false;
        notRemember = true;
    }

    void Bolas_Acima_Pote()
    {
        // Indo até o acima do pote
        DesableMoviments();
        _pos = SortController.current.AlturaPos(_sort);
        
        _checkContinuos = true;
        _oneLoop = false;
        _move = true;
    }

    void Destino_Final()
    {
        //Desceu até o pote
        if(_sortBack != null && !_remember){
            continuos = _sortBack.BallContinuos(cor);
        }

        if(_sort != null){
            if (!_sort.Limite() && _sortBack){
                if(continuos){
                    _sortBack.BallContinuos(_sort, cor, _sortBack);
                }
            }

            _sort.CompleteLimite();
        }

        EfxExplosion();
        DesableMoviments();

        if(!notRemember)
            GameManager.current.AddMemoryBall(this,_sortBack,_sort, continuos);
    }

    void DesableMoviments()
    {
        _movimentdown = false;
        _checkContinuos = false;
        _continuosball = false;
    }

    #endregion
  
    #region Movimentos

    public void BackMove()
    {
        _moveBack = true;
        _oneLoop = false;
        _remember = false;
    }
    void Move(Vector3 pos, SortPots pot, SortPots sortBack)
    {
        _pos = pos;
        _sort = pot;
        _sortBack = sortBack;

        _move = true;
        _oneLoop = false;
    }
    public void MovimentBall(Vector3 pos)
    {
        _back_move = transform.position;
        _pos = pos;

        _move = true;
        _oneLoop = false;

        notRemember = false;
        _remember = false;
    }
    public void MovimentBall(Vector3 pos, SortPots pot, SortPots sortBack)
    {
        Move(pos,pot,sortBack);
        notRemember = false;
        _remember = false;
        _movimentdown = true;
    }
    public void MovimentBallContinuos(SortPots sort, SortPots sortBack)
    {
        Move(sortBack.pointer_up.position,sort,sortBack);
        notRemember = false;
        _remember = false;
        _continuosball = true;
    }
    public void MovimentBallContinuos(SortPots sort, SortPots sortBack, bool continuos)
    {
        Move(sortBack.pointer_up.position,sort,sortBack);
        _continuosball = true;
        this.continuos = continuos;

        _remember = true;
        notRemember = true;
    }
    public void MovimentBack()
    {
        _pos = _back_move;
        _move = true;
        _remember = false;
        notRemember = false;
    }
    
    #endregion
}