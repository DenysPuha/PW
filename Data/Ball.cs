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

    internal Ball(Vector initialPosition, Vector initialVelocity, Action<IBall, IVector, double> checkColision, LoggerAPI log)
    {
      logger = log;
      Position = initialPosition;
      Velocity = initialVelocity;
            _checkColision = checkColision;
            ThreadStart ts = new ThreadStart(MoveLoop);
            _thread = new System.Threading.Thread(ts);
            _thread.IsBackground = true;
            _thread.Start();
    }

    #endregion ctor

    #region IBall

        public void Stop()
        {
            _running = false;
            _thread.Join();
        }


    public event EventHandler<IVector>? NewPositionNotification;

        public void SetVelocity(double x, double y)
        {
            lock(_lock)
            {
                Velocity = new Vector(x, y);
            }
        }

        public IVector Velocity { get; set; }
        public IVector PositionValue => Position;

        #endregion IBall

        #region private

        private readonly LoggerAPI logger;
        private readonly object _lock = new();

        private Thread _thread;
        private volatile bool _running = true;

        private readonly Action<IBall, IVector, double> _checkColision;

        private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

        private void MoveLoop()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (_running)
            {
                double deltaT = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                Vector velocitySnapshot;
                lock (_lock)
                {
                    velocitySnapshot = new Vector(Velocity.x, Velocity.y);
                }
                Vector delta = new Vector(velocitySnapshot.x * deltaT, velocitySnapshot.y * deltaT);

                _checkColision(this, Position, deltaT);
                Move(delta);

                Thread.Sleep(5); 
            }
        }
        private void Move(Vector delta)
        {
            lock (_lock)
            {
                Position = new Vector(Position.x + delta.x, Position.y + delta.y);
                logger.AddToQueue($"Ball position: ({Position.x},{Position.y}), Velocity: ({Velocity.x},{Velocity.y})");
            }
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}