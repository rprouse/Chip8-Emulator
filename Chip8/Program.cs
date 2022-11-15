using System.Diagnostics;
using Chip8.Core;
using Silk.NET.Maths;
using Silk.NET.SDL;

Console.Clear();

const int PixelSize = 10;
const int ScreenWidth = Chip8Emulator.ScreenWidth * PixelSize;
const int ScreenHeight = Chip8Emulator.ScreenHeight * PixelSize;
const int InstructionsPerSecond = 700;

var chip8 = new Chip8Emulator();
chip8.LoadRom(@"..\..\..\..\roms\chip8-test-suite.ch8");

unsafe
{
    var sdl = Sdl.GetApi();
    if(sdl.Init(Sdl.InitVideo) < 0)
    {
        Console.WriteLine($"Error: Failed to initialize SDL. {sdl.GetErrorS()}");
    }

    Window* window = sdl.CreateWindow("Chip-8 Emulator",
        Sdl.WindowposCentered,
        Sdl.WindowposCentered,
        ScreenWidth,
        ScreenHeight,
        (uint)(WindowFlags.AllowHighdpi | WindowFlags.Shown));
    if (window == null)
    {
        Console.WriteLine($"Error: Failed to create SDL Windows.  {sdl.GetErrorS()}");
        return -2;
    }

    Surface* surface = sdl.GetWindowSurface(window);

    uint off = sdl.MapRGB(surface->Format, 0, 0, 0);
    uint on  = sdl.MapRGB(surface->Format, 0, 255, 0);

    sdl.FillRect(surface, null, off);
    sdl.UpdateWindowSurface(window);

    bool quit = false;
    byte? key = null;
    Event e = new Event();
    uint time = sdl.GetTicks(); // Time in ms since initialization
    while (!quit)
    {
        // Check for SDL events
        while (sdl.PollEvent(ref e) != 0)
        {
            switch ((EventType)e.Type)
            {
                case EventType.Quit:
                    quit = true;
                    break;
                case EventType.Keydown:
                    key = GetKeyPress(e);
                    break;
            }
        }

        // Slow down the Chip-8
        uint now = sdl.GetTicks();
        if (now - time > 1000 / InstructionsPerSecond)
        {
            time = now;

            // Step the Chip-8 emulator
            if (chip8.Step(key))
                Console.Beep();

            // Redraw screen if necessary
            if (chip8.RequiresRedraw)
                DrawScreen();

            key = null;
        }
    }

    sdl.FreeSurface(surface);
    sdl.DestroyWindow(window);
    sdl.Quit();
    return 0;

    void DrawScreen()
    {
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                DrawPixel(x, y, chip8.Screen[x, y]);
            }
        }
        sdl.UpdateWindowSurface(window);
    }

    void DrawPixel(int x, int y, bool pixelOn)
    {
        Rectangle<int> pixel = new Rectangle<int>(x * PixelSize, y * PixelSize, PixelSize, PixelSize);
        sdl.FillRect(surface, ref pixel, pixelOn ? on : off);
    }
}

static unsafe byte? GetKeyPress(Event e)
{
    byte? key = null;
    switch ((KeyCode)e.Key.Keysym.Sym)
    {
        case KeyCode.K1:
            key = 0x1;
            break;
        case KeyCode.K2:
            key = 0x2;
            break;
        case KeyCode.K3:
            key = 0x3;
            break;
        case KeyCode.K4:
            key = 0xC;
            break;
        case KeyCode.KQ:
            key = 0x4;
            break;
        case KeyCode.KW:
            key = 0x5;
            break;
        case KeyCode.KE:
            key = 0x6;
            break;
        case KeyCode.KR:
            key = 0xD;
            break;
        case KeyCode.KA:
            key = 0x7;
            break;
        case KeyCode.KS:
            key = 0x8;
            break;
        case KeyCode.KD:
            key = 0x9;
            break;
        case KeyCode.KF:
            key = 0xE;
            break;
        case KeyCode.KZ:
            key = 0xA;
            break;
        case KeyCode.KX:
            key = 0x0;
            break;
        case KeyCode.KC:
            key = 0xB;
            break;
        case KeyCode.KV:
            key = 0xF;
            break;
    }
    return key;
}

#if false
void DrawScreen(bool[,] screen)
{
    Console.WriteLine();
    Console.WriteLine("┌────────────────────────────────────────────────────────────────┐");
    for (int y = 0; y < 32; y++)
    {
        Console.Write('│');
        for (int x = 0; x < 64; x++)
        {
            Console.Write(screen[x, y] ? '█' : ' ');
        }
        Console.WriteLine('│');
    }
    Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
}
#endif