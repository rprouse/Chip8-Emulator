namespace Chip8.Core;

public class Config
{
    /// <summary>
    /// Use the original Chip-8 behaviour for 8XY6 and 8XYE
    /// </summary>
    public bool OriginalShiftBehaviour { get; set; } = false;

    /// <summary>
    /// Use the original BNNN jump with offset behaviour. If set
    /// to false, uses the Super CHIP8 behaviour.
    /// </summary>
    public bool OriginalJumpOffsetBehaviour { get; set; } = true;

    /// <summary>
    /// Use the original behaviour for FX55 and FX65 where I 
    /// is incremented as the memory is copied
    /// </summary>
    public bool OriginalStoreLoadMemoryBehaviour { get; set; } = false;
}