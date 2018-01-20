using System;
using System.Runtime.InteropServices;


namespace CodePanel
{
    public class PowerManager : IDisposable
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern ExecutionState SetThreadExecutionState(ExecutionState flags);

        private const uint ContinuousFlag = 0x80000000;

        public bool IsContinuous { get; private set; }
        public ExecutionState State { get; private set; }

        public PowerManager(ExecutionState state, bool isContinuous)
        {
            State = state;
            IsContinuous = isContinuous;

            if ((state & ExecutionState.AwaymodeRequired) != 0 && !isContinuous)
                throw new InvalidOperationException("Awaymode is only valid when the state is continuous");

            SetThreadExecutionState(isContinuous ? state | (ExecutionState)ContinuousFlag : state);
        }

        public void ResetTimer()
        {
            if (IsContinuous)
                throw new InvalidOperationException("The execution state is continuous");

            SetThreadExecutionState(State);
        }

        ~PowerManager()
        {
            ClearContinuousState();
        }

        public void Dispose()
        {
            ClearContinuousState();
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        private void ClearContinuousState()
        {
            if (!disposed)
            {
                if (IsContinuous)
                    SetThreadExecutionState((ExecutionState)ContinuousFlag);

                disposed = true;
            }
        }
    }
}
