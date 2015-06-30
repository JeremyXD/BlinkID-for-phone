# Table of contents

* [Windows Phone _BlinkID_ integration instructions](#intro)
* [Quick Start](#quickStart)
  * [Quick start with demo app](#quickDemo)
  * [Quick integration of _BlinkID_ into your app](#quickIntegration)
  * [Performing your first scan](#quickScan)
* [Advanced _BlinkID_ integration instructions](#advancedIntegration)
  * [Checking if _BlinkID_ is supported](#supportCheck)
  * [Embedding `RecognizerControl` into custom application page](#recognizerControl)
  * [`RecognizerControl` reference](#recognizerControlReference)
* [Recognition settings and results](#recognitionSettingsAndResults)
  * [Generic settings](#genericSettings)
  * [Scanning machine-readable travel documents](#mrtd)
* [Troubleshooting](#troubleshoot)
  * [Integration problems](#integrationTroubleshoot)
  * [SDK problems](#sdkTroubleshoot)
* [Additional info](#info)

# <a name="intro"></a> Windows Phone _BlinkID_ integration instructions

The package contains Visual Studio 2012 solution(can open in VS 2013) that contains everything you need to get you started with _BlinkID_ library. Demo project _BlinkIDDemo_ for Windows Phone 8.0 is included in solution containing the example use of _BlinkID_ library.
 
_BlinkID_ is supported on Windows Phone 8.0. Windows Phone 8.1 can be supported with minor changes and Windows Phone 10 is expected to be supported soon.
﻿
# <a name="quickStart"></a> Quick Start

## <a name="quickDemo"></a> Quick start with demo app

### In Visual Studio 2012

1. In _FILE_ menu choose _Open Project..._.
2. In _Open project_ dialog select _BlinkIDDemo.sln_ in _BlinkIDDemo_ folder.
3. Wait for project to load.

### In Visual Studio 2013

1. In _FILE_ menu choose _Open_ then _Project/Solution_.
2. In _Open project_ dialog select _BlinkIDDemo.sln_ in _BlinkIDDemo_ folder.
3. Wait for project to load.

## <a name="quickIntegration"></a> Quick integration of _BlinkID_ into your app

This works the same in both _Visual Studio 2012_ or _Visual Studio 2013_

1. In File Explorer (**NOT** in VS) copy the _WPLibDebug_ and _WPLibRelease_ folders from _BlinkID_ SDK to your project's folder
2. Copy (**INSIDE** VS) the _WPLibAssets_ folder from _BlinkID_ SDK into your project (it is important to preserve folder structure)
3. Set the properties _Build Action_ to _None_ and _Copy to Output Directory_ to _Copy if newer_ to **all** the files in _WPLibAssets_ folder
4. Right click to your project's _References_ and choose _Add Reference..._
5. Click the _Browse..._ button on the bottom
6. In _Select the files to reference..._ dialog select _BlinkID.dll_ and _Microblink.winmd_ from _WPLibDebug_ folder (you created in first step) if you want to link to debug version of _BlinkID_ library (select files from _WPLibRelease_ if you want to link to release version of _BlinkID_ library)

## <a name="quickScan"></a> Performing your first scan
1. Add `RecognizerControl` to main page of your application. Your .xaml file should contain something like these lines:

	```xml
	xmlns:UserControls="clr-namespace:Microblink.UserControls;assembly=BlinkID"
	```
	and `RecognizerControl` tag itself:
	```xml
	<UserControls:RecognizerControl x:Name="mRecognizer" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
	```
2. You should setup `RecognizerControl` in containing page constructor like this:

	```c#
	// sets license key
	// obtain your licence key at http://microblink.com/login or
	// contact us at http://help.microblink.com            
	mRecognizer.LicenseKey = "Add your licence key here";
	
	// setup array of recognizer settings
	mRecognizer.RecognizerSettings = new Microblink.IRecognizerSettings[] { new Microblink.MRTDRecognizerSettings() { DetectMRZPosition = true } };        
	
	// these three events must be handled
	mRecognizer.OnCameraError += mRecognizer_OnCameraError;            
	mRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
	mRecognizer.OnInitializationError += mRecognizer_OnInitializationError;
	```
3. You should implement `OnNavigatedTo` and `OnNavigatedFrom` of your main page to initialize and terminate `RecognizerControl` respectively so `RecognizerControl` will be initialized when the user activates the page and will terminate when the user navigates away from the page. You should do it like this:
	
	```c#
	protected override void OnNavigatedTo(NavigationEventArgs e) {
        // call default behaviour
        base.OnNavigatedTo(e);
        // initialize the recognition process
        mRecognizer.InitializeControl(this.Orientation);
    }
        
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        // terminate the recognizer
        mRecognizer.TerminateControl();
        // call default behaviour
        base.OnNavigatingFrom(e);
    }
	```
4. You should also implement `OnOrientationChanged` of your main page and forward the orientation change info to `RecognizerControl` like this:
	
	```c#
	protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
        // call default behaviour
        base.OnOrientationChanged(e);            
        // orientation change events MUST be forwarded to RecognizerControl
        mRecognizer.OnOrientationChanged(e);            
    }
	```
5. After scan finishes it will trigger `OnScanningDone` event. You can obtain the scanning results by implementing the event handler something like this:
	
	```c#
	void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {
        // display results if scan was successful
        if (recognitionType == RecognitionType.SUCCESSFUL) {
            StringBuilder b = new StringBuilder();
            // process all results in result list
            foreach (var result in resultList) {
                // process all result elements
                if (result.Valid && !result.Empty) {
	                foreach (var key in result.Elements.Keys) {
	                    // append key-value pairs to StringBuilder
	                    b.Append(key);
	                    b.Append(" = ");
	                    b.Append(result.Elements[key]);
	                    b.Append("\n");
	                }
                }
            }
            // display message box with scanned results
            MessageBox.Show("Results:\n" + b.ToString());
        }
        // resume scanning
        mRecognizer.ResumeScanning();
    }
	```
	
	For more information about defining recognition settings and obtaining scan results see [Recognition settings and results](#recognitionSettingsAndResults).

# <a name="advancedIntegration"></a> Advanced _BlinkID_ integration instructions
This section will cover more advanced details in _BlinkID_ integration. First part will discuss the methods for checking whether _BlinkID_ is supported on current device. Second part will show how to embed `RecognizerControl` into custom application page. Third part is a brief `RecognizerControl` reference.
## <a name="supportCheck"></a> Checking if _BlinkID_ is supported

### _BlinkID_ requirements
Even before starting the scanning process, you should check if _BlinkID_ is supported on current device. In order to be supported, device needs to have a camera.

Windows Phone 8.0 is the minimum version on which _BlinkID_ is supported. It is supported on Windows Phone 8.1 with minor adjustments and we expect Windows Phone 10.0 to be supported soon.

Camera video preview resolution also matters. In order to perform successful scans, camera preview resolution cannot be too low. _BlinkID_ requires minimum 480p camera preview resolution in order to perform scan. It must be noted that camera preview resolution is not the same as the video record resolution, although on most devices those are the same. However, there are some devices that allow recording of HD video (720p resolution), but do not allow high enough camera preview resolution. _BlinkID_ does not work on those devices.

However, some recognizers require camera with autofocus. If you try to start recognition with those recognizers on a device that does not have camera with autofocus, you will get an error. To determine whether does recognizer require camera with autofocus or not you can call `bool requiresAutofocus()` method of `Microblink.IRecognitionSettings` interface.

## <a name="recognizerControl"></a> Embedding `RecognizerControl` into custom application page
This section will discuss how to embed `RecognizerControl` into your windows phone application page and perform scan.
Note that this example is for Windows Phone 8.0.

Here is the minimum example of integration of `RecognizerControl` as the only control in your page:

**.xaml file**

```xml
<phone:PhoneApplicationPage
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:phone="clr-namespace:Microsoft.Phone.Controls;assembly=Microsoft.Phone"
    xmlns:shell="clr-namespace:Microsoft.Phone.Shell;assembly=Microsoft.Phone"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:UserControls="clr-namespace:Microblink.UserControls;assembly=BlinkID"
    xmlns:local="clr-namespace:MyApp"
    x:Class="MyApp.MyPage"
    mc:Ignorable="d"
    FontFamily="{StaticResource PhoneFontFamilyNormal}"
    FontSize="{StaticResource PhoneFontSizeNormal}"
    Foreground="{StaticResource PhoneForegroundBrush}"
    SupportedOrientations="PortraitOrLandscape" Orientation="Portrait"
    shell:SystemTray.IsVisible="True">
    
    <Grid Background="Transparent">        
                        
        <UserControls:RecognizerControl x:Name="mRecognizer" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
                
    </Grid>

</phone:PhoneApplicationPage>
```

**.xaml.cs file**

```c#
public partial class MyPage : PhoneApplicationPage
    {
    
    private void InitializeRecognizer() {            
        // sets license key
		// obtain your licence key at http://microblink.com/login or
		// contact us at http://help.microblink.com            
		mRecognizer.LicenseKey = "Add your licence key here";
		
		// setup array of recognizer settings
		mRecognizer.RecognizerSettings = new Microblink.IRecognizerSettings[] { new Microblink.MRTDRecognizerSettings() { DetectMRZPosition = true } };        
		
		// these three events must be handled
		mRecognizer.OnCameraError += mRecognizer_OnCameraError;            
		mRecognizer.OnScanningDone += mRecognizer_OnScanningDone;
		mRecognizer.OnInitializationError += mRecognizer_OnInitializationError;
    }
    
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
    
    void mRecognizer_OnScanningDone(IList<Microblink.IRecognitionResult> resultList, RecognitionType recognitionType) {
        // display results if scan was successful
        if (recognitionType == RecognitionType.SUCCESSFUL) {
            StringBuilder b = new StringBuilder();
            // process all results in result list
            foreach (var result in resultList) {
                // process all result elements
                if (result.Valid && !result.Empty) {
	                foreach (var key in result.Elements.Keys) {
	                    // append key-value pairs to StringBuilder
	                    b.Append(key);
	                    b.Append(" = ");
	                    b.Append(result.Elements[key]);
	                    b.Append("\n");
	                }
            	}
            }
            // display message box with scanned results
            MessageBox.Show("Results:\n" + b.ToString());
        }
        // resume scanning
        mRecognizer.ResumeScanning();
    }
    
    void mRecognizer_OnCameraError(Microblink.UserControls.CameraError error) {
        MessageBox.Show("Could not initialize the camera!\nThe application will now terminate!");
        Application.Current.Terminate();
    }        
    
    public MyPage()
    {
        InitializeComponent();
        // set up recognizer
        InitializeRecognizer();
    }
   
    protected override void OnNavigatedTo(NavigationEventArgs e) {
        // call default behaviour
        base.OnNavigatedTo(e);
        // initialize the recognition process
        mRecognizer.InitializeControl(this.Orientation);
    }
    
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        // terminate the recognizer
        mRecognizer.TerminateControl();
        // call default behaviour
        base.OnNavigatingFrom(e);
    }    

    protected override void OnOrientationChanged(OrientationChangedEventArgs e) {
        // call default behaviour
        base.OnOrientationChanged(e);        
        // orientation change events MUST be forwarded to RecognizerControl
        mRecognizer.OnOrientationChanged(e);            
    }    
               
}
```

## <a name="recognizerControlReference"></a> `RecognizerControl` reference
The usage example for `RecognizerControl` is provided in `BlinkIDDemo` demo app provided with SDK. This section just gives a quick overview of `RecognizerControl's` most important features.

### <a name="recognizerControlMethods"></a> **Methods**

##### <a name="recognizerControl_InitializeControl"></a> `InitializeControl(PageOrientation)`
This method will initialize `RecognizerControl's` internal fields and will initialize camera control thread. This method must be called after all other settings are already defined, such as event handlers and recognition settings.

##### <a name="recognizerControl_ResumeScanning"></a> `ResumeScanning()`
This method resumes scanning loop. Scanning loop is usually paused when results have arrived and OnScanningDone event is called. You can also pause scanning loop by yourself by calling PauseScanning.

##### <a name="recognizerControl_PauseScanning"></a> `PauseScanning()`
This method pasuses scanning loop. Scanning loop is resumed by calling ResumeScanning.

##### <a name="recognizerControl_TerminateControl"></a> `TerminateControl()`
This method terminates `RecognizerControl` internal state, stops the recognizers and shuts down camera control thread. Call this method when you are finished with scanning and want to free resources used up by _BlinkID_ library. You can reinitialize `RecognizerControl` by calling `InitializeControl` method.

##### <a name="recognizerControl_OnOrientationChanged"></a> `OnOrientationChanged(OrientationChangedEventArgs)`
This method should be called to forward the `OnOrientationChanged` event to `RecognizerControl` so it can handle the orientation change.

##### <a name="recognizerControl_SetROI"></a> `SetROI(Windows.Foundation.Rect)`
You can use this method to define the scanning region of interest.

Region of interest is defined as `Windows.Foundation.Rect`. First parameter of rectangle is x-coordinate represented as percentage of view width, second parameter is y-coordinate represented as percentage of view height, third parameter is region width represented as percentage of view width and fourth parameter is region height represented as percentage of view height.

View width and height are defined in current context, i.e. they depend on screen orientation.

Note that scanning region of interest only reflects to native code - it does not have any impact on user interface. You are required to create a matching user interface that will visualize the same scanning region you set here.

##### <a name="recognizerControl_TransformPointCoordinateRelative"></a> `TransformPointCoordinateRelative(Point)`
Method to transform a point from a position in image to a position in a control relative to current preview size.

##### <a name="recognizerControl_TransformPointCoordinateAbsolute"></a> `TransformPointCoordinateAbsolute(Point)`
Method to transform a point from a position in image to absolute position in a control.

##### <a name="recognizerControl_TransformPointCoordinateROI"></a> `TransformPointCoordinateROI(Point)`
Method to transform a point from a position in image to a position in a control relative to Region of Interest.

### <a name="recognizerControlProperties"></a> **Properties**

##### <a name="recognizerControl_GenericRecognizerSettings"></a> `GenericRecognizerSettings`
With this property you can set the generic settings that will affect all enabled recognizers or the whole recognition process. For more information about generic settings, see [Generic settings](#genericSettings). This method must be called before `InitializeControl()`.

##### <a name="recognizerControl_RecognizerSettings"></a> `RecognizerSettings`
Array of `IRecognizerSettings` objects. Those objects will contain information about what will be scanned and how will scan be performed. For more information about recognition settings and results see [Recognition settings and results](#recognitionSettingsAndResults). This method must be called before `InitializeControl()`.

##### <a name="recognizerControl_PreviewScale"></a> `PreviewScale`
Defines the aspect mode of camera. If set to `Uniform` (default), then camera preview will be fit inside available view space. If set to `UniformToFill`, camera preview will be zoomed and cropped to use the entire view space.

##### <a name="recognizerControl_MacroMode"></a> `MacroMode`
When set to `true` camera will focus on nearer objects more. Set to `false` by default.

##### <a name="recognizerControl_IsTorchSupported"></a> `IsTorchSupported` 
This property is set to `true` if camera supports torch flash mode. Note that if camera is not loaded it will be set to `false`.

##### <a name="recognizerControl_TorchOn"></a> `TorchOn` 
If torch flash mode is supported on camera, this property can be used to enable/disable torch flash mode. When set to `true` torch is turned on. Note that camera has to be loaded for this to work.

##### <a name="recognizerControl_LicenseKey"></a> `Licensee` and `LicenseKey`
With these properties you can set the license key for _BlinkID_. You can obtain your license key from [Microblink website](http://microblink.com/login) or you can contact us at [http://help.microblink.com](http://help.microblink.com).
Once you obtain a license key, you can set it with following snippet:

```c#
// set the license key
mRecognizer.LicenseKey = "Enter_License_Key_Here";
```

License key is bound to your application ID. For example, if you have license key that is bound to `BlinkIDDemo` application ID, you cannot use the same key in other applications. However, if you purchase Premium license, you will get license key that can be used in multiple applications. This license key will then not be bound to application ID. Instead, it will be bound to the licensee string that needs to be provided to the library together with the license key. To provide licensee string, use something like this:

```c#
// set the license key
mRecognizer.Licensee = "Enter_Licensee_Here";
mRecognizer.LicenseKey = "Enter_License_Key_Here";
```

### <a name="recognizerControlEvents"></a> **Events**

##### <a name="recognizerControl_OnAutofocusFailed"></a> `OnAutofocusFailed` 
This event is raised when camera fails to perform autofocus even after multiple attempts. You should inform the user to try using camera under different light.

##### <a name="recognizerControl_OnCameraError"></a> `OnCameraError`
This event is triggered on camera related errors. This event **must** be handled or the _BlinkID_ library will throw an exception. Camera errors come in four different types:

*`NoCameraAtSelectedSensorLocation` - There is no camera at selected location(front or right)
*`CameraNotReady` - Camera is not ready
*`PreviewSizeTooSmall` - Camera preview size is smaller than required
*`NotSupported` - Required camera is not supported

##### <a name="recognizerControl_OnScanningDone"></a> `OnScanningDone` 
This event is raised when scanning finishes and scan data is ready. This event **must** be handled or the _BlinkID_ library will throw an exception. After recognition completes, `RecognizerControl` will pause its scanning loop and to continue the scanning you will have to call `ResumeScanning` method.

##### <a name="recognizerControl_OnInitializationError"></a> `OnInitializationError` 
This event is raised when an error occurs during RecognizerControl initialization. At the moment only licensing errors trigger this event. This event **must** be handled or the _BlinkID_ library will throw an exception.
        
##### <a name="recognizerControl_OnControlInitialized"></a> `OnControlInitialized` 
Triggered after canvas is initialized and camera is ready for receiving frames.

##### <a name="recognizerControl_OnSuccessfulScanImage"></a> `OnSuccessfulScanImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return images that resulted in a successful scan. Please take notice that installing this listener introduces a large performance penalty.

##### <a name="recognizerControl_OnOriginalImage"></a> `OnOriginalImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return original images passed to recognition process. Please take notice that installing this listener introduces a large performance penalty.

##### <a name="recognizerControl_OnDewarpedImage"></a> `OnDewarpedImage` 
Handle this event to obtain images that are currently being processed by the native library. This event will return dewarped images from the recognition process. Please take notice that installing this listener introduces a large performance penalty.

##### <a name="recognizerControl_OnDisplayDebugStatus"></a> `OnDisplayDebugStatus` 
This event is raised when recognizer control wants to display some debug status. Useful for finding bugs in code.

##### <a name="recognizerControl_OnDisplayMessage"></a> `OnDisplayMessage` 
This event is rased when recognizer wants to display some message.

##### <a name="recognizerControl_OnDisplayDefaultTarget"></a> `OnDisplayDefaultTarget` 
This event is raised when recognizer wants to put viewfinder in its default position (for example if detection has failed).

##### <a name="recognizerControl_OnDisplayNewTarget"></a> `OnDisplayNewTarget` 
This event is raised when recognizer detects an object and wants to put viewfinder in position of detected object.

##### <a name="recognizerControl_OnDisplayOcrResult"></a> `OnDisplayOcrResult` 
This event is raised when recognizer has OCR support enabled and has OCR result ready for displaying / using. You can handle this event to display real-time OCR results in preview.

##### <a name="recognizerControl_OnDisplayPointSet"></a> `OnDisplayPointSet` 
This event is raised when recognizer detects a non-rectangular object (e.g. 1D barcode, QR code, etc), instead of raising `OnDisplayNewTarget`. Handler will be provided with a list of detected object's feature points (in image coordinates). You can handle this event to display real-time detected object's feature points in preview.
        
Image coordinates refer to coordinates in video frame that has been analyzed. Usually the video frame is in landscape right mode, i.e. (0,0) represents the upper right corner and x coordinate rises downwards and y coordinate rises leftward.
        
##### <a name="recognizerControl_OnShakingStartedEvent"></a> `OnShakingStartedEvent` 
Event is triggered when device shaking starts.
# <a name="recognitionSettingsAndResults"></a> Recognition settings and results

This chapter will discuss various recognition settings used to configure different recognizers and scan results generated by them.

## <a name="genericSettings"></a> Generic settings

Generic settings affect all enabled recognizers and the whole recognition process. Here is the list of properties that are most relevant:

##### `bool AllowMultipleScanResults`
Sets whether or not outputting of multiple scan results from same image is allowed. If that is `true`, it is possible to return multiple recognition results from same image. By default, this option is `false`, i.e. the array of `BaseRecognitionResults` will contain at most 1 element. The upside of setting that option to `false` is the speed - if you enable lots of recognizers, as soon as the first recognizer succeeds in scanning, recognition chain will be terminated and other recognizers will not get a chance to analyze the image. The downside is that you are then unable to obtain multiple results from single image.

##### `int RecognitionTimeout`
Number of miliseconds _BlinkID_ will attempt to perform the scan before it exits with timeout error. On timeout returned array of `IRecognitionResults` might be null, empty or may contain only elements that are not valid (`Valid` is `false`) or are empty (`Empty` is `true`).
## <a name="mrtd"></a> Scanning machine-readable travel documents

This section discusses the setting up of machine-readable travel documents(MRTD) recognizer and obtaining results from it.

### Setting up machine-readable travel documents recognizer

To activate MRTD recognizer, you need to create `MRTDRecognizerSettings` and add it to `IRecognizerSettings` array. You can use the following code snippet to perform that:

```c#
using Microblink;

private IRecognizerSettings[] setupSettingsArray() {
	MRTDRecognizerSettings sett = new MRTDRecognizerSettings();
	
	// enable detection of MRZ position
	sett.DetectMRZPosition = true;
	
	// now add sett to recognizer settings array that is used to configure
	// recognition
	return new IRecognizerSettings[] { sett };
}
```

### Obtaining results from machine-readable travel documents recognizer

MRTD recognizer produces `MRTDRecognitionResult`. You can use `is` operator to check if element in results array is instance of `MRTDRecognitionResult` class. See the following snippet for an example:

```c#
using Microblink;

public void OnScanningDone(IList<IRecognitionResult> resultList, RecognitionType recognitionType) {   
    if (recognitionType == RecognitionType.SUCCESSFUL) {        
        foreach (var result in resultList) {            
            if (result is MRTDRecognitionResult) {                
                MRTDRecognitionResult mrtdResult = (MRTDRecognitionResult)result;
                // you can use properties of MRTDRecognitionResult class to 
		        // obtain scanned information
		        if(result.Valid && !result.Empty) {
		           string primaryId = result.PrimaryID;
		           string secondaryId = result.SecondaryID;
		           string documentNumber = result.DocumentNumber;          		 
		        } else {
		        	// not all relevant data was scanned, ask user
		        	// to try again
		        }               
            }
        }                 
    }
}
```

Available properties are:

##### `bool Valid`
Set to `true` if scan result is valid, i.e. if all required elements were scanned with good confidence and can be used. If `false` is returned that indicates that some crucial data fields are missing. You should ask user to try scanning again. If you keep getting `false` (i.e. invalid data) for certain document, please report that as a bug to [help.microblink.com](http://help.microblink.com). Please include high resolution photographs of problematic documents.

##### `bool Empty`
Set to `true` if scan result is empty, i.e. nothing was scanned. All getters should return `null` for empty result.

##### `string PrimaryID`
Set to the primary indentifier. If there is more than one component, they are separated with space.

##### `string SecondaryID`
Set to the secondary identifier. If there is more than one component, they are separated with space.

##### `string Issuer`
Set to three-letter or two-letter code which indicate the issuing State. Three-letter codes are based on `Aplha-3` codes for entities specified in `ISO 3166-1`, with extensions for certain States. Two-letter codes are based on `Aplha-2` codes for entities specified in `ISO 3166-1`, with extensions for certain States.

##### `string DateOfBirth`
Set to holder's date of birth in format `YYMMDD`.

##### `string DocumentNumber`
Set to document number. Document number contains up to 9 characters.

##### `string Nationality`
Set to nationality of the holder represented by a three-letter or two-letter code. Three-letter codes are based on `Alpha-3` codes for entities specified in `ISO 3166-1`, with extensions for certain States. Two-letter codes are based on `Aplha-2` codes for entities specified in `ISO 3166-1`, with extensions for certain States.

##### `string Sex`
Set to sex of the card holder. Sex is specified by use of the single initial, capital letter `F` for female, `M` for male or `<` for unspecified.

##### `string DocumentCode`
Set to document code. Document code contains two characters. For `MRTD` the first character shall be `A`, `C` or `I`. The second character shall be discretion of the issuing State or organization except that V shall not be used, and `C` shall not be used after `A` except in the crew member certificate. On machine-readable passports `(MRP)` first character shall be `P` to designate an `MRP`. One additional letter may be used, at the discretion of the issuing State or organization, to designate a particular `MRP`. If the second character position is not used for this purpose, it shall be filled by the filter character `<`.

##### `string DateOfExpiry`
Set to date of expiry of the document in format `YYMMDD`.

##### `string Optional1`
Set to first optional data. Set to `null` or empty string if not available.

##### `string Optional2`
Set to second optional data. Set to `null` or empty string if not available.

##### `string MRZText`
Set to the entire Machine Readable Zone text from ID. This text is usually used for parsing other elements.

# <a name="troubleshoot"></a> Troubleshooting

## <a name="integrationTroubleshoot"></a> Integration problems

If you have followed [Windows Phone integration instructions](#quickIntegration) and are still having integration problems, please contact us at [help.microblink.com](http://help.microblink.com).

## <a name="sdkTroubleshoot"></a> SDK problems

In case of problems with using the SDK, you should do as follows:

### Licensing problems

If you are getting "invalid license key" error or having other license-related problems (e.g. some feature is not enabled that should be or there is a watermark on top of camera), first check the log. All license-related problems are logged to error log so it is easy to determine what went wrong.

When you have determined what is the license-related problem or you simply do not understand the log, you should contact us [help.microblink.com](http://help.microblink.com). When contacting us, please make sure you provide following information:

* exact ID of your app (`Product ID` from your `WMAppManifest.xml` file)
* license key that is causing problems
* please stress out that you are reporting problem related to Windows Phone version of _BlinkID_ SDK
* if unsure about the problem, you should also provide excerpt from log containing license error

### Other problems

If you are having problems with scanning certain items, undesired behaviour on specific device(s), crashes inside _BlinkID_ or anything unmentioned, please do as follows:

* enable logging to get the ability to see what is library doing. To enable logging, put this line in your application:

	```c#
	Microblink.Log.Level = Microblink.Log.LogLevel.Verbose;	
	```

	After this line, library will display as much information about its work as possible. Please save the entire log of scanning session to a file that you will send to us. It is important to send the entire log, not just the part where crash occured, because crashes are sometimes caused by unexpected behaviour in the early stage of the library initialization.
	
* Contact us at [help.microblink.com](http://help.microblink.com) describing your problem and provide following information:
	* log file obtained in previous step
	* high resolution scan/photo of the item that you are trying to scan
	* information about device that you are using - we need exact model name of the device. 
	* please stress out that you are reporting problem related to Windows Phone version of _BlinkID_ SDK


# <a name="info"></a> Additional info
For any other questions, feel free to contact us at [help.microblink.com](http://help.microblink.com).