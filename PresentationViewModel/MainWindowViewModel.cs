//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using System.Reactive;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
      WindowObserver = ModelLayer.SubscribeToWindowChanges(new AnonymousObserver<WindowChangedEventArgs>(e =>
        {
            SquareWidth = e.SquareWidth;
            SquareHeight = e.SquareHeight;
        }));
        }

    #endregion ctor

    #region public API

    public void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      BallCount = numberOfBalls;
      ModelLayer.Start(numberOfBalls);
        }

    public void UpdateBallsCount(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Balls.Clear();
            ModelLayer.UpdateBallsCount(numberOfBalls);
        }

    public void ChangeWindowSize(double windowWidth, double windowHeight, double squareWidth, double squareHeight)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Balls.Clear();
            ModelLayer.ChangeWindowSize(windowWidth, windowHeight, squareWidth, squareHeight);
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
            WindowObserver.Dispose();
            ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private IDisposable WindowObserver = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;

        #endregion private
        private double _windowWidth = 1152;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                RaisePropertyChanged(nameof(WindowWidth));
                ChangeSize.Execute(null);
            }
        }

        private double _windowHeight = 592;
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                RaisePropertyChanged(nameof(WindowHeight));
                ChangeSize.Execute(null);
            }
        }

        private double _squareWidth = 400;
        public double SquareWidth
        {
            get => _squareWidth;
            set
            {
                _squareWidth = value;
                RaisePropertyChanged(nameof(SquareWidth));
            }
        }

        private double _squareHeight = 420;
        public double SquareHeight
        {
            get => _squareHeight;
            set
            {
                _squareHeight = value;
                RaisePropertyChanged(nameof(SquareHeight));
            }
        }

        private RelayCommand _changeSize;
        public RelayCommand ChangeSize
        {
            get
            {
                return _changeSize ??= new RelayCommand(() =>
                {
                    ChangeWindowSize(_windowWidth, _windowHeight, _squareWidth, _squareHeight);
                });
            }
        }

        private Boolean _firstValue = true;
        private int _ballCount = 5;
        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                RaisePropertyChanged(nameof(BallCount));
                if(!_firstValue)
                    StartCommand.Execute(null);
                else
                    _firstValue = false;
            }
        }

        private RelayCommand _startCommand;
        public RelayCommand StartCommand
        {
            get
            {
                return _startCommand ??= new RelayCommand(() =>
                {
                    UpdateBallsCount(_ballCount);
                });
            }
        }

        private RelayCommand _stopCommand;
        public RelayCommand StopCommand
        {
            get
            {
                return _stopCommand ??= new RelayCommand(() =>
                {
                    _ballCount = 0;
                    RaisePropertyChanged(nameof(BallCount));
                    UpdateBallsCount(_ballCount);
                });
            }
        }
    }
}