using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Minio;
using Minio.DataModel;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using MinioTApp2.Resources.Pages;
using MUXC = Microsoft.UI.Xaml.Controls;


using System.Collections.ObjectModel;
using MinioTApp2.Resources.Classes;

namespace MinioTApp2
{



    public sealed partial class MainPage : Page
    {



        // переменная для отображения крутилки-прогрессбара
        private bool isWorkInProgress
        {
            get { return isWorkInProgress;}
           set
            {
                if (value == false)
                    workRing.Visibility = Visibility.Collapsed;
                else if (value == true)

                    workRing.Visibility = Visibility.Visible;
            }
        }
        //фокус в главном окне
        Buckets focus_1 {
            get { return focus_1; }
            set
            {
                TestOut.Text = value.BucketName;
            }
        }

        ViewerPage itemsViewerPage = new ViewerPage();


        ObservableCollection<Buckets> buckets = new ObservableCollection<Buckets>();



        public MainPage()
        {
            this.InitializeComponent();



            MainWindowContControl.Content = itemsViewerPage;



            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.StandardUICommand"))
            {
                var cutCommand = new StandardUICommand(StandardUICommandKind.Cut);
                cutCommand.ExecuteRequested += CutCommand_ExecuteRequested;
                CutItem.Command = cutCommand;

            }

            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Input.KeyboardAccelerator"))
            {
                var accelerator = new KeyboardAccelerator();
                accelerator.Modifiers = Windows.System.VirtualKeyModifiers.Control;
                accelerator.Key = Windows.System.VirtualKey.Z;
                UndoItem.KeyboardAccelerators.Add(accelerator);
            }
            UndoItem.Click += UndoItem_Click;



        }


   


        private void UndoItem_Click(object sender, RoutedEventArgs e)
        {
           TestOut.Text = "Undo Clicked";
        }

        private void CutCommand_ExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            TestOut.Text = "Cut Clicked";
        }




    



        private void MenuOpenItem_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void MenuSaveItem_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
     




        // (new System.Threading.Thread(delegate () {
        //          })).Start(); 

