//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Reactive;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
  [TestClass]
  public class BusinessLogicImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
      {
        bool newInstanceDisposed = true;
        newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
        Assert.IsFalse(newInstanceDisposed);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataLayerDisposeFixcure dataLayerFixcure = new DataLayerDisposeFixcure();
      BusinessLogicImplementation newInstance = new(dataLayerFixcure);
      Assert.IsFalse(dataLayerFixcure.Disposed);
      bool newInstanceDisposed = true;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
      Assert.IsTrue(dataLayerFixcure.Disposed);
    }

    [TestMethod]
    public void StartTestMethod()
    {
      DataLayerStartFixcure dataLayerFixcure = new();
      using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
      {
        int called = 0;
        int numberOfBalls2Create = 10;
        newInstance.Start(
          numberOfBalls2Create,
          (startingPosition, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(ball); });
        Assert.AreEqual<int>(1, called);
        Assert.IsTrue(dataLayerFixcure.StartCalled);
        Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
      }
    }

    #region testing instrumentation

    private class DataLayerConstructorFixcure : Data.DataAbstractAPI
    {
      public override void Dispose()
      { }
            public bool Disposed = false;
            public bool SetPositionValidatorCalled = false;
            public Func<IVector, bool> SetPositionValidatorFunc = null;
            public override void SetPositionValidator(Func<IVector, bool> validator)
            {
                SetPositionValidatorCalled = true;
                SetPositionValidatorFunc = validator;
            }

            public override IDisposable Subscribe(IObserver<BallChaneEventArgs> observer)
            {
                return new AnonymousObserver<BallChaneEventArgs>(x => { });
            }
            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
       
      }

            public override void UpdateBallsCount(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void ChangeWindowSize(double windowWidth, double windowHeight, double squareWidth, double squareHeight, Action<double, double> upperLayerHandler, Action<IVector, Data.IBall> updateBalls)
            {
                throw new NotImplementedException();
            }
        }

    private class DataLayerDisposeFixcure : Data.DataAbstractAPI
    {
      internal bool Disposed = false;

      public override void Dispose()
      {
        Disposed = true;
      }

            public override IDisposable Subscribe(IObserver<BallChaneEventArgs> observer)
            {
                return new AnonymousObserver<BallChaneEventArgs>(x => { });
            }
            public bool SetPositionValidatorCalled = false;
            public Func<IVector, bool> SetPositionValidatorFunc;
            public override void SetPositionValidator(Func<IVector, bool> validator)
            {
                SetPositionValidatorCalled = true;
                SetPositionValidatorFunc = validator;
            }
            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
        throw new NotImplementedException();
      }

            public override void UpdateBallsCount(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void ChangeWindowSize(double windowWidth, double windowHeight, double squareWidth, double squareHeight, Action<double, double> upperLayerHandler, Action<IVector, Data.IBall> updateBalls)
            {
                throw new NotImplementedException();
            }
        }

    private class DataLayerStartFixcure : Data.DataAbstractAPI
    {
      internal bool StartCalled = false;
      internal bool UpdateBallsCountCalled = false;
    internal bool ChangeWindowSizeCalled = false;
    internal int NumberOfBallseCreated = -1;
            internal double WindowWidthCreated = -1;
            internal double WindowHeightCreated = -1;
            internal double SquareWidthCreated = -1;
            internal double SquareHegihtCreated = -1;

            public override void Dispose()
      { }
            public bool Disposed = false;
            public override IDisposable Subscribe(IObserver<BallChaneEventArgs> observer)
            {
                return new AnonymousObserver<BallChaneEventArgs>(x => { });
            }
            public override void SetPositionValidator(Func<IVector, bool> validator)
            {
                
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
      {
        StartCalled = true;
                
        NumberOfBallseCreated = numberOfBalls;
        upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
      }

            public override void UpdateBallsCount(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                UpdateBallsCountCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
            }

            public override void ChangeWindowSize(double windowWidth, double windowHeight, double squareWidth, double squareHeight, Action<double, double> upperLayerHandler, Action<IVector, Data.IBall> updateBalls)
            {
                ChangeWindowSizeCalled = true;
                WindowHeightCreated = windowHeight;
                WindowWidthCreated = windowWidth;
                SquareHegihtCreated = squareHeight;
                SquareWidthCreated = squareWidth;
                upperLayerHandler(squareWidth, squareHeight);
                updateBalls(new DataVectorFixture(), new DataBallFixture());
            }

            private record DataVectorFixture : Data.IVector
      {
        public double x { get; init; }
        public double y { get; init; }
      }

      private class DataBallFixture : Data.IBall
      {
        public IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public IVector PositionValue => new DataVectorFixture();
                public void SetVelocity(double x, double y) { }

                public event EventHandler<IVector>? NewPositionNotification = null;
      }
    }

    #endregion testing instrumentation
  }
}