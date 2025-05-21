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

    internal Ball(Vector initialPosition, Vector initialVelocity, Action<IBall, IVector> checkColision)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
            _checkColision = checkColision;

            _thread = new Thread(MoveLoop);
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
            Velocity = new Vector(x, y);
        }

        public IVector Velocity { get; set; }
    public IVector PositionValue => Position;

        #endregion IBall

        #region private

        private Thread _thread;
        private volatile bool _running = true;

        private readonly Action<IBall, IVector> _checkColision;

        private Vector Position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }
        private void MoveLoop() {
            while(_running){
                _checkColision(this, Position);
                Move(new Vector(Velocity.x/ Math.Abs(Velocity.x), Velocity.y / Math.Abs(Velocity.y)));
                Thread.Sleep((int)(20/ Math.Sqrt(Velocity.x*Velocity.x+Velocity.y*Velocity.y)));
            }
         }

    private void Move(Vector delta)
    {
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}