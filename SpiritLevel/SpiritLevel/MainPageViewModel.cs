using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SpiritLevel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        readonly SensorSpeed speed = SensorSpeed.Fastest;

        private int ticks;
        private double rollSum, pitchSum;

        private Stopwatch sw = new Stopwatch();
        private Stopwatch delaySw = new Stopwatch();

        private double _pitch;
        public double Pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                _pitch = value;
                OnPropertyChanged("Pitch");
            }
        }

        private double _roll;
        public double Roll
        {
            get
            {
                return _roll;
            }
            set
            {
                _roll = value;
                OnPropertyChanged("Roll");
            }
        }

        private double _yaw;
        public double Yaw
        {
            get
            {
                return _yaw;
            }
            set
            {
                _yaw = value;
                OnPropertyChanged("Yaw");
            }
        }

        private double _avgTime;
        public double AvgTime
        {
            get
            {
                return _avgTime;
            }
            set
            {
                _avgTime = value;
                OnPropertyChanged("AvgTime");
            }
        }

        private double _delayTime;
        public double DelayTime
        {
            get
            {
                return _delayTime;
            }
            set
            {
                _delayTime = value;
                OnPropertyChanged("DelayTime");
            }
        }

        private double _elapsed;
        public double Elapsed
        {
            get
            {
                return (int)_elapsed;
            }
            set
            {
                _elapsed = value;
                OnPropertyChanged("Elapsed");
            }
        }

        private double _delayElapsed;
        public double DelayElapsed
        {
            get
            {
                return (int)_delayElapsed;
            }
            set
            {
                _delayElapsed = value;
                OnPropertyChanged("DelayElapsed");
            }
        }

        private double _avgPitch;
        public double AvgPitch
        {
            get
            {
                return _avgPitch;
            }
            set
            {
                _avgPitch = value;
                OnPropertyChanged("AvgPitch");
            }
        }

        private double _avgRoll;
        public double AvgRoll
        {
            get
            {
                return _avgRoll;
            }
            set
            {
                _avgRoll = value;
                OnPropertyChanged("AvgRoll");
            }
        }

        private double _avgYaw;
        public double AvgYaw
        {
            get
            {
                return _avgYaw;
            }
            set
            {
                _avgYaw = value;
                OnPropertyChanged("AvgYaw");
            }
        }

        private ObservableCollection<ResultModel> _results;
        public ObservableCollection<ResultModel> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
                OnPropertyChanged("Results");
            }
        }

        public MainPageViewModel()
        {
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            Accelerometer.Start(speed);

            Results = new ObservableCollection<ResultModel>();
            AvgTime = 10;
            DelayTime = 3;
        }

        public ICommand Toggle => new Command(() => ToggleTimer());


        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;

            float X = data.Acceleration.X;
            float Y = data.Acceleration.Y;
            float Z = data.Acceleration.Z;

            Pitch = Math.Atan2(-X, Math.Sqrt(Y * Y + Z * Z)) * 180 / Math.PI;
            Roll = Math.Atan2(Y, Z) * 180 / Math.PI;

            if (sw.IsRunning)
            {
                pitchSum += Pitch;
                rollSum += Roll;
                Elapsed = sw.ElapsedMilliseconds / 1000;
                ticks++;

                if (sw.ElapsedMilliseconds >= AvgTime * 1000)
                {
                    ToggleTimer();
                }
            }
            else
            {
                DelayElapsed = DelayTime - delaySw.ElapsedMilliseconds / 1000;
            }

        }

        public void ToggleTimer()
        {
            if (sw.IsRunning)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }

        private async void StartTimer()
        {
            delaySw.Start();
            await Task.Delay((int)(DelayTime * 1000));
            delaySw.Stop();
            delaySw.Reset();
            sw.Start();
        }

        private void StopTimer()
        {
            SetAvgs();

            Results.Add(
                new ResultModel
                {
                    Roll = AvgRoll,
                    Pitch = AvgPitch
                });

            ResetAvgs();
            sw.Stop();
            sw.Reset();
        }

        private void ResetAvgs()
        {
            pitchSum = 0;
            rollSum = 0;
            ticks = 0;
        }

        private void SetAvgs()
        {
            AvgPitch = pitchSum / ticks;
            AvgRoll = rollSum / ticks;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
