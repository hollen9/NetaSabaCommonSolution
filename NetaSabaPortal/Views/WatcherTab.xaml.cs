using Microsoft.Extensions.DependencyInjection;
using NetaSabaPortal.Models;
using NetaSabaPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetaSabaPortal.Views
{
    /// <summary>
    /// Interaction logic for WatcherTab.xaml
    /// </summary>
    public partial class WatcherTab : UserControl
    {
        private int _current_zone = 1;
        private int _current_frame = 0;
        private int[] _maxFrames = { 13, 7, 8 };

        public WatcherTab()
        {
            InitializeComponent();

            var services = App.Current.Svc;
            var vm = services.GetRequiredService<MainWindowVM>();
            vm.WatcherTimerHandlerStateChanged += (s, e) =>
            {
                if (e == MainWindowVM.WatcherTimerHandlerType.Start)
                {
                    _current_zone = 0;
                }
                else
                {
                    Task.Run(() => 
                    {
                        // Keep playing LoadingAnimation for n seconds
                        int n = 5;
                        System.Threading.Thread.Sleep(n * 1000);
                        _current_zone = 1;
                    });
                }
                _current_frame = 0;
            };

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(40000 * 60);
            timer.Tick += (s,e) => 
            {
                if (_current_zone == 0 && timer.Interval.Seconds != 20000 * 60)
                {
                    timer.Interval = TimeSpan.FromTicks(20000 * 60);
                }
                else if (_current_zone == 1 && timer.Interval.Seconds != 40000 * 60)
                {
                    timer.Interval = TimeSpan.FromTicks(40000 * 60);
                }

                if ((_current_frame + 1) > _maxFrames[_current_zone])
                {
                    _current_frame = 0;
                    // _current_zone++;
                    //if (_current_zone > 2)
                    //{
                    //    _current_zone = 0;
                    //}
                    if (_current_zone == 0)
                    {
                        _current_zone = 2;
                    }
                }
                int maxFrames = _maxFrames[_current_zone];
                // Console.WriteLine($"Frame: {_current_frame}; MaxF: {maxFrames}; Zone: {_current_zone}");
                DisplayHorizontalSprite(x=> this.SpriteSheetOffset.X=x, y=> this.SpriteSheetOffset.Y = y, _current_zone, _current_frame, 5, 100, 80);
                _current_frame++;
            };
            timer.Start();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is not MainWindowVM vm)
            {
                return;
            }
            if (e.Source is not ListViewItem lvItem)
            {
                return;
            }
            if (lvItem.Content is not WatcherItem wItem)
            {
                return;
            }
            vm.ModifyWatchItemCmd.Execute(wItem);
        }

        private void DisplayHorizontalSprite(Action<double> setX, Action<double> setY, int zone, int frame, int count_per_row, int sprite_width, int sprite_height)
        {
            double x;
            double y;

            int row, column;
            int maxFrames = 0;
            //int sprite_width = 100;
            //int sprite_height = 80;

            int col_count = count_per_row; //5
            int padding_left = 0;

            padding_left = col_count * zone;

            row = frame / col_count;
            column = frame % col_count;
            x = - (column + padding_left) * sprite_width;
            y = -row * sprite_height;
            setX(x);
            setY(y);
        }

        //private void LoadingGif_AnimationCompleted(DependencyObject d, XamlAnimatedGif.AnimationCompletedEventArgs e)
        //{
        //    if (DataContext is not MainWindowVM vm)
        //    {
        //        return;
        //    }
        //    vm.HandleWatcherGif_AniCompleted(d, e);
        //}

        //private void Image_AnimationStarted(DependencyObject d, XamlAnimatedGif.AnimationStartedEventArgs e)
        //{

        //}
    }
}
