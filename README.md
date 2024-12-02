# Ara 3D Revit Sample Browser

The Ara3D Revit Sample Browser is a plug-in for Revit 2024 that provides a Windows Forms UI to browse the documentation and activate most of the Revit SDK C# samples for Revit 2024. 
The code for the samples was downloaded from [Jeremy Tammik's Revit SDK Samples](https://github.com/jeremytammik/RevitSdkSamples), then merged together in a single project
with some code style changes applied via ReSharper, and some regular expression magic.   

![image](https://github.com/ara3d/revit-sample-browser/assets/1759994/0b563e02-9cfe-445c-8f07-c38c0053aea2)

## Motivation 

The Revit API is very complete and ships with roughly 200 samples. I wanted a tool which would make it easier to browse the documentation of the different samples,
try them out, and to debug them. I also wanted a single project that I could use for testing Bowerbird, a C# scripting 
solution for multiple host applications that is being built by [Ara 3D](https://ara3d.com).  

As I developed the Revit Sample browser I realized that it could help others who are learning the Revit API. It also helped me uncover some areas of improvement 
for the samples. The samples were built over the last 15+ years, and use an outdated coding style. 

I took the liberty of refactoring many of the samples using more modern C# idioms to make the code 
succinct and easier to read, modify, and understand. 

## Status

This project is in an alpha (pre-Beta) state. We are approaching feature complete, but would greatly appreciate feedback on it. 
There are a lot of samples to test. 

## Known Issues

Currently any sample that requires an external file, is using the wrong path. 

## How to Build and Compile 

The project runs a batch script after build that copies the add-in file and the DLLs into the Revit 2024 add-ins directory.
Just launching Revit 2024 afterwards should work. 

## How to Use

The Ara 3D Revit Sample browser plug-in is exposed on the Ribbon an "external tool". 
It assumes that you compiled the project locally from source code, and uses that to find the ReadMe files.  

![image](https://github.com/ara3d/revit-sample-browser/assets/1759994/aef71eab-555c-4a96-86c9-916bd20ccc02)

After launching the plug-in, select a sample to see the documentation, and double-click it to launch: 

![image](https://github.com/ara3d/revit-sample-browser/assets/1759994/f3261c95-d7d2-4431-addc-3968cf0b7a30)

## How to Contribute 

- If you have a suggested code fix or improvement please make a [pull request](https://github.com/ara3d/revit-sample-browser/pulls). 
- If you have comments or questions please file an issue. 
- If you find the tool useful, please tell others about it.

# Guidelines

Remember, open-source software is a generous gift from people who work extremely hard for very little reward. 
Be patient, kind, and supportive!
