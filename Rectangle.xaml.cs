using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microblink;

namespace BlinkIDDemo
{
    /// <summary>
    /// Control for displaying target rectangle
    /// </summary>
    public partial class Rectangle : UserControl
    {
        // rectangle width to height aspect ratio
        private const double kAspectRatio = 0.63060747663;
        // rectangle to screen width ratio in portrait orientation
        private const double kPortraitRatio = 0.9;
        // rectangle to screen width ratio in landscape orientation
        private const double kLandscapeRatio = 0.6;

        // rectangle width in portrait orientation
        double mPortraitWidth;
        // rectangle height in portrait orientation
        double mPortraitHeight;
        // rectangle width in landscape orientation
        double mLandscapeWidth;
        // rectangle height in landscape orientation
        double mLandscapeHeight;
        // have all widths and heights been initialized?
        bool mAnimationInitialized = false;

        /// <summary>
        /// Calculates initial rectangle dimensions (in portrait)
        /// </summary>
        public void InitBoxSize() {       
            // calculate portrait box size based on screen size
            mPortraitWidth = ActualWidth * kPortraitRatio;
            mPortraitHeight = mPortraitWidth * kAspectRatio;
            
            // set initial box size (portrait assumed)
            mBox.Width = mPortraitWidth;
            mBox.Height = mPortraitHeight;                      
        }

        /// <summary>
        /// Default constructor
        /// Sets loaded event handler
        /// </summary>
        public Rectangle() {
            // handle loaded event
            Loaded += Rectangle_Loaded;          
            InitializeComponent();            
        }

        /// <summary>
        /// Handles loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Rectangle_Loaded(object sender, RoutedEventArgs e) {
            // screen dimensions are known so we calculate initial rectangle size
            InitBoxSize();
        }

        /// <summary>
        /// Animates the rectangle on orientation change
        /// </summary>
        /// <param name="e"></param>
        public void AnimateOrientationChange(OrientationChangedEventArgs e) {            
            // check if new orientation is landscape or portrait
            if (e.Orientation == PageOrientation.Landscape || e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight) {
                // if landscape dimensions were not calculated we must do it now
                if (!mAnimationInitialized) {
                    // calculate landscape box size based on screen size
                    mLandscapeWidth = ActualWidth * kLandscapeRatio;
                    mLandscapeHeight = mLandscapeWidth * kAspectRatio;

                    // set animation values for both animations (landcape to portrait and reverse)
                    mP2LWidthAnimation.From = mPortraitHeight;
                    mP2LWidthAnimation.To = mLandscapeWidth;

                    mP2LHeightAnimation.From = mPortraitWidth;
                    mP2LHeightAnimation.To = mLandscapeHeight;

                    mL2PWidthAnimation.From = mLandscapeHeight;
                    mL2PWidthAnimation.To = mPortraitWidth;

                    mL2PHeightAnimation.From = mLandscapeWidth;
                    mL2PHeightAnimation.To = mPortraitHeight;
                    // mark animation as initialized and ready
                    mAnimationInitialized = true;
                }
                // start portrait to landscape animation
                mP2LAnimation.Begin();
            } else {
                // start landscape to portrait animation if all values are calculated
                if (mAnimationInitialized) {
                    mL2PAnimation.Begin();
                }
            }
        }        
        
    }
}