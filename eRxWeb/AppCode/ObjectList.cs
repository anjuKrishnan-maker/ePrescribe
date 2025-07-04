using System;
using System.Collections;
using System.Data;

namespace eRxWeb
{
public class ObjectList<ItemType> : CollectionBase
{
    public int Add(ItemType value)
    {
        return InnerList.Add(value);
    }

    public void Remove(ItemType value)
    {
        InnerList.Remove(value);
    }

    public ItemType this[int index]
    {
        get
        {
            return (ItemType)InnerList[index];
        }
        set
        {
            InnerList[index] = value;
        }
    }
}
}