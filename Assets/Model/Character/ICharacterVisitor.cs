using System;

public interface ICharacterVisitor
{
    void VisitLever();

    void VisitElectricPanel();
    void ElectricPanelDeath();

    void VisitPit(Action onPitFalling = null);
    void FinishVisitPit(Action onPitFalling = null);

    void VisitTimer(float timeSpeed);
    void FinishVisitTimer();

    Item GetItem();
    void StartTakingItem(Item item);
    void PutItem();
    bool TryTakeItem(Item item);
    void RemoveItem();

    BoxedObject GetBoxedObject();

    void FinishVisiting();
}
