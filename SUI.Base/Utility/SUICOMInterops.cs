using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SUI.Base.Utility
{
    public class SUICOMInterops
    {
        #region interfaces
        [ComImport(), ComVisible(true),
        Guid("0000011B-0000-0000-C000-000000000046"),
        InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer
        {
            //IParseDisplayName
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ParseDisplayName(
                [In, MarshalAs(UnmanagedType.Interface)] object pbc,
                [In, MarshalAs(UnmanagedType.BStr)]      string pszDisplayName,
                [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten,
                [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);

            //IOleContainer
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnumObjects(
                [In, MarshalAs(UnmanagedType.U4)] tagOLECONTF grfFlags,
                out IEnumUnknown ppenum);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int LockContainer(
                [In, MarshalAs(UnmanagedType.Bool)] Boolean fLock);
        }

        [ComImport, ComVisible(true)]
        [Guid("00000100-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumUnknown
        {
            [PreserveSig]
            int Next(
                [In, MarshalAs(UnmanagedType.U4)] int celt,
                [Out, MarshalAs(UnmanagedType.IUnknown)] out object rgelt,
                [Out, MarshalAs(UnmanagedType.U4)] out int pceltFetched);
            [PreserveSig]
            int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);
            void Reset();
            void Clone(out IEnumUnknown ppenum);
        }
        #endregion

        #region enums
        public enum tagOLECONTF
        {
            OLECONTF_EMBEDDINGS = 1,
            OLECONTF_LINKS = 2,
            OLECONTF_OTHERS = 4,
            OLECONTF_ONLYUSER = 8,
            OLECONTF_ONLYIFRUNNING = 16
        }
        #endregion

        #region IIDs
        public static Guid IID_IHTMLBodyElement = new Guid("3050F1D8-98B5-11CF-BB82-00AA00BDCE0B");
        #endregion

        #region constants
        public static int S_OK = 0;
        #endregion

        #region Utility methods
        //We can compare the memory address of two IUnknown pointers to 
        // determine that whether two COM objects equal to each other.
        public static bool ComObjectsEquals(object obj1, object obj2)
        {
            int addressOfObj1 = -1, addressOfObj2 = -2;
            IntPtr ptr1 = new IntPtr(-1);
            IntPtr ptr2 = new IntPtr(-2);
            try
            {
                ptr1 = Marshal.GetIUnknownForObject(obj1);
                addressOfObj1 = ptr1.ToInt32();
            }
            catch
            {
                return false;
            }
            finally
            {
                Marshal.Release(ptr1);
            }
            try
            {
                ptr2 = Marshal.GetIUnknownForObject(obj2);
                addressOfObj2 = ptr2.ToInt32();
            }
            catch
            {
                return false;
            }
            finally
            {
                Marshal.Release(ptr2);
            }
            return (addressOfObj1 == addressOfObj2);
        }
        #endregion
    }
}
