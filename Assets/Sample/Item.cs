using UnityEngine;
using UnityEngine.UI;
using Jerry;

public class Item : LayoutItem
{
    private Transform head;
    private Text id;
    private Text num;

    private ItemData curData = null;
    private ItemDataChange dataChange = new ItemDataChange();

    void Awake()
    {
        head = this.transform.FindChild("head");
        id = this.transform.FindChild("id").GetComponent<Text>();
        num = this.transform.FindChild("num").GetComponent<Text>();
    }

    public override void TryRefreshUI(ILayoutItemData data)
    {
        curData = dataChange.Diff(curData, data as ItemData);

        if (dataChange.headChanged)
        {
            JerryUtil.CloneGo(new JerryUtil.CloneGoData()
            {
                active = true,
                clean = true,
                parant = head,
                prefab = Resources.Load<GameObject>(curData.head),
            });
        }

        id.text = curData.id.ToString();
        num.text = curData.num.ToString();
    }

    public class ItemData : ILayoutItemData
    {
        public int id;
        public int num;
        public string head;
    }

    public class ItemDataChange
    {
        public bool headChanged = false;
        public ItemData Diff(ItemData oriD, ItemData newD)
        {
            headChanged = (oriD == null || DiffUtil.DiffStr(oriD.head, newD.head));
            return newD;
        }
    }
}