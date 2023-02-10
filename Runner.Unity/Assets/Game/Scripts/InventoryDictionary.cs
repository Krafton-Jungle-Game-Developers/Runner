using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
        {
            throw new System.Exception(string.Format("There are {0} keys and {1} values after deserializationi. Make sure that both key and value types are serializable."));
        }
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
