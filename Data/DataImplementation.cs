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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
 
    }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));
      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
                Ball? newBall;
                Vector startingPosition;
                Vector startingVelocity;
                do
                {
                    startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                } while (isValidPosition != null && !isValidPosition(startingPosition));
                startingVelocity = new((random.NextDouble() - 0.5) * 300, (random.NextDouble() - 0.5) * 300);
                newBall = new(startingPosition, startingVelocity);

                upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

        
        public override void SetPositionValidator(Func<IVector, bool> validator)
        {
            isValidPosition = validator;
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

        #endregion IDisposable

        #region private

        //private bool disposedValue;
        private bool Disposed = false;

    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
        private Func<IVector, bool>? isValidPosition;

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}
