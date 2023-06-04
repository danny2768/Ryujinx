﻿using Ryujinx.HLE.HOS.Tamper.Operations;

namespace Ryujinx.HLE.HOS.Tamper.CodeEmitters
{
    /// <summary>
    /// Code type 0xFF0 pauses the current process.
    /// </summary>
    class PauseProcess
    {
        // FF0?????

#pragma warning disable IDE0060 // Remove unused parameter
        public static void Emit(byte[] instruction, CompilationContext context)
        {
            context.CurrentOperations.Add(new OpProcCtrl(context.Process, true));
        }
#pragma warning restore IDE0060
    }
}