//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall
  {
    public Ball(Data.IBall ball, List<Ball> ballList)
    {
      _dataBall = ball;
      positionValue = _dataBall.PositionValue;
      _ballList = ballList;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        public IVector getPosition => positionValue;

        #endregion IBall

        #region private

        IVector positionValue;

        private readonly object _lock = new();

        private readonly List<Ball> _ballList;

        private readonly Data.IBall _dataBall;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
            CheckColision(e);
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

        private void CheckColision(Data.IVector e)
        {
            lock (_lock)
            {
                int indx = -1;
                IPosition newPos = new Position(e.x, e.y);

                if ((newPos.x <= 0) | (newPos.x + 20 >= 400 - 4 * 2))
                {
                    _dataBall.SetVelocity(-_dataBall.Velocity.x, _dataBall.Velocity.y);
                }
                if ((newPos.y <= 0) | (newPos.y + 20 >= 420 - 4 * 2))
                {
                    _dataBall.SetVelocity(_dataBall.Velocity.x, -_dataBall.Velocity.y);
                }
                double distance;
                for (int i = 0; i < _ballList.Count; i++)
                {
                    Ball others = _ballList[i];
                    if (others._dataBall.PositionValue.x == _dataBall.PositionValue.x && others._dataBall.PositionValue.y == _dataBall.PositionValue.y)
                    {
                        continue;
                    }

                    distance = Math.Sqrt(Math.Pow(others._dataBall.PositionValue.x - newPos.x, 2) + Math.Pow((others._dataBall.PositionValue.y - newPos.y), 2));
                    if (distance <= 20)
                    {
                        indx = _ballList.IndexOf(others);
                        break;
                    }
                }
                if (indx == -1) return;
                else
                {
                    Ball B = _ballList[indx];

                    IPosition posB = new Position(B._dataBall.PositionValue.x, B._dataBall.PositionValue.y);
                    IVector velB = B._dataBall.Velocity;

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
                    B._dataBall.SetVelocity(velB.x + impulse * nx, velB.y + impulse * ny);
                }
            }
        }

        #endregion private
    }
}