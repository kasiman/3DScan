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
using Microsoft.Win32;

namespace _3DScan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ImageSourceConverter converter = new ImageSourceConverter();
        OpenFileDialog opnDlg;
        Model model;
        string resPath = @"Img\";
        bool isFullScrn = false;
        bool isWiden;
        bool isOpen = false;


        #region Menu Events


            private void MenuItemOpn_Clck(object sender, RoutedEventArgs e)
            {
                this.opnDlg = new OpenFileDialog();
                this.opnDlg.Filter = "3D Scan Result(*.stl)|*.stl";
                this.opnDlg.Multiselect = false;

                try
                {
                    if (this.opnDlg.ShowDialog() == true)
                    {
                        this.model = new Model(View, VisualModel);
                        if(this.model.Load(this.opnDlg.FileName))
                        {
                            this.model.ModelVisualisation(asPoly.IsChecked.Value, asPoints.IsChecked.Value, frontPlane.IsChecked.Value);
                            isOpen = true;
                        }
                        WinDecor();
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    asPoly.Visibility = System.Windows.Visibility.Visible;
                    asPoints.Visibility = System.Windows.Visibility.Visible;
                    frontPlane.Visibility = System.Windows.Visibility.Visible;
                }
            }

            private void MenuItemExprt_Click(object sender, RoutedEventArgs e)
            {
                SaveFileDialog svDlg = new SaveFileDialog();
                svDlg.Filter = HelixToolkit.Wpf.Exporters.Filter;
                if (svDlg.ShowDialog() == true)
                {
                    this.model.Save(svDlg.FileName);
                }
            }

            private void exitBtn_Clck(object sender, RoutedEventArgs e)
            {
                Application.Current.Shutdown();
            }

            private void ViewItemPoly_Click(object sender, RoutedEventArgs e)
            {
                //this.model.toPoly();
                this.model.ModelVisualisation(true, false, frontPlane.IsChecked.Value);
                asPoly.IsChecked = true;
                asPoints.IsChecked = false;
            }

            private void ViewItemPoint_Click(object sender, RoutedEventArgs e)
            {
                //this.model.toPoint();
                this.model.ModelVisualisation(false, true, frontPlane.IsChecked.Value);
                asPoly.IsChecked = false;
                asPoints.IsChecked = true;
            }

            private void ViewItemPolyPoint_Click(object sender, RoutedEventArgs e)
            {
                //this.model.toPolyPoint();
                this.model.ModelVisualisation(true, true, frontPlane.IsChecked.Value);
                asPoly.IsChecked = true;
                asPoints.IsChecked = true;
            }

            private void ViewItemFrontPlane_Click(object sender, RoutedEventArgs e)
            {
                if (ViewItem4.IsChecked)
                {
                    frontPlane.IsChecked = true;
                    this.model.ModelVisualisation(asPoly.IsChecked.Value, asPoints.IsChecked.Value, frontPlane.IsChecked.Value);
                }
                else
                {
                    frontPlane.IsChecked = false;
                    this.model.ModelVisualisation(asPoly.IsChecked.Value, asPoints.IsChecked.Value, frontPlane.IsChecked.Value);
                }
            }

            private void checkBox_Click(object sender, RoutedEventArgs e)
            {
                this.model.ModelVisualisation(asPoly.IsChecked.Value, asPoints.IsChecked.Value, frontPlane.IsChecked.Value);
                ViewItem4.IsChecked = frontPlane.IsChecked.Value;
            }

            private void WinDecor()
            {
                ViewItem1.IsEnabled = true;
                ViewItem1.Foreground = Brushes.Black;

                ViewItem2.IsEnabled = true;
                ViewItem2.Foreground = Brushes.Black;

                ViewItem3.IsEnabled = true;
                ViewItem3.Foreground = Brushes.Black;

                ViewItem4.IsEnabled = true;
                ViewItem4.Foreground = Brushes.Black;

                FileName.Content = this.opnDlg.SafeFileName;
                asPoly.Visibility = System.Windows.Visibility.Visible;
                asPoints.Visibility = System.Windows.Visibility.Visible;
                frontPlane.Visibility = System.Windows.Visibility.Visible;

                View.ZoomExtents(0);
            }

        #endregion


        #region Minimized, Maximized and Close Window Button


            #region ButtonStyle

                private void clsImg_MouseEnter(object sender, MouseEventArgs e)
                {
                    closeImg.Source = (ImageSource)converter.ConvertFromString(resPath + "closeSelected.png");
                    // closeImg.Source = (ImageSource) converter.ConvertFromString(System.Windows.Forms.Application.StartupPath + "\\Img\\closeSelected.png");
                }

                private void clsImg_MouseLeave(object sender, MouseEventArgs e)
                {
                    closeImg.Source = (ImageSource)converter.ConvertFromString(resPath + "close.png");
                }

                private void fullScrImg_MouseEnter(object sender, MouseEventArgs e)
                {
                     if (!isFullScrn)
                         fullSreenImg.Source = (ImageSource)converter.ConvertFromString(resPath + "fullScreenSelected.png");
                     else
                         fullSreenImg.Source = (ImageSource)converter.ConvertFromString(resPath + "isFullScreenSelected.png");
                }

                private void fullScrImg_MouseLeave(object sender, MouseEventArgs e)
                {
                     if (!isFullScrn)
                         fullSreenImg.Source = (ImageSource)converter.ConvertFromString(resPath + "fullScreen.png");
                     else
                         fullSreenImg.Source = (ImageSource)converter.ConvertFromString(resPath + "isFullScreen.png");
                }

                private void minWndwImg_MouseEnter(object sender, MouseEventArgs e)
                {
                    minimizedImg.Source = (ImageSource)converter.ConvertFromString(resPath + "minimizedSelected.png");
                }

                private void minWndwImg_MouseLeave(object sender, MouseEventArgs e)
                {
                    minimizedImg.Source = (ImageSource)converter.ConvertFromString(resPath + "minimized.png");
                }

            #endregion
                
            #region ButtonEvents

                private void minWndwBtn_Clck(object sender, MouseButtonEventArgs e)
                {
                    this.WindowState = System.Windows.WindowState.Minimized;
                }

                private void fullScrBtn_Clck(object sender, MouseButtonEventArgs e)
                {
                    if (!isFullScrn)
                    {
                        this.WindowState = System.Windows.WindowState.Maximized;
                        //fullSreenImg.Source = (ImageSource)converter.ConvertFromString("D:\\3D_Scan\\3DScan\\Img\\isFullScreen.png");
                        isFullScrn = true;
                    }
                    else
                    {
                        this.WindowState = System.Windows.WindowState.Normal;
                        //fullSreenImg.Source = (ImageSource)converter.ConvertFromString("D:\\3D_Scan\\3DScan\\Img\\fullScreen.png");
                        isFullScrn = false;
                    }
                }

            #endregion
                

        #endregion


        #region Menu Designer

            private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
            {
                MenuItem.Background = Brushes.Black;
                MenuItem.Foreground = Brushes.LightGray;
                MenuItem1.Foreground = Brushes.Black;
                MenuItem2.Foreground = Brushes.Black;
                MenuItem3.Foreground = Brushes.Black;
            }

            private void MenuItem_MouseLeave(object sender, MouseEventArgs e)
            {
                MenuItem.Background = Brushes.Transparent;
                MenuItem.Foreground = Brushes.Black;
            }


            private void ViewItem_MouseEnter(object sender, MouseEventArgs e)
            {
                ViewItem.Background = Brushes.Black;
                ViewItem.Foreground = Brushes.LightGray;

                if (isOpen)
                {
                    ViewItem1.Foreground = Brushes.Black;
                    ViewItem2.Foreground = Brushes.Black;
                    ViewItem3.Foreground = Brushes.Black;
                }
            }

            private void ViewItem_MouseLeave(object sender, MouseEventArgs e)
            {
                ViewItem.Background = Brushes.Transparent;
                ViewItem.Foreground = Brushes.Black;
            }


        #endregion


        #region Window Size & Position
        
            private void PositionRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                this.DragMove();
            }

            private void HorizontalResizing_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                isWiden = true;
            }

            private void HorizontalResizing_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isWiden = false;
                Rectangle rect = (Rectangle)sender;
                rect.ReleaseMouseCapture();
            }

            private void HorizontalResizing_MouseMove(object sender, MouseEventArgs e)
            {
                Rectangle rect = (Rectangle)sender;
                if (isWiden)
                {
                    rect.CaptureMouse();
                    double newWidth = e.GetPosition(this).X + 5;
                    if (newWidth > 0) this.Width = newWidth;
                }
            }

            private void VerticalResizing_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                isWiden = true;
            }

            private void VerticalResizing_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isWiden = false;
                Rectangle rect = (Rectangle)sender;
                rect.ReleaseMouseCapture();
            }

            private void VerticalResizing_MouseMove(object sender, MouseEventArgs e)
            {
                Rectangle rect = (Rectangle)sender;
                if (isWiden)
                {
                    rect.CaptureMouse();
                    double newHeight = e.GetPosition(this).Y + 5;
                    if (newHeight > 0) this.Height = newHeight;
                }
            }

        #endregion

    }
}
