using System.Diagnostics;

namespace Chip8.Core;

public class Chip8Emulator
{
    public const int ScreenWidth = 64;
    public const int ScreenHeight = 32;

    public const ushort FontMemory = 0x50;
    public const ushort ProgramMemory = 0x200;

    const int InstructionsPerSecond = 700;

    public byte[] Memory { get; } = new byte[0x1000];

    /// <summary>
    /// Program counter
    /// </summary>
    public ushort PC { get; set; } = ProgramMemory;

    /// <summary>
    /// Index register
    /// </summary>
    public ushort I { get; set; }

    /// <summary>
    /// Program stack
    /// </summary>
    public Stack<ushort> Stack { get; } = new Stack<ushort>();

    public byte DelayTimer { get; set; }

    public byte SoundTimer { get; set; }

    /// <summary>
    /// Registers V0 to VF
    /// </summary>
    public byte[] V { get; } = new byte[16];

    /// <summary>
    /// Easy access to VF
    /// </summary>
    public byte VF
    {
        get => V[0x0F];
        set => V[0x0F] = value;
    }

    /// <summary>
    /// Configuration options for the various ambiguous modes
    /// of operation
    /// </summary>
    public Config Config { get; } = new Config();

    public bool RequiresRedraw { get; private set; }

    /// <summary>
    /// Screen memory
    /// </summary>
    public bool[,] Screen { get; private set; } = new bool[ScreenWidth, ScreenHeight];

    readonly Instructions _instructions;

    readonly Stopwatch _stopwatch;

    readonly IConsole _console;

    public Chip8Emulator(IConsole console)
    {
        Array.Copy(Font.Default, 0, Memory, FontMemory, Font.Default.Length);
        _instructions = new Instructions(this);
        _stopwatch = Stopwatch.StartNew();
        _console = console;
    }

    public void LoadRom(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        byte[] rom = File.ReadAllBytes(filename);
        Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);
    }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        while (!_console.Quit)
        {
            _console.ProcessEvents();

            // Slow down the Chip-8
            if (stopwatch.ElapsedMilliseconds > 1000 / InstructionsPerSecond)
            {
                stopwatch.Restart();

                // SingleStep the Chip-8 emulator
                if (SingleStep(_console.CurrentKey))
                    _console.Beep();

                // Redraw screen if necessary
                if (RequiresRedraw)
                    _console.DrawScreen(Screen);
            }
        }
    }

    /// <summary>
    /// Executes one instruction
    /// </summary>
    /// <param name="key">Any key that is currently pressed</param>
    /// <returns>True if a beep should be played.</returns>
    public bool SingleStep(byte? key)
    {
        RequiresRedraw = false;
        var opcode = new OpCode((ushort)(Memory[PC++] << 8 | Memory[PC++]));
        _instructions.Execute(opcode, key);

        // Decrement timers. Technically, this should happen every 16.66 MS.
        if (_stopwatch.ElapsedMilliseconds > 16)
        {
            if (DelayTimer > 0) DelayTimer--;
            if (SoundTimer > 0) SoundTimer--;
            _stopwatch.Restart();
        }
        return SoundTimer > 0;
    }

    public void ClearScreen()
    {
        Screen = new bool[ScreenWidth, ScreenHeight];
        RequiresRedraw = true;
    }

    public bool GetPixel(int x, int y) => Screen[x, y];

    public void SetPixel(int x, int y, bool on)
    {
        Screen[x, y] = on;
        RequiresRedraw = true;
    }
}