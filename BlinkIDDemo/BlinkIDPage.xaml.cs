﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BlinkIDDemo.Resources;
using System.Text;
using Microblink.UserControls;
using System.Windows.Media.Imaging;
using Microblink;

namespace BlinkIDDemo
{
    /// <summary>
    /// Main application page containing RecognizerControl
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {

        /// <summary>
        /// Initializes the RecognizerControl
        /// </summary>
        private void InitializeRecognizer() {
            // sets license key
            // obtain your licence key at http://microblink.com/login or
            // contact us at http://help.microblink.com            
            mRecognizer.LicenseKey = "Add license key here"; 

            // add MRTD recognizer settings
            mRecognizer.RecognizerSettings = new Microblink.IRecognizerSettings[] { new Microblink.MRTDRecognizerSettings(), 
                new Microblink.EUDLRecognizerSettings(EUDLCountry.EUDL_COUNTRY_AUTO),
                new Microblink.MyKadRecognizerSettings() };

            // set camera preview scale
            mRecognizer.PreviewScale = CameraPreviewScale.UniformToFill;

            // these three events must be handled
            mRecognizer.OnCameraError += mRecognizer_OnCameraError;            
            mRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
            mRecognizer.OnInitializationError += mRecognizer_OnInitializationError;

            // enable display of OCR results
            mRecognizer.OnDisplayOcrResult += mRecognizer_OnDisplayOcrResult;
        }

        void mRecognizer_OnDisplayOcrResult(Microblink.OCRResult result) {
            mOCRDisplay.DisplayOCRResult(result);
        }        

        /// <summary>
        /// Handles initialization error(invalid license)
        /// </summary>
        /// <param name="errorType">initialization error type(only INVALID_LICENSE_KEY at the moment)</param>
        void mRecognizer_OnInitializationError(InitializationErrorType errorType) {
            // handle licensing error by displaying error message and terminating the application
            if (errorType == InitializationErrorType.INVALID_LICENSE_KEY) {
                MessageBox.Show("Could not unlock API! Invalid license key!\nThe application will now terminate!");
                Application.Current.Terminate();
            } else {
                // there are no other error types
                throw new NotImplementedException();
            }           
        }

        /// <summary>
        /// Handles completed scanning events.
        /// Navigates to results page if scanning was successful.
        /// </summary>
        /// <param name="resultList">list of recognition results</param>
        /// <param name="recognitionType">type of recognition</param>
        void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {
            // navigate to results page if type of recognition is SUCCESSFUL
            if (recognitionType == RecognitionType.SUCCESSFUL) {
                // Find MRTD result in list of results. Should be the only result in the list.
                bool resultFound = false;
                foreach (var result in resultList) {
                    if (result.Valid && !result.Empty) {
                        // check if result is a MRTD result
                        if (result is Microblink.MRTDRecognitionResult) {
                            // obtain the MRTD result
                            Microblink.MRTDRecognitionResult mrtdResult = (Microblink.MRTDRecognitionResult)result;
                            // set it as input for results page
                            ResultsPage.results = mrtdResult.Elements;
                            ResultsPage.resultsType = "MRTD";
                            // mark as found
                            resultFound = true;
                            break;
                        }
                            // check if result is a MyKad result
                        else if (result is Microblink.MyKadRecognitionResult) {
                            // obtain the MyKad result
                            Microblink.MyKadRecognitionResult mykadResult = (Microblink.MyKadRecognitionResult)result;
                            // set it as input for results page
                            ResultsPage.results = mykadResult.Elements;
                            ResultsPage.resultsType = "MyKad";
                            // mark as found
                            resultFound = true;
                            break;
                        }
                            // check if result is a EUDL result
                        else if (result is Microblink.EUDLRecognitionResult) {
                            // obtain the EUDL result
                            Microblink.EUDLRecognitionResult eudlResult = (Microblink.EUDLRecognitionResult)result;
                            // set it as input for results page
                            ResultsPage.results = eudlResult.Elements;
                            ResultsPage.resultsType = "EUDL";
                            // mark as found
                            resultFound = true;
                            break;
                        }
                    }
                }
                // navigate to results page if MRTD result was found
                if (resultFound) {
                    NavigationService.Navigate(new Uri("/ResultsPage.xaml", UriKind.Relative));
                }
            }
        }

        /// <summary>
        /// Here camera errors should be handled.
        /// </summary>
        /// <param name="error"></param>
        void mRecognizer_OnCameraError(Microblink.UserControls.CameraError error) {
            // just throw an exception
            throw new NotImplementedException();
        }        

        /// <summary>
        /// Default construtor.
        /// Initializes component(page) and RecognizerControl
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            // set up recognizer
            InitializeRecognizer();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        /// <summary>
        /// Called when this page is navigated to.
        /// Starts the recognition process.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            // call default behaviour
            base.OnNavigatedTo(e);
            // initialize the recognition process
            mRecognizer.InitializeControl(this.Orientation);
        }

