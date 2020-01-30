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


        public ObservableCollection<MinioBucketModel> BucketsM { get; set; }
        public ObservableCollection<MinioItemModel> ItemsM { get; set; }
        

        // CONSTRUCTOR
        public ViewPageVM() {
            BucketsM = new ObservableCollection<MinioBucketModel>();
            ItemsM = new ObservableCollection<MinioItemModel>();
            refreshBucketsList();
        }

        private void refreshBucketsList() 
        {
            BucketsM.Clear();
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
                BucketsM.Add(t); //
           //List = BucketsM;
        }

        public void OnRefreshClick() 
        {
            refreshBucketsList();
        }

        public void ListViewOut_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_selectedBucket != null)
            {
                LoadListOfItemsInBucket();
            }
        }

        public void LoadListOfItemsInBucket() 
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

            
            ItemsM.Clear();
            foreach (var item in itemslist)
            {
                var itm = new MinioItemModel(item);
                ItemsM.Add(itm);
            }
            List = ItemsM;
        }


        public void ListViewOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // var bucket = e.AddedItems[0] as Buckets;
            //focus_1 = bucket;
            TestString = (e.AddedItems[0] as MinioBucketModel).Name;
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

        private MinioBucketModel _selectedBucket;
        public MinioBucketModel SelectedBucket
        {
            get { return _selectedBucket; }
            set
            {
                _selectedBucket = value;
                RaisePropertyChanged("SelectedBucket");
            }
        }

        private Object _list;
        public Object List
        {
            get { return _list; }
            set
            {
                _list = value;
                RaisePropertyChanged("List");
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
          
        public void CommandExecuted()
        {
            if (ReferenceEquals(_list, _intList))
            {
                List = _stringList;
            }
            else
            {
                List = _intList;
            }
        }
        
   public DelegateCommand<string> ButtonClickTest  { get; }
      
        
 
             ButtonClickTest = new DelegateCommand<string>(str => {
                //BUTTON CLICK
            buckets.Clear();   
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
            buckets.Add(t); //
               
            });

    <TextBlock  DataContext="{Binding SelectedBucket}"  Text="{Binding BucketName, UpdateSourceTrigger=PropertyChanged}" TextWrapping="Wrap" VerticalAlignment="Bottom" Height="38" Margin="68,0,0,79" HorizontalAlignment="Left" Width="1808">Main</TextBlock>

    <TextBlock  Text="{x:Bind ViewModel.TestString,Mode=OneWay}" TextWrapping="Wrap" VerticalAlignment="Bottom" Height="38" Margin="68,0,0,22" HorizontalAlignment="Left" Width="1808">Test</TextBlock>


     */
