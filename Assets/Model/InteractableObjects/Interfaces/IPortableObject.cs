using UnityEngine;

public interface IPortableObject
{
    bool IsSmallTeleport { get; }
    void Teleport(Vector3 position, Quaternion rotationDifference);

    void AfterTeleport(Vector2 direction) { }
}
