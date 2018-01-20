using System;

namespace CodePanel
{
    [Flags]
    public enum ExecutionState : uint
    {
        DisplayRequired = 0x00000002,
        SystemRequired = 0x00000001,
        AwaymodeRequired = 0x00000040
    }
}