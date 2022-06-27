using UnityEngine;

public interface IPortalVisitor
{
    void Teleport(Vector3 position, Vector3 direction);
}
