using Chip8.Core;
using Silk.NET.Maths;
using Silk.NET.SDL;

Console.Clear();

const int PixelSize = 10;
const int ScreenWidth = Chip8Emulator.ScreenWidth * PixelSize;
const int ScreenHeight = Chip8Emulator.ScreenHeight * PixelSize;

var chip8 = new Chip8Emulator();
chip8.LoadRom(@"..\..\..\..\roms\IBM Logo.ch8");

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
    Event e = new Event();
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
                    switch ((KeyCode)e.Key.Keysym.Sym)
                    {
                        case KeyCode.KQ:
                            quit = true;
                            break;
                    }
                    break;
            }
        }

        // Step the Chip-8 emulator
        chip8.Step();

        // Redraw if necessary
        if (chip8.RequiresRedraw)
            DrawScreen();
    }

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