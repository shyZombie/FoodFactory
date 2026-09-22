using System;
using System.Collections.Generic;

[Serializable]
public class DeliveryTrackerSaveData
{
    public List<DeliveryCategorySaveData> categoryCounts =
        new List<DeliveryCategorySaveData>();
}

[Serializable]
public class DeliveryCategorySaveData
{
    public FoodCategory category;
    public int count;
}
