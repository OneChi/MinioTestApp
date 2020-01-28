using MinioTApp2.Model.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Minio;
using MinioTApp2;
using Minio.DataModel;
using Prism.Mvvm;
using Prism.Commands;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Minio.Exceptions;
using System.Threading;

namespace MinioTApp2.ViewModel.ViewModels
{
    public class ViewPageVM : BindableBase
    {

        public ObservableCollection<BucketsMinio> buckets { get; set; }

       

    public ViewPageVM() {

            buckets = new ObservableCollection<BucketsMinio>();

            

        }

        public void OnRefreshClick() 
        {
            buckets.Clear();
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
                buckets.Add(t); //
        }

        public void ListViewOut_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var objects = App.Repository.ListObjectsAsync(_selectedBucket.BucketName, null, false);
            ObservableCollection<Item> itemslist = new ObservableCollection<Item>();
            bool complete = false;
                IDisposable subscription = objects.Subscribe(
                        item => itemslist.Add(item),
                        ex => Console.WriteLine("OnError: {0}", ex.Message),  // error handling
                        () => complete = true);
               // Thread.Sleep(1000);
                while (complete != true)
                {
                    Thread.Sleep(10);
                }

            buckets.Clear();

            foreach(var item in itemslist)
            {
                
            }
                
        }

        public void ListViewOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // var bucket = e.AddedItems[0] as Buckets;
            //focus_1 = bucket;
            TestString = (e.AddedItems[0] as BucketsMinio).Name;
            //TestOut.Text = bucket.Name;
        }


        private string _testString;
        public string TestString 
        {
            get { return _testString; }
            set
            { 
                _testString = value;
                RaisePropertyChanged("TestString"); 
            }   

        }

        private BucketsMinio _selectedBucket;
        public BucketsMinio SelectedBucket
        {
            get { return _selectedBucket; }
            set
            {
                _selectedBucket = value;
                RaisePropertyChanged("SelectedBucket");
            }
        }



        private static bool HandleBatchExceptions(Exception exceptionToHandle)
        {
            if (exceptionToHandle is ArgumentNullException)
            {
                //I'm handling the ArgumentNullException.
                Console.WriteLine("Handling the ArgumentNullException.");
                //I handled this Exception, return true.
                return true;
            }

            //I'm only handling ArgumentNullExceptions.
            Console.WriteLine(string.Format("I'm not handling the {0}.", exceptionToHandle.GetType()));
            //I didn't handle this Exception, return false.
            return false;
        }

    }
}



/*
  
   public DelegateCommand<string> ButtonClickTest  { get; }
      
        
 
             ButtonClickTest = new DelegateCommand<string>(str => {
                //BUTTON CLICK
            buckets.Clear();   
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
            buckets.Add(t); //
               
            });

    <TextBlock  DataContext="{Binding SelectedBucket}"  Text="{Binding BucketName, UpdateSourceTrigger=PropertyChanged}" TextWrapping="Wrap" VerticalAlignment="Bottom" Height="38" Margin="68,0,0,79" HorizontalAlignment="Left" Width="1808">Main</TextBlock>



     */
