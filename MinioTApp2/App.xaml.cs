using MinioTApp2.ViewModel.ViewModels;
using MinioTApp2.Repository.Repository;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MinioTApp2
{
    // Provides application-specific behavior to supplement the default Application class.
    sealed partial class App : Application
    {
        // Gets the app-wide MainViewModel singleton instance.
        public static MainWindowVM ViewModel { get; private set; }


        // Pipeline for interacting with backend service or database.
        public static MinioRepository Repository { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }


        /*
         Call when user open app usually. When user launch app not usually like dropping file - use other entry point.
         <param name="e">Launch parameters.</param>
        */
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previous launched app
                }

                Window.Current.Content = rootFrame;
            }
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Обеспечение активности текущего окна
                Window.Current.Activate();
            }


        }


        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);

            ViewModel = new MainWindowVM();
            Repository = new MinioRepository();


        }

        // Call when error ocurrs while navigation between pages
        /// <param name="sender">Error occurrs here Frame</param>
        /// <param name="e">Error Info</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // Call when app paused
        /// <param name="sender">Pause call Source.</param>
        /// <param name="e">Pause call info.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save app state and stop operations
            deferral.Complete();
        }
    }
}
