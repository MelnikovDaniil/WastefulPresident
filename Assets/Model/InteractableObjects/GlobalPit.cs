using UnityEngine;

public class GlobalPit : MonoBehaviour
{
    public PitInteractableObject leftPit;
    public PitInteractableObject rightPit;

    private void Start()
    {
        leftPit.OnPitClosing += DisableGlobalPit;
        rightPit.OnPitClosing += DisableGlobalPit;

        leftPit.OnPitOpenning += EnableGlobalPit;
        rightPit.OnPitOpenning += EnableGlobalPit;
    }

    public void DisableGlobalPit()
    {
        leftPit.enabled = false;
        rightPit.enabled = false;
    }

    public void EnableGlobalPit()
    {
        leftPit.enabled = true;
        rightPit.enabled = true;
    }
}
