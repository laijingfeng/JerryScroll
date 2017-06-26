using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public GameObject prefab;
    public Transform layoutHContent;
    public Transform layoutVContent;

    private List<Item.ItemData> datas = new List<Item.ItemData>();
    private int dataAmt = 20;

    private Layout layoutH;
    private Layout layoutV;

    void Awake()
    {
        GenDatas();
        layoutH = layoutHContent.gameObject.AddComponent<Layout>();
        layoutV = layoutVContent.gameObject.AddComponent<Layout>();
        
        layoutH.DoInit(new Layout.ConfigData()
        {
            startIdx = 0,
            bufferHalfAmt = 1,
            cellSize = new Vector2(190, 190),
            dir = Layout.Dir.Horizontal,
            oneScreenAmt = 3,
            prefab = prefab.transform,
            spacing = 10,
        }, datas);

        layoutV.DoInit(new Layout.ConfigData()
        {
            startIdx = 0,
            bufferHalfAmt = 1,
            cellSize = new Vector2(190, 190),
            dir = Layout.Dir.Vertical,
            oneScreenAmt = 3,
            prefab = prefab.transform,
            spacing = 10,
        }, datas);
    }

    private void GenDatas()
    {
        datas.Clear();
        for (int i = 0; i < dataAmt; i++)
        {
            datas.Add(RandomOneData());
        }
    }

    private int idKey = 0;
    private int GenId()
    {
        idKey++;
        return idKey;
    }

    private Item.ItemData RandomOneData()
    {
        Item.ItemData ret = new Item.ItemData();
        ret.id = GenId();
        ret.num = Random.Range(0, 1000);
        ret.head = string.Format("head{0}", Random.Range(0, 3));
        return ret;
    }
}