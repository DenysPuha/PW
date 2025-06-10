//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using System.Reactive;
using System.Runtime.InteropServices;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    { }

    internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
    {
      layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
            layerBellow.SetPositionValidator(pos => IsValidPosition(new Position(pos.x, pos.y)));
        }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      layerBellow.Dispose();
      Disposed = true;
    }


    public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
            _upperLayerHandler = upperLayerHandler;
     
      layerBellow.Start(numberOfBalls, (startingPosition, databall) => {
          Ball ball = new Ball(databall, _ballList);
          _ballList.Add(ball);
          upperLayerHandler(new Position(startingPosition.x, startingPosition.y), ball);
      });
    }
        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private Action<IPosition, IBall>? _upperLayerHandler;

        private readonly UnderneathLayerAPI layerBellow;

        private readonly List<Ball> _ballList = new();

        private double width = 400;
        private double height = 420;
        private int margin = 4;
        private int ballDiameter = 20;

        public bool IsValidPosition(IPosition position)
        {
            foreach (Ball ball in _ballList)
            {
                double dx = ball.getPosition.x - position.x;
                double dy = ball.getPosition.y - position.y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= ballDiameter)
                    return false;
            }

            return position.x >= 0 + margin && position.x + ballDiameter <= width - margin &&
                   position.y >= 0 + margin && position.y + ballDiameter <= height - margin;
        }


        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}