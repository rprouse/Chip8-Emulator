using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Chip8.Core;

public class Chip8Emulator
{
    public const int ScreenWidth = 64;
    public const int ScreenHeight = 32;

    public const ushort FontMemory = 0x50;
    public const ushort ProgramMemory = 0x200;

    const int InstructionsPerSecond = 700;

    public byte[] Memory { get; private set; }

    /// <summary>
    /// Program counter
    /// </summary>
    public ushort PC { get; set; }

    /// <summary>
    /// Index register
    /// </summary>
    public ushort I { get; set; }

    /// <summary>
    /// Program stack
    /// </summary>
    public Stack<ushort> Stack { get; private set; }

    public byte DelayTimer { get; set; }

    public byte SoundTimer { get; set; }

    /// <summary>
    /// Registers V0 to VF
    /// </summary>
    public byte[] V { get; private set; }

    /// <summary>
    /// Easy access to VF
    /// </summary>
    public byte VF
    {
        get => V[0x0F];
        set => V[0x0F] = value;
    }

    public override string ToString() =>
        $"PC: 0x{PC:X3}\tV0: 0x{V[0]:X2}  V4: 0x{V[4]:X2}  V8: 0x{V[8]:X2}  VC: 0x{V[0xC]:X2}\r\n" +
        $" I: 0x{I:X4}\tV1: 0x{V[1]:X2}  V5: 0x{V[5]:X2}  V9: 0x{V[9]:X2}  VD: 0x{V[0xD]:X2}\r\n" +
        $"DT: 0x{DelayTimer:X2}\tV2: 0x{V[2]:X2}  V6: 0x{V[6]:X2}  VA: 0x{V[0xA]:X2}  VE: 0x{V[0xD]:X2}\r\n" +
        $"ST: 0x{SoundTimer:X2}\tV3: 0x{V[3]:X2}  V7: 0x{V[7]:X2}  VB: 0x{V[0xB]:X2}  VF: 0x{V[0xF]:X2}";

    /// <summary>
    /// Configuration options for the various ambiguous modes
    /// of operation
    /// </summary>
    public Config Config { get; } = new Config();

    public bool RequiresRedraw { get; private set; }

    /// <summary>
    /// Screen memory
    /// </summary>
    public bool[,] Screen { get; private set; }

    private bool[] _keys;

    readonly Instructions _instructions;

    readonly Stopwatch _stopwatch;

    readonly IConsole _console;

    public Chip8Emulator(IConsole console)
    {
        Init();
        _instructions = new Instructions(this);
        _stopwatch = Stopwatch.StartNew();
        _console = console;
    }

    [MemberNotNull(nameof(Memory))]
    [MemberNotNull(nameof(Stack))]
    [MemberNotNull(nameof(V))]
    [MemberNotNull(nameof(Screen))]
    private void Init()
    {
        Memory = new byte[0x1000];
        Array.Copy(Font.Default, 0, Memory, FontMemory, Font.Default.Length);
        PC = ProgramMemory;
        I = 0;
        Stack = new Stack<ushort>();
        DelayTimer = 0;
        SoundTimer = 0;
        V = new byte[16];
        RequiresRedraw = false;
        Screen = new bool[ScreenWidth, ScreenHeight];
        _keys = new bool[0x10];
    }

    public byte[] LoadRom(string filename)
    {
        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        Init();

        byte[] rom = File.ReadAllBytes(filename);
        Array.Copy(rom, 0, Memory, ProgramMemory, rom.Length);

        return rom;
    }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        while (!_console.Quit)
        {
            _console.ProcessEvents();

            // Slow down the Chip-8
            if (stopwatch.ElapsedTicks > 1000 / InstructionsPerSecond)
            {
                stopwatch.Restart();

                // SingleStep the Chip-8 emulator
                if (SingleStep())
                    _console.Beep();

                // Redraw screen if necessary
                if (RequiresRedraw)
                    _console.DrawScreen(Screen);

                Thread.Sleep(0);    // Give up control
            }
        }
    }

    /// <summary>
    /// Executes one instruction
    /// </summary>
    /// <returns>True if a beep should be played.</returns>
    public bool SingleStep()
    {
        RequiresRedraw = false;
        var opcode = new OpCode((ushort)(Memory[PC++] << 8 | Memory[PC++]));
        _instructions.Execute(opcode, _keys);

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

    public void SetKey(byte key)
    {
        if (key > 0x0f) return;
        _keys[key] = true;
    }

    public void UnsetKey(byte key)
    {
        if (key > 0x0f) return;
        _keys[key] = false;
    }
}