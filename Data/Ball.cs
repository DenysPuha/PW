//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
            ThreadStart ts = new ThreadStart(MoveLoop);
            _thread = new System.Threading.Thread(ts);
            _thread.IsBackground = true;
    }

    #endregion ctor

    #region IBall

        public void Start()
        {
             _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _thread.Join();
        }


    public event EventHandler<IVector>? NewPositionNotification;

        public void SetVelocity(double x, double y)
        {
            lock(_lockVelocity)
            {
                Velocity = new Vector(x, y);
            }
        }

        public IVector PositionValue
        {
            get
            {
                lock (_lockPosition)
                {
                    return Position;
                }
            }
        }

        public IVector Velocity { get; set; }

        #endregion IBall

        #region private

        private readonly object _lock = new();
        private readonly object _lockVelocity = new();
        private readonly object _lockPosition = new();

        private Thread _thread;
        private volatile bool _running = true;

        private Vector Position;
        private int refreshTime;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

        private void MoveLoop()
        {
            Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            long previous = stopwatch.ElapsedMilliseconds;
            while (_running)
            {
                long current = stopwatch.ElapsedMilliseconds;
                refreshTime = (int)(current - previous);
                previous = current;
                Move(new Vector(Velocity.x * refreshTime / 1000, Velocity.y * refreshTime/1000));
            }
            stopwatch.Stop();
        }
        private void Move(Vector delta)
        {
            lock (_lock)
            {
                Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            }
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}