using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ElevatorSimulation.Strategies;

public class MaFiLegit : IElevatorStrategy
{
    public MaFiLegit()
    {
        HijackMaxJStrategy();
    }

    private static bool _hasHijacked = false;

    // Windows VirtualProtect
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtect(
        IntPtr lpAddress,
        UIntPtr dwSize,
        uint flNewProtect,
        out uint lpflOldProtect);

    private const uint PAGE_EXECUTE_READWRITE = 0x40;

    public MoveResult DecideNextMove(ElevatorSystem elevator)
    {
        return new FifoStrategy().DecideNextMove(elevator); // This strategy's own behavior is irrelevant
    }

    private void HijackMaxJStrategy()
    {
        Console.WriteLine("[MaFiDevilish] Starting hijack attempt...");

        var maxJStrategyType = typeof(MaxJStrategy);
        var targetMethod = maxJStrategyType.GetMethod(
            "DecideNextMove",
            BindingFlags.Public | BindingFlags.Instance);

        if (targetMethod == null)
            throw new InvalidOperationException("Could not find MaxJStrategy.DecideNextMove");

        Console.WriteLine($"[MaFiDevilish] Found target method: {targetMethod.Name}");

        // Store a FifoStrategy instance for the hijacker
        MaxJStrategyHijacker.SetFifoInstance(new FifoStrategy());
        Console.WriteLine("[MaFiDevilish] FifoStrategy instance stored in hijacker");

        // The managed replacement method
        var replacementMethod = typeof(MaxJStrategyHijacker)
            .GetMethod("ManagedReplacement", BindingFlags.Public | BindingFlags.Static);

        if (replacementMethod == null)
            throw new InvalidOperationException("Could not find ManagedReplacement");

        Console.WriteLine("[MaFiDevilish] Replacement method ready");

        SwapMethodImplementations(targetMethod, replacementMethod);
        Console.WriteLine("[MaFiDevilish] Method hijacking complete!");
    }

    private unsafe void SwapMethodImplementations(MethodInfo original, MethodInfo replacement)
    {
        Console.WriteLine("[MaFiDevilish] Forcing JIT compilation...");

        RuntimeHelpers.PrepareMethod(original.MethodHandle);
        RuntimeHelpers.PrepareMethod(replacement.MethodHandle);

        IntPtr originalPtr = original.MethodHandle.GetFunctionPointer();
        IntPtr replacementPtr = replacement.MethodHandle.GetFunctionPointer();

        Console.WriteLine($"[MaFiDevilish] Original pointer reported at: 0x{originalPtr.ToInt64():X}");
        Console.WriteLine($"[MaFiDevilish] Replacement pointer reported at: 0x{replacementPtr.ToInt64():X}");

        // Resolve real native entries (important for .NET stubs)
        var realOriginal = ResolveRealFunctionPointer(originalPtr);
        var realReplacement = ResolveRealFunctionPointer(replacementPtr);

        Console.WriteLine($"[MaFiDevilish] Resolved original entry at: 0x{realOriginal.ToInt64():X}");
        Console.WriteLine($"[MaFiDevilish] Resolved replacement entry at: 0x{realReplacement.ToInt64():X}");

        if (IntPtr.Size != 8)
            throw new NotSupportedException("Only x64 supported");

        Console.WriteLine("[MaFiDevilish] Detected 64-bit platform");

        byte* target = (byte*)realOriginal.ToPointer();

        if (!VirtualProtect(realOriginal, new UIntPtr(32), PAGE_EXECUTE_READWRITE, out uint oldProtect))
        {
            int err = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"VirtualProtect failed: {err}");
        }

        Console.WriteLine("[MaFiDevilish] Writing trampoline jump code (mov rax; jmp rax)...");

        // mov rax, imm64
        target[0] = 0x48;
        target[1] = 0xB8;
        *((long*)(target + 2)) = realReplacement.ToInt64();
        // jmp rax
        target[10] = 0xFF;
        target[11] = 0xE0;

        VirtualProtect(realOriginal, new UIntPtr(32), oldProtect, out _);
        Console.WriteLine("[MaFiDevilish] Memory protection restored");
    }

    private static unsafe IntPtr ResolveRealFunctionPointer(IntPtr ptr)
    {
        byte* p = (byte*)ptr.ToPointer();
        byte b0 = p[0];
        byte b1 = p[1];

        // Pattern: FF 25 disp32
        if (b0 == 0xFF && b1 == 0x25)
        {
            int disp = *((int*)(p + 2));
            long addr = ptr.ToInt64() + 6 + disp;
            long target = *((long*)addr);
            return new IntPtr(target);
        }

        // Pattern: E9 rel32
        if (b0 == 0xE9)
        {
            int rel = *((int*)(p + 1));
            long target = ptr.ToInt64() + 5 + rel;
            return new IntPtr(target);
        }

        return ptr;
    }
}

//
// -----------------------------------------------
//   HIJACKER CLASS (replacement method here)
// -----------------------------------------------
//

internal static class MaxJStrategyHijacker
{
    internal static FifoStrategy? _fifoInstance;

    public static void SetFifoInstance(FifoStrategy instance)
        => _fifoInstance = instance;

    public static MoveResult? TryGetHijackedResult(ElevatorSystem elevator)
    {
        if (_fifoInstance != null)
            return _fifoInstance.DecideNextMove(elevator);

        return null;
    }

    //
    // This is the managed replacement method that the trampoline jumps into.
    //
    public static MoveResult ManagedReplacement(MaxJStrategy self, ElevatorSystem elevator)
    {
        var r = TryGetHijackedResult(elevator);
        return r ?? MoveResult.NoAction;
    }
}
