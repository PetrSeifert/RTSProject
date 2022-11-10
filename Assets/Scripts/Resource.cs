using System;

public enum ResourceType
{
    Food, Wood, Stone, Money
}

[Serializable] public class DictionaryAmountPerResource : SerializableDictionary<ResourceType, int> {}