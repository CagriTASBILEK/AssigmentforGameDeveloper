public class CommandInvoker
{
    private IGameCommand[] commandHistory;
    private int currentIndex;
    private const int MAX_COMMANDS = 100;

    public CommandInvoker()
    {
        commandHistory = new IGameCommand[MAX_COMMANDS];
        currentIndex = 0;
    }

    public void ExecuteCommand(IGameCommand command)
    {
        command.Execute();
        commandHistory[currentIndex] = command;
        currentIndex = (currentIndex + 1) % MAX_COMMANDS;
    }

    public void UndoLastCommand()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            IGameCommand command = commandHistory[currentIndex];
            if (command != null)
            {
                command.Undo();
                commandHistory[currentIndex] = null;
            }
        }
    }

    public void ClearHistory()
    {
        for (int i = 0; i < MAX_COMMANDS; i++)
        {
            commandHistory[i] = null;
        }
        currentIndex = 0;
    }
}
