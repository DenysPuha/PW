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

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor

    public DataImplementation()
    {
      MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(20)); // Timer - 100 bylo
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
                Ball? newBall; // ? - znaczy, że wartość może być null
                Vector startingPosition;
                Vector startingVelocity;
                Vector predictDelta = new(0, 0);
                do
                {
                    startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
                    startingVelocity = new((random.NextDouble() - 0.5) * 6, (random.NextDouble() - 0.5) * 6);
                    newBall = new(startingPosition, startingVelocity); // prędkość - velocity
                } while (IsValidMode(startingPosition, newBall)!=-1);
        
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
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

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];

        private const int margin = 4;
        private const double ballDiameter = 20.0;
        private const double width = 400.0;
        private const double height = 420.0;

        private int IsValidMode(Vector newPosition, Ball currentBall)
        {
            if((newPosition.x /* - ballDiameter*/ <= 0-margin/2)  | (newPosition.x + ballDiameter >= width - margin*2))
            {
                return -2; //ściana po x
            }
            else if((newPosition.y <= 0 - margin / 2) | (newPosition.y + ballDiameter >= height - margin * 2))
            {
                return -3; //ściana po y
            }
            else
            {
                foreach (Ball others in BallsList)
                {
                    if (others == currentBall)
                        continue;
                    double distance = Math.Sqrt(Math.Pow((others.PositionValue.x - newPosition.x), 2) + Math.Pow((others.PositionValue.y - newPosition.y), 2));
                    if (distance <= ballDiameter)
                    {
                        return BallsList.IndexOf(others); //indeks elementu, z którym
                                                          //zderza się bieżąca kula
                    }
                }
            }
            return -1; //wszystko ok
        }

    private void Move(object? x)
    {
    // w przypadku dużej ilości kul pozycji bez kolizji może nie istniać i pętla może iść
    // w nieskończoność. Dlatego wprowadzam limit, żeby nie było stopu całego programu
    // przez nieskończoną pętlę
            int limit = 50;
            foreach (Ball item in BallsList)
            {
                int i = 0;
                while (true)
                {
                    //Vector delta = new((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10);
                    Vector delta = (Vector)item.Velocity;
                    Vector newPosition = new(item.PositionValue.x + delta.x, item.PositionValue.y + delta.y);
                    int code_number = IsValidMode(newPosition, item);
                    if (code_number==-1)
                    {
                        item.Move(delta);
                        break;
                    }
                    else if(code_number==-2) //ściane boczna 
                    {
                        item.Velocity = new Vector(-item.Velocity.x, item.Velocity.y);
                    }
                    else if(code_number == -3)
                    {
                        item.Velocity = new Vector(item.Velocity.x, -item.Velocity.y);
                    }
                    else // code_number == [0, .... inf] - uderzenie w kulę. [0, .... inf]
                    //indeks kuli
                    {
                        //item.Velocity = new Vector(-item.Velocity.x, -item.Velocity.y);
                        //BallsList[code_number].Velocity = new Vector(-BallsList[code_number].Velocity.x, BallsList[code_number].Velocity.y);
                        Ball A = item;
                        Ball B = BallsList[code_number];


                        Vector posA = (Vector)A.PositionValue;
                        Vector posB = (Vector)B.PositionValue;
                        Vector velA = (Vector)A.Velocity;
                        Vector velB = (Vector)B.Velocity;

                        double dx = posA.x - posB.x;
                        double dy = posA.y - posB.y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance == 0) distance = 0.01;

                        double nx = dx / distance;
                        double ny = dy / distance;

                        double vA_proj = velA.x * nx + velA.y * ny;
                        double vB_proj = velB.x * nx + velB.y * ny;
                        double impulse = vA_proj - vB_proj;

                        A.Velocity = new Vector(
    velA.x - impulse * nx,
    velA.y - impulse * ny
);

                        B.Velocity = new Vector(
                            velB.x + impulse * nx,
                            velB.y + impulse * ny
                        );

                    }

                    if (i == limit)
                        break;
                    i++;
                }
            }
       
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
}
