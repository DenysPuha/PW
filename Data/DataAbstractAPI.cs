﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, ILogger logger);

        public abstract void SetPositionValidator(Func<IVector, bool> validator);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

#endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

    public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; init; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; init; }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

        void SetVelocity(double x, double y);
        IVector PositionValue { get; }
        IVector Velocity { get;}
  }

    public interface ILogger : IDisposable
    {
        public void AddToQueue(DateTime time, string message, IVector position, IVector velocity);
        static ILogger CreateDefaultLogger() => Logger.LoggerInstance;
    }

}