using Chip8.Core;
using Silk.NET.Maths;
using Silk.NET.SDL;

namespace Chip8.SDL;

public unsafe class SdlConsole : IConsole, IDisposable
{
    const int PixelSize = 10;
    const int ScreenWidth = Chip8Emulator.ScreenWidth * PixelSize;
    const int ScreenHeight = Chip8Emulator.ScreenHeight * PixelSize;

    public bool Quit { get; private set; }

    public byte? CurrentKey { get; private set; }

    private Sdl sdl;
    private Window* window;
    private Surface* surface;
    private uint off;
    private uint on;

    public SdlConsole()
    {
        sdl = Sdl.GetApi();
    }

    public bool Init()
    {
        if (sdl.Init(Sdl.InitVideo) < 0)
        {
            Console.WriteLine($"Error: Failed to initialize SDL. {sdl.GetErrorS()}");
            return false;
        }

        window = sdl.CreateWindow("Chip-8 Emulator",
        Sdl.WindowposCentered,
        Sdl.WindowposCentered,
        ScreenWidth,
        ScreenHeight,
        (uint)(WindowFlags.AllowHighdpi | WindowFlags.Shown));
        if (window == null)
        {
            Console.WriteLine($"Error: Failed to create SDL Windows.  {sdl.GetErrorS()}");
            return false;
        }

        surface = sdl.GetWindowSurface(window);

        off = sdl.MapRGB(surface->Format, 0, 0, 0);
        on = sdl.MapRGB(surface->Format, 0, 255, 0);

        sdl.FillRect(surface, null, off);
        sdl.UpdateWindowSurface(window);

        return true;
    }

    public void Dispose()
    {
        sdl.FreeSurface(surface);
        sdl.DestroyWindow(window);
        sdl.Quit();
    }

    public void DrawScreen(bool[,] screen)
    {
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                DrawPixel(x, y, screen[x, y]);
            }
        }
        sdl.UpdateWindowSurface(window);
    }

    public void ProcessEvents()
    {
        // Check for SDL events
        CurrentKey = null;
        Event e = new Event();
        while (sdl.PollEvent(ref e) != 0)
        {
            switch ((EventType)e.Type)
            {
                case EventType.Quit:
                    Quit = true;
                    break;
                case EventType.Keydown:
                    CurrentKey = GetKeyPress(e);
                    break;
            }
        }
    }

    public void Beep()
    {
        Console.Beep();
    }

    void DrawPixel(int x, int y, bool pixelOn)
    {
        Rectangle<int> pixel = new Rectangle<int>(x * PixelSize, y * PixelSize, PixelSize, PixelSize);
        sdl.FillRect(surface, ref pixel, pixelOn ? on : off);
    }

    static byte? GetKeyPress(Event e)
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
}


#if false
string DebugScreen(bool[,] screen)
{
    var builder = new StringBuilder();
    builder.AppendLine();
    builder.AppendLine("┌────────────────────────────────────────────────────────────────┐");
    for (int y = 0; y < 32; y++)
    {
        builder.Append('│');
        for (int x = 0; x < 64; x++)
        {
            builder.Append(screen[x, y] ? '█' : ' ');
        }
        builder.Append('│');
        builder.AppendLine();
    }
    builder.AppendLine("└────────────────────────────────────────────────────────────────┘");
    return builder.ToString();
}
#endif