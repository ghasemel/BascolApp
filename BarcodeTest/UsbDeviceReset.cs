using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Mali.MaliControls
{
    /// <summary>
    /// Helper class to reset USB COM port devices (like CH340)
    /// This mimics what happens when you change settings in Device Manager
    /// </summary>
    public static class UsbDeviceReset
    {
        // SetupAPI constants
        private const int DIGCF_PRESENT = 0x02;
        private const int DIGCF_DEVICEINTERFACE = 0x10;
        private const int DICS_FLAG_GLOBAL = 0x01;
        private const int DIREG_DEV = 0x01;
        private const int KEY_READ = 0x20019;

        private static readonly Guid GUID_DEVCLASS_PORTS = new Guid("4D36E978-E325-11CE-BFC1-08002BE10318");

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(
            ref Guid ClassGuid,
            IntPtr Enumerator,
            IntPtr hwndParent,
            int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(
            IntPtr DeviceInfoSet,
            int MemberIndex,
            ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiOpenDevRegKey(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            int Scope,
            int HwProfile,
            int KeyType,
            int samDesired);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(IntPtr hKey);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiChangeState(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiSetClassInstallParams(
            IntPtr DeviceInfoSet,
            ref SP_DEVINFO_DATA DeviceInfoData,
            ref SP_PROPCHANGE_PARAMS ClassInstallParams,
            int ClassInstallParamsSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader;
            public int StateChange;
            public int Scope;
            public int HwProfile;
        }

        private const int DIF_PROPERTYCHANGE = 0x12;
        private const int DICS_PROPCHANGE = 0x03;

        /// <summary>
        /// Triggers a property change on the COM port device, similar to changing settings in Device Manager
        /// This forces the driver to reinitialize
        /// </summary>
        public static bool ResetComPort(string portName)
        {
            IntPtr deviceInfoSet = IntPtr.Zero;

            try
            {
                Guid classGuid = GUID_DEVCLASS_PORTS;
                deviceInfoSet = SetupDiGetClassDevs(ref classGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT);

                if (deviceInfoSet == IntPtr.Zero || deviceInfoSet == new IntPtr(-1))
                    return false;

                SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                deviceInfoData.cbSize = Marshal.SizeOf(deviceInfoData);

                int index = 0;
                while (SetupDiEnumDeviceInfo(deviceInfoSet, index, ref deviceInfoData))
                {
                    IntPtr hKey = SetupDiOpenDevRegKey(deviceInfoSet, ref deviceInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
                    if (hKey != IntPtr.Zero && hKey != new IntPtr(-1))
                    {
                        try
                        {
                            using (var key = RegistryKey.FromHandle(new Microsoft.Win32.SafeHandles.SafeRegistryHandle(hKey, false)))
                            {
                                string devicePortName = key.GetValue("PortName") as string;
                                if (devicePortName != null && devicePortName.Equals(portName, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Found the device, trigger property change
                                    SP_PROPCHANGE_PARAMS propChangeParams = new SP_PROPCHANGE_PARAMS();
                                    propChangeParams.ClassInstallHeader.cbSize = Marshal.SizeOf(typeof(SP_CLASSINSTALL_HEADER));
                                    propChangeParams.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
                                    propChangeParams.StateChange = DICS_PROPCHANGE;
                                    propChangeParams.Scope = DICS_FLAG_GLOBAL;
                                    propChangeParams.HwProfile = 0;

                                    if (SetupDiSetClassInstallParams(deviceInfoSet, ref deviceInfoData, ref propChangeParams, Marshal.SizeOf(propChangeParams)))
                                    {
                                        if (SetupDiChangeState(deviceInfoSet, ref deviceInfoData))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            RegCloseKey(hKey);
                        }
                    }
                    index++;
                }
            }
            catch (Exception)
            {
                // Ignore errors
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero && deviceInfoSet != new IntPtr(-1))
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return false;
        }
    }
}