        /// <summary>
        /// Called when the user leaves this page.
        /// Stops the recognition process.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            // terminate the recognizer
            mRecognizer.TerminateControl();
            // call default behaviour
            base.OnNavigatingFrom(e);
        }

        /// <summary>
        /// Animate rotation of "Cancel" and "Light" buttons
        /// on orientation change.
        /// </summary>
        /// <param name="e">orientation change event info</param>
        private void AnimateButtons(OrientationChangedEventArgs e) {
            // check if a new orientation is landscape or portrait
            if (e.Orientation == PageOrientation.Landscape || e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight) {
                // align "Light" button to the left
                mLightButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                // align "Cancel" button to the bottom
                mCancelButton.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                // start the portrait to landscape animation
                mP2LAnimation.Begin();
            } else {
                // align "Light" button to the right
                mLightButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                // align "Cancel" button to the top
                mCancelButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                // start the landscape to portrait animation
                mL2PAnimation.Begin();
            }
        }

        /// <summary>
        /// Handles orientation change event by animating
        /// buttons and rectangle and forwarding the event
        /// to RecognizerControl
        /// </summary>
        /// <param name="e">orientation change event info</param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
            // call default behaviour
            base.OnOrientationChanged(e);
            // animate "Cancel" and "Light" buttons
            AnimateButtons(e);
            // animate rectangle 
            mRectangle.AnimateOrientationChange(e);
            // orientation change events MUST be forwarded to RecognizerControl
            mRecognizer.OnOrientationChanged(e);            
        }

        /// <summary>
        /// Handles "Light" button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mLightButton_Click(object sender, RoutedEventArgs e) {
            // check if the camera light is available
            if (mRecognizer.IsTorchSupported) {
                // toggle camera light
                mRecognizer.TorchOn = !mRecognizer.TorchOn;
                // toggle "Light" button icon
                if (mRecognizer.TorchOn) {                   
                    mLightImage.ImageSource = new BitmapImage(new Uri("Assets/Icons/icon_flashlight_selected.png", UriKind.Relative));
                } else {
                    mLightImage.ImageSource = new BitmapImage(new Uri("Assets/Icons/icon_flashlight.png", UriKind.Relative));
                }
            } else {
                // if camera light is not available display message
                MessageBox.Show("Camera light is not supported on this device!");
            }
        }

        /// <summary>
        /// Sets up "Light" button rotation center to be at
        /// the center of the button. We must do it at 
        /// button loaded event when button dimensions are known.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mLightButton_Loaded(object sender, RoutedEventArgs e) {
            mLightButtonRotation.CenterX = mLightButton.ActualWidth / 2.0;
            mLightButtonRotation.CenterY = mLightButton.ActualHeight / 2.0;
        }

        /// <summary>
        /// Sets up "Cancel" button rotation center to be at
        /// the center of the button. We must do it at 
        /// button loaded event when button dimensions are known.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCancelButton_Loaded(object sender, RoutedEventArgs e) {
            mCancelButtonRotation.CenterX = mCancelButton.ActualWidth / 2.0;
            mCancelButtonRotation.CenterY = mCancelButton.ActualHeight / 2.0;
        }

        /// <summary>
        /// Handles "Cancel" button click event by
        /// terminating RecognizerControl and shuting down
        /// the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCancelButton_Click(object sender, RoutedEventArgs e) {
            // terminate the recognizer
            mRecognizer.TerminateControl();
            // terminate the application
            Application.Current.Terminate();
        }               
                    
    }
}