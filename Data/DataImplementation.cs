﻿//____________________________________________________________________________________________________________________________________
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
                    startingVelocity = new((random.NextDouble() - 0.5) * 300, (random.NextDouble() - 0.5) * 300);
                    newBall = new(startingPosition, startingVelocity, checkColisionHandler, logger);
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
                    BallsList[BallsList.Count - 1].Stop();
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
                        startingVelocity = new((random.NextDouble() - 0.5) * 300, (random.NextDouble() - 0.5) * 300);
                        newBall = new(startingPosition, startingVelocity, checkColisionHandler, logger);
                    } while (isValidPosition != null && !isValidPosition(startingPosition));
                    BallsList.Add(newBall);
                }
            }
            for (int i = 0; i < BallsList.Count; i++)
            {
                upperLayerHandler(BallsList[i].PositionValue, BallsList[i]);
            }
        }

    public override void ChangeWindowSize(double windowWidth, double windowHeight, double squareWidth, double squareHeight, Action<double, double> upperLayerHandler, Action<IVector, IBall> updateBalls)
    {

            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            if (updateBalls == null)
                throw new ArgumentNullException(nameof(updateBalls));
            Boolean isSizeChanged = false;
            if (windowWidth - squareWidth <= 80)
            {
                squareWidth = windowWidth / 1.2;
                squareHeight = squareWidth * 1.05;
                isSizeChanged = true;
            }
            if (windowHeight - squareHeight <= 140)
            {
                squareHeight = windowHeight / 1.33;
                squareWidth = squareHeight / 1.05;
                isSizeChanged = true;
            }
            if (isSizeChanged)
            {
                List<Ball> copy = new();
                foreach (Ball item in BallsList)
                {
                    Vector newPosition = new(item.PositionValue.x * squareWidth / width, item.PositionValue.y * squareHeight / height);
                    Vector newVelocity = (Vector)item.Velocity;
                    Ball newBall = new(newPosition, newVelocity, checkColisionHandler, logger);
                    copy.Add(newBall);
                }
                BallsList.Clear();
                BallsList = copy;
            }
            foreach (Ball item in BallsList)
            {
                updateBalls(item.PositionValue, item);
            }
            width = squareWidth;
            height = squareHeight;
            upperLayerHandler(width, height);
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
        private readonly Logger logger = new();

    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
        private Func<IVector, bool>? isValidPosition;
        private double width = 400;
            private double height = 420;
        private void checkColisionHandler(IBall Ball, IVector Pos, double refreshTime)
        {
            BallChanged?.Invoke(this, new BallChaneEventArgs { Ball = Ball, Pos = Pos, refreshTime = refreshTime });
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
        public required IBall Ball { get; init; }

        public required IVector Pos {get; init;}

        public double refreshTime { get; init; }
    }

    internal class Logger : LoggerAPI
    {

        #region ctor
        public Logger()
        {
            ThreadStart ts = new ThreadStart(LogLoop);
            thread = new System.Threading.Thread(ts);
            thread.IsBackground = true;
            thread.Start();
        }
        #endregion ctor
        #region DataAbstractAPI
        public override void AddToQueue(string msg)
        {
            if (!isDisposed)
                queue.Add($"{DateTime.Now:HH:mm:ss:fff}: {msg}");
        }

        #endregion DataAbstractAPI
        #region IDisposable
        public override void Dispose()
        {
            if (!isDisposed)
            {
                queue.CompleteAdding();
                isDisposed = true;
                thread.Join();
            }
        }
        #endregion IDisposable
        #region private
        private Thread thread;
        private bool isDisposed = false;
        private readonly BlockingCollection<string> queue = new();
        private void LogLoop()
        {
            using StreamWriter writer = new("log.txt", false);
            foreach (string msg in queue.GetConsumingEnumerable())
            {
                writer.WriteLine(msg);
                writer.Flush();
            }
        }
        #endregion private
    }

}
