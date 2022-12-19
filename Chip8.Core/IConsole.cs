namespace Chip8.Core;

/// <summary>
/// Provides keyboard input and events to the emulator
/// </summary>
public interface IConsole
{
    /// <summary>
    /// Should we quit running?
    /// </summary>
    bool Quit { get; }

    /// <summary>
    /// Draws the screen
    /// </summary>
    /// <param name="screen"></param>
    void DrawScreen(bool[,] screen);

    /// <summary>
    /// Play a beep audio
    /// </summary>
    void Beep();

    /// <summary>
    /// Allows the calling code to run event handlers
    /// </summary>
    void ProcessEvents();
}
