﻿public interface IVisitor
{
    void VisitLever();

    void VisitElectricPanel();
    void ElectricPanelDeath();

    void VisitPit();
    void FinishVisitPit();

    void VisitTimer(float timeSpeed);
    void FinishVisitTimer();

    void FinishVisiting();
}
