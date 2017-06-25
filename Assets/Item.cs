using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, LayoutItem
{
    private Image img;
    private Text txt;

    void Awake()
    {
        img = this.transform.GetComponent<Image>();
        txt = this.transform.FindChild("Text").GetComponent<Text>();
    }

    private void RefreshUI()
    {
        txt.text = gridIdx.ToString();
        img.color = gridIdx % 2 == 0 ? Color.yellow : Color.blue;
    }

    [ContextMenu("本地坐标")]
    private void Pos()
    {
        Debug.LogWarning(this.transform.localPosition);
    }

    #region LayoutItem

    private int gridIdx;
    private bool gridState;

    public void SetGridIdx(int idx, Vector3 localPos)
    {
        gridIdx = idx;
        this.transform.localPosition = localPos;
        RefreshUI();
        //Debug.LogWarning("xxx " + idx);
    }

    public int GetGridIdx()
    {
        return gridIdx;
    }

    public void SetGridState(bool state)
    {
        gridState = state;
    }

    public bool GetGridState()
    {
        return gridState;
    }

    #endregion LayoutItem
}