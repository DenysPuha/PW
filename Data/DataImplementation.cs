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
using System.Diagnostics;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
            eventObservable = Observable.FromEventPattern<BallChaneEventArgs>(this, "BallChanged");
    }

        #endregion ctor

        #region DataAbstractAPI

        public override IDisposable Subscribe(IObserver<BallChaneEventArgs> observer)
        {
            return eventObservable.Subscribe(x => observer.OnNext(x.EventArgs), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

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
                Vector predictDelta = new(0, 0);
                do
                {
                    startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                    startingVelocity = new((random.NextDouble() - 0.5) * 6, (random.NextDouble() - 0.5) * 6);
                    newBall = new(startingPosition, startingVelocity, checkColisionHandler);
                } while (isValidPosition != null && !isValidPosition(startingPosition));

                upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

    public override void UpdateBallsCount(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            int sizeOfBallList = BallsList.Count;
            if (numberOfBalls < sizeOfBallList)
            {
                for(int i = 0; i < sizeOfBallList - numberOfBalls; i++)
                {
                    BallsList.RemoveAt(BallsList.Count-1);
                }
            }
            else
            {
                Random random = new Random();
                for (int i = 0; i < numberOfBalls - sizeOfBallList; i++)
                {
                    Ball? newBall;
                    Vector startingPosition;
                    Vector startingVelocity;
                    Vector predictDelta = new(0, 0);
                    do
                    {
                        startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                        startingVelocity = new((random.NextDouble() - 0.5) * 6, (random.NextDouble() - 0.5) * 6);
                        newBall = new(startingPosition, startingVelocity, checkColisionHandler);
                    } while (isValidPosition != null && !isValidPosition(startingPosition));
                    BallsList.Add(newBall);
                }
            }
            for (int i = 0; i < BallsList.Count; i++)
            {
                upperLayerHandler(BallsList[i].PositionValue, BallsList[i]);
            }
        }

        public event EventHandler<BallChaneEventArgs>? BallChanged;

        
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
        private readonly IObservable<EventPattern<BallChaneEventArgs>> eventObservable;
        private bool Disposed = false;

    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
        private Func<IVector, bool>? isValidPosition;
        private void checkColisionHandler(IBall Ball, IVector Pos)
        {
            BallChanged?.Invoke(this, new BallChaneEventArgs { Ball = Ball, Pos = Pos });
        }

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
    public class BallChaneEventArgs : EventArgs
    {
        public IBall Ball { get; init; }

        public IVector Pos {get; init;}
    }

}
