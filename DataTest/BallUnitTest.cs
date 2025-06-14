﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class BallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Vector testinVector = new Vector(0.0, 0.0);
        Action<IBall, IVector, double> checkColision = (ball, position, refreshTime) => { Assert.IsNotNull(ball); Assert.IsNotNull(position); Assert.IsNotNull(refreshTime); };
            
        Ball newInstance = new(testinVector, testinVector);
    }

    [TestMethod]
    public void MoveTestMethod()
    {
      Vector initialPosition = new(0.0, 0.0);
            Action<IBall, IVector, double> checkColision = (ball, position, refreshTime) => { Assert.IsNotNull(ball); Assert.IsNotNull(position); Assert.IsNotNull(refreshTime); };

            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0));
      IVector curentPosition = new Vector(0.0, 0.0);
      int numberOfCallBackCalled = 0;
      newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
      Assert.AreEqual<int>(0, numberOfCallBackCalled);
      Assert.AreEqual<IVector>(initialPosition, curentPosition);
            
    }
  }
}