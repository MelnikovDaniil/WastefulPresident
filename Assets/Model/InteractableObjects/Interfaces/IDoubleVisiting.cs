public interface IDoubleVisiting
{
    bool IsDoubleVisiting(ICharacterVisitor visitor);

    void DoubleVisit(ICharacterVisitor visitor);
}
