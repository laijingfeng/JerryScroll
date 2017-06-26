using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public GameObject prefab;
    public InfinitelyLinearGridLayoutGroup layoutH;
    public InfinitelyLinearGridLayoutGroup layoutV;

    private List<Item.ItemData> datas = new List<Item.ItemData>();
    private int dataAmt = 20;

    void Awake()
    {
        GenDatas();
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