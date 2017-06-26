using UnityEngine;
using UnityEngine.UI;

public class Item : LayoutItem
{
    private Transform head;
    private Text id;
    private Text num;

    private ItemData curData = null;
    private ItemData newData = null;

    void Awake()
    {
        head = this.transform.FindChild("head");
        id = this.transform.FindChild("id").GetComponent<Text>();
        num = this.transform.FindChild("id").GetComponent<Text>();
    }

    [ContextMenu("本地坐标")]
    private void Pos()
    {
        Debug.LogWarning(this.transform.localPosition);
    }

    public override void TryRefreshUI(ILayoutItemData data)
    {
        curData = data as ItemData;
    }

    public class ItemData : ILayoutItemData
    {
        public int id;
        public int num;
        public string head;
    }
}