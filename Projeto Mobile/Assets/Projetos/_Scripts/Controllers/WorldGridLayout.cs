using UnityEngine;

public class WorldGridLayout : MonoBehaviour
{
    public int columns = 5;
    public float cellWidth = 1.5f;
    public float cellHeight = 1.5f;
    public Vector3 startOffset = Vector3.zero;

    void Start()
    {
        Arrange();
    }

    void Arrange()
    {
        int total = transform.childCount;

        int rows = 2;
        int columns = Mathf.CeilToInt(total / (float)rows);

        // tamanho total do grid
        float totalWidth = (columns - 1) * cellWidth;
        float totalHeight = (rows - 2) * cellHeight;

        // offset pra centralizar
        Vector3 centerOffset = new Vector3(totalWidth / 2f, -totalHeight / 2f, 0);

        for (int i = 0; i < total; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float x = col * cellWidth;
            float y = -row * cellHeight;

            Vector3 pos = new Vector3(x, y, 0) - centerOffset;

            transform.GetChild(i).localPosition = pos;
        }
    }
}
