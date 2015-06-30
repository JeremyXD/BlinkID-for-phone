using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BlinkIDDemo
{
    /// <summary>
    /// Page that displays recognition results
    /// </summary>
    public partial class ResultsPage : PhoneApplicationPage
    {

        /// <summary>
        /// static field for sending result data to the page
        /// </summary>
        public static Microblink.MRTDRecognitionResult results = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ResultsPage() {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the button click event by
        /// navigating back to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) {
            // navigate back to main page
            NavigationService.GoBack();
        }

        /// <summary>
        /// Called when this page is navigated to.
        /// Fills out form fields with recognition results.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            // call default behaviour
            base.OnNavigatedTo(e);
            // if results have been passed copy them to form fields
            if (results != null) {
                mLastnameBox.Text = results.PrimaryID;
                mFirstnameBox.Text = results.SecondaryID;
                mSexBox.Text = results.Sex;
                mDateOfBirthBox.Text = results.DateOfBirth;
                mNationalityBox.Text = results.Nationality;
                mDocumentCodeBox.Text = results.DocumentCode;
                mDocumentNumberBox.Text = results.DocumentNumber;
                mIssuerBox.Text = results.Issuer;
                mDateOfExpiryBox.Text = results.DateOfExpiry;
                mOptional1Box.Text = results.Optional1;
                mOptional2Box.Text = results.Optional2;
                mRawBox.Text = results.MRZText;
                // invalidate results
                results = null;
            } else {
                // clear form fields
                mLastnameBox.Text = "";
                mFirstnameBox.Text = "";                
                mSexBox.Text = "";
                mDateOfBirthBox.Text = "";
                mNationalityBox.Text = "";
                mDocumentCodeBox.Text = "";
                mDocumentNumberBox.Text = "";
                mIssuerBox.Text = "";
                mDateOfExpiryBox.Text = "";
                mOptional1Box.Text = "";
                mOptional2Box.Text = "";
                mRawBox.Text = "";
            }
        }

    }
}