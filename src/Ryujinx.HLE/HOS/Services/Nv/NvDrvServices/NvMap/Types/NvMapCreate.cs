using System.Runtime.InteropServices;

namespace Ryujinx.HLE.HOS.Services.Nv.NvDrvServices.NvMap.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct NvMapCreate
    {
        public int Size;
        public int Handle;
    }
}