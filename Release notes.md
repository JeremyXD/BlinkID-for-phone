# Release notes


## 1.1.0

- EUDL recognition added to demo application
- initial implementation of OCRDisplayControl
- OCR results implemented 
- MyKad recognition added to demo application
- Fixed RecognizerControl scaling & ROI calculation
- Fixed bug in DirectX shaders that severely impaired OCR detection
- Fixed ProductID crash when developing on Windows Phone 10
- Introduced FaceDetector and automated country detection for EUDL recognizer
- Added detector API and DetectorRecognizer
- Introduced EUDLRecognizer which reads German and UK driver's licenses
- Removed UKDLRecognizer
- Improved document detection algorithm
- MRTD recognizer optimized for speed
- Better handling of FullName, FullAddress, Height and Weight of cardholder
- Reading Malaysian ID - MyKad

## 1.0.0

- improved performance and quality of United Kingdom's Driver's Licence scanning
- improved OCR quality when scanning documents with machine readable zone
- improved USDL barcode parsing
- fixed crash in USDL parser
- support for obtaining raw MRZ text even if our internal parser fails to parse it
- in order to do that, you must enable returning of unparsed MRTD results
- when obtaining MRTD recognition result, you should check if MRZ was parsed
- if MRZ was parsed, you can obtain parsed data as ususal
- if MRZ was not parsed, you can obtain raw OCR result and parse it yourself
- added support for Visa MRZ format in internal MRZ parser
- support for obtaining image of MRZ zone and full document image via ImageListener
- implemented DirectAPI for recognition of individual bitmaps
- added BlinkIDDirectAPIDemo sample app
- bitmaps can now be processed while RecognizerControl is active using method Recognize
- by default, null quiet zone is now set to true in US Driver's License recognizer
- optimized MRZ text extraction algorithm - 0-O confusion now does not cause extremely long processing that resulted in freezing the device

## 0.8.0

- Initial version for Windows Phone 8.0