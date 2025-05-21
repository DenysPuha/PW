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
    public Ball(Data.IBall ball, IVector position)
    {
      _dataBall = ball;
      Pos = position;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

        public IVector PositionValue => Pos;

        public IVector Velocity => _dataBall.Velocity;

        public void setVelocity(double x, double y)
        {
            _dataBall.SetVelocity(x, y);
        }

        #endregion IBall

        #region private

        private readonly Data.IBall _dataBall;

        private IVector Pos;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
            Pos = e;
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

        #endregion private
    }
}