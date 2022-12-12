namespace Chip8.Tests;

internal class NullConsole : IConsole
{
    public bool Quit => false;

    public byte? CurrentKey { get; private set; }

    public void Beep()
    {
    }

    public void DrawScreen(bool[,] screen)
    {
    }

    public void ProcessEvents()
    {
    }
}
