namespace Commands
{
    public interface IGameCommand
    {
        void Execute();
        void Undo();
    }
}
