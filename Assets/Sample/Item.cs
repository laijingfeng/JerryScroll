using UnityEngine;
using UnityEngine.UI;
using Jerry;

public class Item : LayoutItem
{
    private Transform head;
    private Text id;
    private Text num;

    private ItemData curData = null;

    void Awake()
    {
        head = this.transform.FindChild("head");
        id = this.transform.FindChild("id").GetComponent<Text>();
        num = this.transform.FindChild("id").GetComponent<Text>();
    }

    public override void TryRefreshUI(ILayoutItemData data)
    {
        curData = data as ItemData;

        JerryUtil.CloneGo(new JerryUtil.CloneGoData()
        {
            active = true,
            clean = true,
            parant = head,
            prefab = Resources.Load<GameObject>(curData.head),
        });

        id.text = curData.id.ToString();
        num.text = curData.num.ToString();
    }

    public class ItemData : ILayoutItemData
    {
        public int id;
        public int num;
        public string head;
    }
}