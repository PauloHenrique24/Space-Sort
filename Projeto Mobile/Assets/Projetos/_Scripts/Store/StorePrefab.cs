using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorePrefab : MonoBehaviour
{
    [Header("Style")]
    [SerializeField] private Image icone;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI value;

    [Space]
    [SerializeField] private GameObject btn_buy;
    [SerializeField] private GameObject locked;
    [SerializeField] private GameObject selected_obj;

    public bool _unlocked = false;
    public bool _selected = false;

    private StoreItem item;
    
    public int id;

    [HideInInspector]
    public bool default_;

    void Awake()
    {
        _selected = false;
        _unlocked = false;
    }

    public void Style(StoreItem item)
    {
        icone.sprite = item.icon;
        this.item = item;
        
        default_ = item.default_;
        id = item.id;

        if(SaveSystem.Exists("Store List")){
            StoreManager.current.store_list = SaveSystem.Load<List<int>>("Store List");

            foreach (var i in StoreManager.current.store_list)
                if(id == i){ _unlocked = true; break;}
        }

        if(SaveSystem.Exists($"Store {item.type}"))
        {
            if(id == SaveSystem.Load<int>($"Store {item.type}")) {_selected = true;}
            else _selected = false;
        }

        if(item.typeBuy == TypeBuyStore.Level) 
        {
            level.gameObject.SetActive(true);
            level.text = "Level " + item.value.ToString("");
        }
        else if(item.typeBuy == TypeBuyStore.Value)
        {
            btn_buy.SetActive(true);
            value.text = item.value.ToString("");
        }

        if (_unlocked || item.default_)
        {
            Unlock();

            if (_selected)
            {
                gameObject.GetComponent<Image>().color = Color.green;
                selected_obj.SetActive(true);
            }
        }else{
            gameObject.GetComponent<Image>().color = Color.gray;
            icone.GetComponent<Image>().color = Color.gray;
        }
    }

    void Unlock()
    {
        gameObject.GetComponent<Image>().color = Color.white;
        icone.GetComponent<Image>().color = Color.white;

        locked.SetActive(false);
        btn_buy.SetActive(false);
        level.gameObject.SetActive(false);
        _unlocked = true;
    }

    public void BTN_ACTION()
    {
        if ((_unlocked || item.default_) && !_selected)
        {
            // Esta desbloqueado ou é o item default, então eu seleciono este item
            // Feedback sonoro
            SelectedItem();
            return;
        }


        // Faço a verificação para poder comprar ou desbloquear este item
        if(item.typeBuy == TypeBuyStore.Level)
        {
            if(item.value <= GameController.Level_global)
            {
                Unlock();
                SaveStore(id);
                return;
            }
        }
    }

    void SaveStore(int id)
    {
        if(SaveSystem.Exists("Store List")){
            StoreManager.current.store_list = SaveSystem.Load<List<int>>("Store List");

            foreach(var i in StoreManager.current.store_list)
                if(i == id) return;
        }

        StoreManager.current.store_list.Add(id);
        SaveSystem.Save("Store List",StoreManager.current.store_list);
    }

    public void Unselect()
    {
        _selected = false;
        selected_obj.SetActive(false);
        gameObject.GetComponent<Image>().color = Color.white;
    }

    void SelectedItem()
    {
        FindFirstObjectByType<StoreManager>().Unselected();
        Selected();

        SaveSystem.Save($"Store {item.type}",item.id);
    }

    public void Selected()
    {
        _selected = true;

        selected_obj.SetActive(true);
        gameObject.GetComponent<Image>().color = Color.green;
    }
}
