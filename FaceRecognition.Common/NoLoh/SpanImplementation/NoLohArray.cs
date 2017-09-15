using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Conditions;

namespace FaceRecognition.Common.NoLoh.SpanImplementation
{
    public class NoLohArray<T> : IDisposable
        where T : struct
    {
        private bool _disposed;
        private readonly int _size;
        private int _length;
        private readonly IntPtr _array;
        private readonly bool _gcPressure;

        public NoLohArray(int length, bool gcPressure = false)
        {
            Condition.Requires(length, nameof(length)).IsGreaterThan(0);

            _size = Marshal.SizeOf<T>();
            _length = length;
            _array = Win32.VirtualAlloc(IntPtr.Zero, (IntPtr)(_length * _size), Win32.MEM_RESERVE | Win32.MEM_COMMIT, Win32.PAGE_READWRITE);
            if (_array == IntPtr.Zero)
            {
                throw new Exception("Allocation request failed.", new Win32Exception(Marshal.GetLastWin32Error()));
            }
            _gcPressure = gcPressure;
            if (_gcPressure)
            {
                GC.AddMemoryPressure(_length * _size);
            }
        }

        public T this[int index]
        {
            get
            {
                Condition.Requires(index, nameof(index)).IsInRange(0, _length - 1);

                var start = index * _size;
                return Marshal.PtrToStructure<T>(_array + start);
            }
            set
            {
                Condition.Requires(index, nameof(index)).IsInRange(0, _length - 1);

                var start = index * _size;
                Marshal.StructureToPtr(value, _array + start, true);
            }
        }

        /// <summary>
        /// Releases all unmanaged memory backing this array.
        /// </summary>
        private void Free()
        {
            if (_array != IntPtr.Zero && !Win32.VirtualFree(_array, IntPtr.Zero, Win32.MEM_RELEASE))
                if (!Environment.HasShutdownStarted)
                    throw new Exception("Free allocation failed.", new Win32Exception(Marshal.GetLastWin32Error()));

            if (_gcPressure) GC.RemoveMemoryPressure(_length * _size);

            _length = 0;
        }


        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~NoLohArray()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //managed here...
                }
                //unmanaged here...
                Free();
            }
            this._disposed = true;
        }

        #endregion
    }
}