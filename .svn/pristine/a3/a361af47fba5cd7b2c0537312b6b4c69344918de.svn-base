using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Multimedia
{
    public sealed class Timer : IComponent, IDisposable
    {
        private static TimerCaps caps;
        private volatile bool disposed;
        private volatile TimerMode mode;
        private volatile int period;
        private volatile int resolution;
        private bool running;
        private ISite site;
        private ISynchronizeInvoke synchronizingObject;
        private EventRaiser tickRaiser;
        private TimeProc timeProcOneShot;
        private TimeProc timeProcPeriodic;
        private int timerID;
        private const int TIMERR_NOERROR = 0;

        public event EventHandler Disposed;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event EventHandler Tick;

        static Timer()
        {
            timeGetDevCaps(ref caps, Marshal.SizeOf(caps));
        }

        public Timer()
        {
            this.running = false;
            this.disposed = false;
            this.synchronizingObject = null;
            this.site = null;
            this.Initialize();
        }

        public Timer(IContainer container)
        {
            this.running = false;
            this.disposed = false;
            this.synchronizingObject = null;
            this.site = null;
            container.Add(this);
            this.Initialize();
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.IsRunning)
                {
                    this.Stop();
                }
                this.disposed = true;
                this.OnDisposed(EventArgs.Empty);
            }
        }

        ~Timer()
        {
            if (this.IsRunning)
            {
                timeKillEvent(this.timerID);
            }
        }

        private void Initialize()
        {
            this.mode = TimerMode.Periodic;
            this.period = Capabilities.periodMin;
            this.resolution = 1;
            this.running = false;
            this.timeProcPeriodic = new TimeProc(this.TimerPeriodicEventCallback);
            this.timeProcOneShot = new TimeProc(this.TimerOneShotEventCallback);
            this.tickRaiser = new EventRaiser(this.OnTick);
        }

        private void OnDisposed(EventArgs e)
        {
            EventHandler disposed = this.Disposed;
            if (disposed != null)
            {
                disposed(this, e);
            }
        }

        private void OnStarted(EventArgs e)
        {
            EventHandler started = this.Started;
            if (started != null)
            {
                started(this, e);
            }
        }

        private void OnStopped(EventArgs e)
        {
            EventHandler stopped = this.Stopped;
            if (stopped != null)
            {
                stopped(this, e);
            }
        }

        private void OnTick(EventArgs e)
        {
            EventHandler tick = this.Tick;
            if (tick != null)
            {
                tick(this, e);
            }
        }

        public void Start()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Timer");
            }
            if (!this.IsRunning)
            {
                if (this.Mode == TimerMode.Periodic)
                {
                    this.timerID = timeSetEvent(this.Period, this.Resolution, this.timeProcPeriodic, 0, (int)this.Mode);
                }
                else
                {
                    this.timerID = timeSetEvent(this.Period, this.Resolution, this.timeProcOneShot, 0, (int)this.Mode);
                }
                if (this.timerID == 0)
                {
                    throw new TimerStartException("Unable to start multimedia Timer.");
                }
                this.running = true;
                if ((this.SynchronizingObject != null) && this.SynchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(new EventRaiser(this.OnStarted), new object[] { EventArgs.Empty });
                }
                else
                {
                    this.OnStarted(EventArgs.Empty);
                }
            }
        }

        public void Stop()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Timer");
            }
            if (this.running)
            {
                Debug.Assert(timeKillEvent(this.timerID) == 0);
                this.running = false;
                if ((this.SynchronizingObject != null) && this.SynchronizingObject.InvokeRequired)
                {
                    this.SynchronizingObject.BeginInvoke(new EventRaiser(this.OnStopped), new object[] { EventArgs.Empty });
                }
                else
                {
                    this.OnStopped(EventArgs.Empty);
                }
            }
        }

        [DllImport("winmm.dll")]
        private static extern int timeGetDevCaps(ref TimerCaps caps, int sizeOfTimerCaps);
        [DllImport("winmm.dll")]
        private static extern int timeKillEvent(int id);
        private void TimerOneShotEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if (this.synchronizingObject != null)
            {
                this.synchronizingObject.BeginInvoke(this.tickRaiser, new object[] { EventArgs.Empty });
                this.Stop();
            }
            else
            {
                this.OnTick(EventArgs.Empty);
                this.Stop();
            }
        }

        private void TimerPeriodicEventCallback(int id, int msg, int user, int param1, int param2)
        {
            if (this.synchronizingObject != null)
            {
                this.synchronizingObject.BeginInvoke(this.tickRaiser, new object[] { EventArgs.Empty });
            }
            else
            {
                this.OnTick(EventArgs.Empty);
            }
        }

        [DllImport("winmm.dll")]
        private static extern int timeSetEvent(int delay, int resolution, TimeProc proc, int user, int mode);

        public static TimerCaps Capabilities
        {
            get
            {
                return caps;
            }
        }

        public bool IsRunning
        {
            get
            {
                return this.running;
            }
        }

        public TimerMode Mode
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                return this.mode;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                this.mode = value;
                if (this.IsRunning)
                {
                    this.Stop();
                    this.Start();
                }
            }
        }

        public int Period
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                return this.period;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                if ((value < Capabilities.periodMin) || (value > Capabilities.periodMax))
                {
                    throw new ArgumentOutOfRangeException("Period", value, "Multimedia Timer period out of range.");
                }
                this.period = value;
                if (this.IsRunning)
                {
                    this.Stop();
                    this.Start();
                }
            }
        }

        public int Resolution
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                return this.resolution;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Resolution", value, "Multimedia timer resolution out of range.");
                }
                this.resolution = value;
                if (this.IsRunning)
                {
                    this.Stop();
                    this.Start();
                }
            }
        }

        public ISite Site
        {
            get
            {
                return this.site;
            }
            set
            {
                this.site = value;
            }
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                return this.synchronizingObject;
            }
            set
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Timer");
                }
                this.synchronizingObject = value;
            }
        }

        private delegate void EventRaiser(EventArgs e);

        private delegate void TimeProc(int id, int msg, int user, int param1, int param2);
    }

    public enum TimeType
    {
        Bytes = 4,
        Midi = 0x10,
        Milliseconds = 1,
        Samples = 2,
        Smpte = 8,
        Ticks = 0x20
    }

    public enum TimerMode
    {
        OneShot = 0,
        Periodic = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TimerCaps
    {
        public int periodMin;
        public int periodMax;
    }

    public enum Note
    {
        A = 9,
        AFlat = 8,
        ASharp = 10,
        B = 11,
        BFlat = 10,
        C = 0,
        CSharp = 1,
        D = 2,
        DFlat = 1,
        DSharp = 3,
        E = 4,
        EFlat = 3,
        F = 5,
        FSharp = 6,
        G = 7,
        GFlat = 6,
        GSharp = 8
    }

    public enum Key
    {
        AFlatMinor,
        EFlatMinor,
        BFlatMinor,
        FMinor,
        CMinor,
        GMinor,
        DMinor,
        AMinor,
        EMinor,
        BMinor,
        FSharpMinor,
        CSharpMinor,
        GSharpMinor,
        DSharpMinor,
        ASharpMinor,
        CFlatMajor,
        GFlatMajor,
        DFlatMajor,
        AFlatMajor,
        EFlatMajor,
        BFlatMajor,
        FMajor,
        CMajor,
        GMajor,
        DMajor,
        AMajor,
        EMajor,
        BMajor,
        FSharpMajor,
        CSharpMajor
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Time
    {
        [FieldOffset(4)]
        public int byteCount;
        [FieldOffset(9)]
        public byte dummy;
        [FieldOffset(7)]
        public byte frames;
        [FieldOffset(8)]
        public byte framesPerSecond;
        [FieldOffset(4)]
        public byte hours;
        [FieldOffset(4)]
        public int milliseconds;
        [FieldOffset(5)]
        public byte minutes;
        [FieldOffset(10)]
        public byte pad1;
        [FieldOffset(11)]
        public byte pad2;
        [FieldOffset(4)]
        public int samples;
        [FieldOffset(6)]
        public byte seconds;
        [FieldOffset(4)]
        public int songPositionPointer;
        [FieldOffset(4)]
        public int ticks;
        [FieldOffset(0)]
        public int type;
    }

    public interface IDevice : IDisposable
    {
        event EventHandler<BufferFinishedEventArgs> BufferFinished;

        void Close();
        void Reset();

        int Handle { get; }

        ISynchronizeInvoke SynchronizingObject { get; set; }
    }

    public class BufferFinishedEventArgs : EventArgs
    {
        private byte[] buffer;

        public BufferFinishedEventArgs(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public byte[] GetBuffer()
        {
            return this.buffer;
        }
    }

    public class TimerStartException : ApplicationException
    {
        public TimerStartException(string message)
            : base(message)
        {
        }
    }
}