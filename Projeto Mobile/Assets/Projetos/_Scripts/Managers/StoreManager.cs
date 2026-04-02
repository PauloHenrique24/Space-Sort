using System;
using System.Collections.Generic;
using EasyTransition;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager current;

    void Awake()
    {
        if(current == null)
            current = this;
    }

    [Header("Store Controller")]
    [SerializeField] private GameObject _objStore;
    [SerializeField] private Transform _parent_Store;

    [Header("Style")]
    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private GameObject[] _btns;

    [Header("Componentes")]
    [SerializeField] private Sprite _blueSpr;
    [SerializeField] private Sprite _graySpr;

    TypeStore _typeSelect = TypeStore.Bola;
    float posUp,posDown;

    [HideInInspector] public List<StorePrefab> itens_store = new();
    [HideInInspector] public List<int> store_list = new();

    void Start()
    {
        posUp = _btns[0].transform.position.y;
        posDown = _btns[1].transform.position.y;

        if(SaveSystem.Exists("Store List"))
            store_list = SaveSystem.Load<List<int>>("Store List");

        BtnsSelected(1);
        Check_Save_Store();
        _GenerateStoreItens();
    }

    void _GenerateStoreItens()
    {
        DestroyStore();

        foreach(var s in GameController.instance.itensStore)
        {
            if(s.type == _typeSelect){
                var item = Instantiate(_objStore,_parent_Store);
                item.GetComponent<StorePrefab>().Style(s);

                itens_store.Add(item.GetComponent<StorePrefab>());
            }
        }
    }

    void StyleStore(string title)
    {
        _titleTxt.text = title;
        _GenerateStoreItens();
    }

    void DestroyStore()
    {
        itens_store.Clear();
        for (int i = 0; i < _parent_Store.childCount; i++)
        {
            Destroy(_parent_Store.GetChild(i).gameObject);
        }
    }

    public void BtnType(int t)
    {
        _typeSelect = (TypeStore)t;
        StyleStore(_typeSelect.ToString());

        BtnsSelected(t);
        Check_Save_Store();
    }

    void BtnsSelected(int indice) //0 = tubo; 1 = ball; 2 = bg
    {
        for (int i = 0; i < _btns.Length; i++)
        {
            _btns[i].GetComponent<Image>().sprite = _graySpr;
            _btns[i].transform.GetChild(0).GetComponent<Image>().color = new Color(255,255,255,.4f);
            _btns[i].transform.position = new Vector3(_btns[i].transform.position.x,posDown);
        }

        _btns[indice].GetComponent<Image>().sprite = _blueSpr;
        _btns[indice].transform.GetChild(0).GetComponent<Image>().color = new Color(255,255,255,1f);
        _btns[indice].transform.position = new Vector3(_btns[indice].transform.position.x,posUp);
    }

    // Desselected All Stores Itens
    public void Unselected()
    {
        foreach(var i in itens_store) i.Unselect();
    }

    public void Check_Save_Store()
    {
        TypeStore t = _typeSelect;
            
        if(SaveSystem.Exists($"Store {t}"))
        {
            int id = SaveSystem.Load($"Store {t}",0);

            foreach(var item in itens_store)
            {
                if(item.id == id)
                {
                    item.Selected();
                    return;
                }
            }
        }
        else
        {
            foreach(var item in itens_store)
                if (item.default_) item.Selected();
        }
    }

    public void Return(){
        // Volta ao menu
        TransitionManager.Instance().Transition("Menu",GameController.instance.transition,0f);
    }
}

[Serializable]
public class ItensStore
{
    public string nome;
    public List<StoreItem> storeItem = new();
}

public enum TypeBuyStore
{
    Level,
    Value,
    Announcement
}

public enum TypeStore : int
{
    Tubo = 0,
    Bola = 1,
    Fundo = 2
}
