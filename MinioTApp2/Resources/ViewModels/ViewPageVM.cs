﻿using Minio.DataModel;
using MinioTApp2.Model.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Web;

namespace MinioTApp2.ViewModel.ViewModels
{
    public class ViewPageVM : BindableBase
    {


        public ObservableCollection<MinioBucketModel> BucketsM { get; set; }
        public ObservableCollection<MinioItemModel> ItemsM { get; set; }


        // CONSTRUCTOR
        public ViewPageVM()
        {
            BucketsM = new ObservableCollection<MinioBucketModel>();
            ItemsM = new ObservableCollection<MinioItemModel>();
            refreshBucketsList();
            if (BucketsM.Count > 0)
                _selectedBucket = BucketsM[0];
            LoadListOfItemsInBucket(_selectedBucket);

        }


        private void refreshBucketsList()
        {
            ProgressRingState = true;
            BucketsM.Clear();
            var rep = App.Repository.getListBuckets();
            foreach (var t in rep)
                BucketsM.Add(t); //
                                 //List = BucketsM;
            ProgressRingState = false;
        }

        public void OnRefreshClick()
        {
            refreshBucketsList();

        }

        public void ListViewBuckets_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_selectedBucket != null)
            {
                // _openedBucket = SelectedBucket;
                if (_openedHistory.Count > 0)
                    _openedHistory.Clear();
                LoadListOfItemsInBucket(_selectedBucket);
            }
        }

        public void LoadListOfItemsInBucket(MinioBucketModel openingBucket)
        {
            _openedBucket = openingBucket;
            var objects = App.Repository.ListObjectsAsync(openingBucket.BucketName, null, false);
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

        public void CommandBarDelete_Click(object sender, RoutedEventArgs e)
        {
            ProgressRingState = true;
            var taskDelete = App.Repository.RemoveObjectFromServerAsync(_openedBucket.BucketName, SelectedItem.ItemKey);
            Task.WaitAll(taskDelete);
            ProgressRingState = false;
            if (_openedHistory.Count > 0)
                GoIntoPath(_openedBucket, _openedHistory[_openedHistory.Count - 1]);
            else
                LoadListOfItemsInBucket(_openedBucket);
        }

        public void ListViewItems_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_openedBucket != null)
            {
                if (_selectedItem.ItemIsDir)
                {
                    // go in path
                    _openedItem = SelectedItem;
                    _openedHistory.Add(SelectedItem);
                    GoIntoPath(_openedBucket, _selectedItem);
                }
            }
        }



        public void backButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_openedHistory.Count > 1)
                {
                    _openedHistory.RemoveAt(_openedHistory.Count - 1);
                    GoIntoPath(_openedBucket, _openedHistory[_openedHistory.Count - 1]);
                }
                else if (_openedHistory.Count == 1)
                {
                    _openedHistory.Clear();
                    LoadListOfItemsInBucket(_openedBucket);
                }
            } catch (Exception exe)
            {
                DisplayWarningDialog("Restart application please. There some error with navigation");
            }

        }

        public void GoIntoPath(MinioBucketModel openedBucket, MinioItemModel openingItem)
        {
            // var objectStat = App.Repository.StatOfObjectAsync(_selectedBucket.BucketName,  _selectedItem.ItemKey);
            var objects = App.Repository.ListObjectsAsync(openedBucket.BucketName, openingItem.ItemKey, false);
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

        public async void LoadOnServerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
                openPicker.CommitButtonText = "Открыть";
                openPicker.FileTypeFilter.Add("*");
                ProgressRingState = true;
                var file = await openPicker.PickSingleFileAsync();

                ProgressRingState = false;

                if (file != null)
                {
                    var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    using (var a = fileStream.AsStream())
                    {
                        String newFileName = "";
                        ProgressRingState = true;
                        if (_openedHistory.Count > 0)
                            newFileName = _openedHistory[_openedHistory.Count - 1].ItemKey + file.Name;
                        else
                            newFileName = file.Name;

                        await App.Repository.PutObjectFromStreamAsync(_openedBucket.BucketName, newFileName, a, a.Length);
                        ProgressRingState = false;
                    }
                    fileStream.Dispose();
                    if (_openedHistory.Count > 0)
                        GoIntoPath(_openedBucket, _openedHistory[_openedHistory.Count - 1]);
                    else
                        LoadListOfItemsInBucket(_openedBucket);

                }
            }
            catch (Exception exe)
            {
                DisplayWarningDialog(exe.Message);
            }

        }

        public async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var objStat = await App.Repository.StatOfObjectAsync(_openedBucket.BucketName, SelectedItem.ItemKey);
                var exten = System.IO.Path.GetExtension(objStat.ObjectName);
                var savePicker = new FileSavePicker();

                // место для сохранения по умолчанию
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // устанавливаем типы файлов для сохранения

                savePicker.FileTypeChoices.Add(exten, new List<string>() { exten });
                // устанавливаем имя нового файла по умолчанию
                savePicker.SuggestedFileName = "New Document";
                savePicker.CommitButtonText = "Сохранить";
                ProgressRingState = true;
                var new_file = await savePicker.PickSaveFileAsync();
                ProgressRingState = false;
                if (new_file != null)
                {
                    var randomAccessStream = await new_file.OpenAsync(FileAccessMode.ReadWrite);
                    using (var fileSream = randomAccessStream.AsStream())
                    {
                        ProgressRingState = true;
                        var tsk = App.Repository.GetObjectAsStreamAsync(_openedBucket.BucketName, SelectedItem.ItemKey, (stream) =>
                         {

                            //StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            stream.CopyTo(fileSream); //= reader.ReadToEnd();

                        });
                        Task.WaitAll(tsk);
                        ProgressRingState = false;

                    }
                    randomAccessStream.Dispose();
                    //await FileIO.WriteTextAsync(new_file, myTextBox.Text);
                }
            }
            catch (Exception exe)
            {
                DisplayWarningDialog(exe.Message);
            }
        }


        public async void createNewFolderInBucket_Click(object sender, RoutedEventArgs e) 
        {
            createNewFolderInBucket(_fileFolderName);

           // createNewFolderInBucket();
        }

        private async void createNewFolderInBucket(string folderName) 
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                await storageFolder.CreateFileAsync("placeholder.txt",
                Windows.Storage.CreationCollisionOption.ReplaceExisting);
                var fileStream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
                using (var stream = fileStream.AsStream())
                {

                    String newFileName = "";
                    if (_openedHistory.Count > 0)
                        newFileName = _openedHistory[_openedHistory.Count - 1].ItemKey + folderName + "/";
                    else
                        newFileName = folderName + "/";
                    await App.Repository.PutObjectFromStreamAsync(_openedBucket.BucketName, newFileName, stream, stream.Length);
                }
                fileStream.Dispose();
                if (_openedHistory.Count > 0)
                    GoIntoPath(_openedBucket, _openedHistory[_openedHistory.Count - 1]);
                else
                    LoadListOfItemsInBucket(_openedBucket);

            } catch (Exception exe) 
            {
                DisplayWarningDialog(exe.Message);
            }

        }





        private bool _progressRing = false;
        public bool ProgressRingState
        {
            get { return _progressRing; }
            set
            {
                _progressRing = value;
                RaisePropertyChanged("ProgressRingState");
            }
        }

        private string _fileFolderName = "";
        public string FileFolderName 
        {
            get { return _fileFolderName; }
            set
            {
                _fileFolderName = value;
                RaisePropertyChanged("FileFolderName");
            }
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
        // opened state
        private MinioItemModel _openedItem;
        private MinioBucketModel _openedBucket;
        private List<MinioItemModel> _openedHistory = new List<MinioItemModel>();
        // picked bucket
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
        // picked item
        private MinioItemModel _selectedItem;
        public MinioItemModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
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


        private async void DisplayWarningDialog(string Warning)
        {
            ContentDialog noWifiDialog = new ContentDialog()
            {
                Title = "Warning",
                Content = Warning,
                CloseButtonText = "Ok"
            };

            await noWifiDialog.ShowAsync();
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
