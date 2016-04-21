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
        public static IReadOnlyDictionary<string, object> results = null;
        public static string resultsType = null;

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
            // clear form fields
            mContent.Children.Clear();
            // if results have been passed copy them to form fields
            if (results != null) {
                StackPanel typePanel = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, Margin = new Thickness(0, 0, 0, 40) };
                TextBlock typeLabel = new TextBlock() { Text = "Result Type: " + resultsType, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, FontSize = 27 };
                typePanel.Children.Add(typeLabel);
                mContent.Children.Add(typePanel);
                foreach (var key in results.Keys) {
                    if (key != null && results[key] != null) {
                        StackPanel panel = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, Margin = new Thickness(0, 0, 0, 20) };
                        TextBlock label = new TextBlock() { Text = key, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap };
                        TextBox textbox = new TextBox() { Text = results[key].ToString(), HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap };
                        panel.Children.Add(label);
                        panel.Children.Add(textbox);
                        mContent.Children.Add(panel);
                    }
                }
            } else {
                StackPanel typePanel = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, Margin = new Thickness(0, 0, 0, 40) };
                TextBlock typeLabel = new TextBlock() { Text = "No results found", HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, TextWrapping = TextWrapping.Wrap, FontSize = 27 };
                typePanel.Children.Add(typeLabel);
                mContent.Children.Add(typePanel);
            }
        }

    }
}