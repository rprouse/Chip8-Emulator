using Chip8.Core;
using Chip8.SDL;

Console.Clear();

using (var console = new SdlConsole())
{
    if (!console.Init())
        return -1;
    console.Chip8Emulator = new Chip8Emulator(console);
    console.Chip8Emulator.LoadRom(@"..\..\..\..\roms\Chip8 picture.ch8");
    console.Chip8Emulator.Run();
}
return 0;

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