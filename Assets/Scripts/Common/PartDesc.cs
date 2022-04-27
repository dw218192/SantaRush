using System;

[Serializable]
public struct PartDesc<T> where T : SpriteObject
{
    public T prefab;
    public uint count;
}