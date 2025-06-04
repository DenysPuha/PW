using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data;
using System;
using System.Threading;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallLoggerComparisonTest
    {
        private class dummyLogger : LoggerAPI
        {
            public override void AddToQueue(string msg)
            {
            }
            public override void Dispose()
            {
            }
        }
        [TestMethod]
        public void BallMovesSameWithAndWithoutLogger()
        {
            Vector startPosition = new(0.0, 0.0);
            Vector velocity = new(50.0, 0.0);

            Logger logger = new Logger();
            dummyLogger dLogger = new dummyLogger();

            Action<IBall, IVector, double> checkColision = (ball, position, refreshTime) => { Assert.IsNotNull(ball); Assert.IsNotNull(position); Assert.IsNotNull(refreshTime); };


            Ball ballWithLogger = new(startPosition, velocity, checkColision, logger);
            Ball ballWithoutLogger = new(startPosition, velocity, checkColision, dLogger);

            Thread.Sleep(200);

            IVector posWithLogger = ballWithLogger.PositionValue;
            IVector posWithoutLogger = ballWithoutLogger.PositionValue;

            double diffX = Math.Abs(posWithLogger.x - posWithoutLogger.x);
            double diffY = Math.Abs(posWithLogger.y - posWithoutLogger.y);

            Assert.IsTrue(diffX < 1.0, $"X mismatch: {posWithLogger.x} vs {posWithoutLogger.x}");
            Assert.IsTrue(diffY < 1.0, $"Y mismatch: {posWithLogger.y} vs {posWithoutLogger.y}");

            ballWithLogger.Stop();
            ballWithoutLogger.Stop();
            logger.Dispose();
        }
    }
}