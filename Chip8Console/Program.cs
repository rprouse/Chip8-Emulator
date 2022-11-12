using Chip8;

Console.Clear();

var chip8 = new Chip8Emulator(DrawScreen);
chip8.LoadRom(@"..\..\..\..\roms\IBM Logo.ch8");
chip8.Run();

void DrawScreen(bool[,] screen)
{
    Console.WriteLine();
    Console.WriteLine("__________________________________________________________________");
    for(int y = 0; y < 32; y++)
    {
        Console.Write('|');
        for(int x = 0; x < 64; x++)
        {
            Console.Write(screen[x,y] ? '#' : '.');
        }
        Console.WriteLine('|');
    }
    Console.WriteLine("------------------------------------------------------------------");
}