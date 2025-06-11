//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void MoveTestMethod()
    {
      List<Ball> _testBallList = new();
      DataBallFixture dataBallFixture = new DataBallFixture();
      DummyLogger logger = new DummyLogger();
      object _lock = new();
      IVector testinVector = new VectorFixture(0.0, 0.0);
      Ball newInstance = new(dataBallFixture, _testBallList,_lock, logger);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
      dataBallFixture.Move();
      Assert.AreEqual<int>(1, numberOfCallBackCalled);
    }

    #region testing instrumentation

    private class DataBallFixture : Data.IBall
    {
            private Data.IVector _velocity;

            public Data.IVector Velocity
            {
                get => _velocity;
                set => _velocity = value;
            }

            public event EventHandler<Data.IVector>? NewPositionNotification;

            public DataBallFixture()
            {
                _velocity = new VectorFixture(0.0, 0.0);
            }

            public Data.IVector PositionValue => new VectorFixture(0.0, 0.0);

            public void SetVelocity(double x, double y) { }

            internal void Move()
      {
        NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
      }
    }

    private class VectorFixture : Data.IVector
    {
      internal VectorFixture(double X, double Y)
      {
        x = X; y = Y;
      }

      public double x { get; init; }
      public double y { get; init; }
    }

    private class DummyLogger : ILogger
        {
            public void Dispose()
            {

            }

            public void AddToQueue(DateTime time, string message, IVector position, IVector velocity) { }
        }

    #endregion testing instrumentation
  }
}