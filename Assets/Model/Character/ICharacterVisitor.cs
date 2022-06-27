using UnityEngine;

public interface ICharacterVisitor
{
    void VisitLever();

    void VisitElectricPanel();
    void ElectricPanelDeath();

    void VisitPit();
    void FinishVisitPit();

    void VisitTimer(float timeSpeed);
    void FinishVisitTimer();

    Battery GetBattery();
    void StartTakingBattery(Battery battery);
    void PutBattery();
    bool TryTakeBattery(Battery battery);
    void RemoveBattery();

    void Teleport(Vector3 position, Vector3 direction);

    void FinishVisiting();
}
