//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Data;
using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball, List<Ball> ballList, Object _lock, ILogger logger)
        {
            this.logger = logger;
            _dataBall = ball;
            this._lock = _lock;
            positionValue = _dataBall.PositionValue;
            _ballList = ballList;
            ball.NewPositionNotification += RaisePositionChangeEvent;
            
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;

        public IVector getPosition => positionValue;

        #endregion IBall

        #region private

        private ILogger logger;

        private IVector positionValue;

        private readonly object _lock;

        private readonly List<Ball> _ballList;

        private readonly Data.IBall _dataBall;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
            CheckCollision(e);
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }


        private void CheckCollision(Data.IVector e)
        {
            lock (_lock)
            {
                IPosition newPos = new Position(e.x, e.y);

                if ((newPos.x <= 0) | (newPos.x + 20 >= 400 - 4 * 2))
                {
                    _dataBall.SetVelocity(-_dataBall.Velocity.x, _dataBall.Velocity.y);
                    logger.AddToQueue(DateTime.UtcNow, "Collision with X wall", _dataBall.PositionValue, _dataBall.Velocity);
                }
                if ((newPos.y <= 0) | (newPos.y + 20 >= 420 - 4 * 2))
                {
                    _dataBall.SetVelocity(_dataBall.Velocity.x, -_dataBall.Velocity.y);
                    logger.AddToQueue(DateTime.UtcNow, "Collision with Y wall", _dataBall.PositionValue, _dataBall.Velocity);
                }
                double distance;
                foreach(Ball others in _ballList)
                {
                    if (others._dataBall.PositionValue.x == _dataBall.PositionValue.x && others._dataBall.PositionValue.y == _dataBall.PositionValue.y)
                    {
                        continue;
                    }

                    distance = Math.Sqrt(Math.Pow(others._dataBall.PositionValue.x - newPos.x, 2) + Math.Pow((others._dataBall.PositionValue.y - newPos.y), 2));
                    if (distance <= 20)
                    {
                        logger.AddToQueue(DateTime.UtcNow, "Collision with ball", _dataBall.PositionValue, _dataBall.Velocity);

                        IPosition posB = new Position(others._dataBall.PositionValue.x, others._dataBall.PositionValue.y);
                        IVector velB = others._dataBall.Velocity;

                        double dx = _dataBall.PositionValue.x - posB.x;
                        double dy = _dataBall.PositionValue.y - posB.y;
                        distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance == 0) distance = 0.01;

                        double nx = dx / distance;
                        double ny = dy / distance;

                        double vA_proj = _dataBall.Velocity.x * nx + _dataBall.Velocity.y * ny;
                        double vB_proj = velB.x * nx + velB.y * ny;
                        double impulse = vA_proj - vB_proj;

                        _dataBall.SetVelocity(_dataBall.Velocity.x - impulse * nx, _dataBall.Velocity.y - impulse * ny);
                        others._dataBall.SetVelocity(velB.x + impulse * nx, velB.y + impulse * ny);
                    }
                }
            }
        }

        #endregion private
    }
}