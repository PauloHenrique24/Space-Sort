using UnityEngine;

[CreateAssetMenu(fileName = "Store Item",menuName = "Scriptable Objects/Store")]
public class StoreItem : ScriptableObject
{
    public Sprite icon;
    public Sprite sprite_item;
    [Space]
    public TypeBuyStore typeBuy;
    public TypeStore type;
    [Space]
    public int value;

    [Header("Id")]
    public int id = 00001;

    [Space]
    public bool default_;

    [Space]
    [Header("Se for Ball")]
    public BallsItem item_ball;
}
