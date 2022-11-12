using Chip8;

Console.Clear();

var chip8 = new Chip8Emulator(DrawScreen);
chip8.LoadRom(@"..\..\..\..\roms\IBM Logo.ch8");
chip8.Run();

void DrawScreen(bool[] display)
{
    Console.SetCursorPosition(0, 0);
    for(int y = 0; y < 32; y++)
    {
        for(int x = 0; x < 64; x++)
        {
            Console.Write(display[y * 32 + x] ? '#' : ' ');
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}