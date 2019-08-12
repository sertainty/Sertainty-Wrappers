# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

- [Getting Started with Sertainty](#getting-started-with-sertainty)
    - [Installing](#installing)
    - [License](#license)
    - [Usage](#usage)
    - [Tutorial](#tutorial)
- [API reference](#api-reference)

## Getting Started with Sertainty
This is the official C# SDK for Sertainty; Which provides functionality for generating ID files and protect data and control access.

#### Installing

- __Prerequisites__
  
  Install Sertainty Tools for Windows - [Contact US](https://www.sertainty.com/)

-  __Install Sertainty SDK__

    You can install [SertaintySDK](https://www.nuget.org/packages/SertaintySDK/) using one of these methods. 

    1. From Visual Studio 
       - Right click on your Visual Studio project in the Solution Explorer. 
       - Select Add -> Add Nuget Packages 
       - Search "SertaintySDK" and Add package for the project 

    2. From Package Manager 
   
       `Install-Package SertaintySDK -Version 1.0.2`

    3. From .Net CLI

       `dotnet add package SertaintySDK --version 1.0.2`

    4. Add Package Reference (For projects that support PackageReference)

       `<PackageReference Include="SertaintySDK" Version="1.0.2" />`

    

-  __Set `SERTAINTY_HOME` environment variable__
  
    We reference necessary files from Sertainty Tools hence you need to set the install location of the Seratainty Tools using system environmental variable.

    you can set the `SERTAINTY_HOME` with Command Prompt (run as Administrator).
    
    `setx SERTAINTY_HOME /M "path\to\sertainty"`
  
    Example: `setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"` 

### License
You will be able to Install the License with Sertainty Tools or try it free.

### Usage
```csharp
/* reference the sdk */
using SertaintySDK;
```

### Tutorial
We will guide you through how to use Sertainty in your project. You will learn,
  - [Step 1: Initialize Sertainty](#Initialize-Sertainty)
  - [Step 2: Generate an ID file](#Generate-an-ID-file)
  - [Step 3: Protect file with UXP](#Protect-file-with-UXP)
    - [Step 3.1: Create UXP file](#Create-UXP-file)
    - [Step 3.2: Protect data](#Protect-data)
  - [Step 4: Authorize UXP](#Authorize-UXP)
    - [Step 4.1: Handle authorization](#Handle-authorization)
    - [Step 4.2: Handle challange response](#Handle-challange-response)
  - [Step 5: Read UXP file](#Read-UXP-file)
  - [Step 6: Handle Errors](#Handle-Errors)

Create sample project and import Sertainty. (Please refer [Usage](#Usage) section)

See the full example [here.](https://github.com/sertainty/Sertainty-Wrappers/blob/master/C%23/examples/sampleProgram.cs)

Get the sample files we are using for the tutorial in [examples.](https://github.com/sertainty/Sertainty-Wrappers/tree/master/C%23/examples)

#### Initialize Sertainty 
```csharp
IntPtr bufferHandle = SertaintyCore.uxpba_newHandle();

string licenseFile = "sertainty.lic";
string appKey = "SertaintyONE";
string logPrefix = "sertainty-tutorial";
string logVersion = "1.0";

long status = SertaintyCore.uxpsys_initLibrary(bufferHandle, args.LongLength, args, licenseFile, appKey, logPrefix, logVersion);

/* make sure if Sertainty initialized correctly */
if (status == 0)
{
  errorstr = SertaintyCore.uxpba_getData(bufferHandle).ReadString();
  Console.WriteLine("Error initializing environment: {0}", errorstr);
}

Console.WriteLine("Sertainty initialized successfully");
```

#### Generate an ID file
To generate ID (.iic) file, we need a XML version of ID spec which can generate from Sertainty application. 
```csharp
/* XML id file generated from the sertainty application */
string idXmlSpec = "sampleid.xml";
string idFileSpec = "sampleid.iic";

IntPtr callStatusHandle = SertaintyCore.uxpsys_newCallStatusHandle();

SertaintyCore.uxpsys_fileReadAll(callStatusHandle, idXmlSpec, bufferHandle);

/* check errors in last `uxpsys_fileReadAll` function call */
if (SertaintyCore.uxpsys_hasError(callStatusHandle))
{
    IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(callStatusHandle);
    string errMsg = errMsgPtr.ReadString();
    Console.WriteLine("Error reading ID: {0}", errMsg);
} 
else 
{
  /* Generate ID file */
  Console.WriteLine("{0} read", idXmlSpec);
  string doc = SertaintyCore.uxpba_getData(bufferHandle).ReadString();

  SertaintyCore.uxpid_publishToFile(callStatusHandle, idFileSpec, doc, 1);
  
  if (SertaintyCore.uxpsys_hasError(callStatusHandle))
  {
      IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(callStatusHandle);
      string errMsg = errMsgPtr.ReadString();
      Console.WriteLine("Error creating ID: {0}", errMsg);
  }

  Console.WriteLine("{0} created", idFileSpec);
}
```

#### Protect file with UXP
We'll create a file with .uxp extension that can encapsulate data in encrypted mode. This function requires two main inputs,
- Data(to be encrypted)
- ID(.iic) file

##### Create UXP file
```csharp
/* a file to be encrypted */
string dataPdfSpec = "data.pdf";
/* output file name */
string uxpFileSpec = "sample.uxp";


IntPtr appHandle = SertaintyCore.uxpfile_newHandle();
SertaintyCore.uxpfile_openNewFile(appHandle, uxpFileSpec, idFileSpec, 3, 1, 0);


if (SertaintyCore.uxpsys_hasError(appHandle))
{
    IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(appHandle);
    string errMsg = errMsgPtr.ReadString();
    Console.WriteLine("Error creating Data: {0}", errMsg);
} 

Console.WriteLine("{0} created", uxpFileSpec);
```

##### Protect data
```csharp
SertaintyCore.uxpfile_addVirtualFromFile(appHandle, "data.pdf", dataPdfSpec, -1, -1, 8);
  
if (SertaintyCore.uxpsys_hasError(appHandle))
{
    IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(appHandle);
    string errMsg = errMsgPtr.ReadString();
    Console.WriteLine("Error creating virtual file: {0}", errMsg);
}

Console.WriteLine("File has been Encrypted");
```

#### Authorize UXP

##### Handle authorization
```csharp
SertaintyCore.uxpfile_openFile(appHandle, uxpFileSpec, Mode.ReadOnly);

if (SertaintyCore.uxpsys_hasError(appHandle))
{
    IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(appHandle);
    string errMsg = errMsgPtr.ReadString();
    Console.WriteLine("Error opening UXP: {0}", errMsg);
}

bool done = false;
bool authorized = false;
AuthorizationStatus status;

while (!done)
{
    status = SertaintyCore.uxpfile_authenticate(appHandle);

    switch (status)
    {
        case AuthorizationStatus.Authorized:
            Console.WriteLine("You're authorized");
            done = true;
            authorized = true;
            break;
        case AuthorizationStatus.NotAuthorized:
            Console.WriteLine("You're not authorized");
            authorized = false;
            done = true;
            break;
        case AuthorizationStatus.Challenged:
            for (int i = 0; i < SertaintyCore.uxpfile_getChallengeCount(appHandle); i++)
            {
                IntPtr challengeHandle = SertaintyCore.uxpfile_getChallenge(appHandle, i);
                getResponse(challengeHandle);
                SertaintyCore.uxpfile_addResponse(appHandle, challengeHandle);
                SertaintyCore.uxpch_freeHandle(challengeHandle);
            }
            break;
        default:
            break;
    }
}
```

##### Handle challange response
```csharp
private static void getResponse(IntPtr challengeHandle)
{
    IntPtr prompt = SertaintyCore.uxpba_newHandle();
    SertaintyCore.uxpch_getPrompt(challengeHandle, prompt);
    SertaintyCore.uxpch_startTimer(challengeHandle);

    Console.WriteLine("{0}> ", SertaintyCore.uxpba_getData(prompt).ReadString());
    string value = Console.ReadLine().TrimEnd('\r', '\n');

    SertaintyCore.uxpch_endTimer(challengeHandle);
    SertaintyCore.uxpch_setValueString(challengeHandle, value);

    SertaintyCore.uxpba_freeHandle(prompt);
}
```

#### Read UXP file
```csharp
/* output file name */
string copy1Spec = "copy1.pdf";

if (authorized)
{
    Console.WriteLine("Extracting data.pdf to copy2.pdf");

    fileHandle = SertaintyCore.uxpfile_openVirtualFile(appHandle, "data.pdf", Mode.ReadOnly);

    if (SertaintyCore.uxpsys_hasError(appHandle))
    {
        IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(appHandle);
        string errMsg = errMsgPtr.ReadString();
        Console.WriteLine("Error opening virtual file: {0}", errMsg);
    }
    else
    {
        FileStream sw = new FileStream(copy2Spec, FileMode.Create);
        while (SertaintyCore.uxpfile_readVirtualFile(appHandle, fileHandle, bufferHandle, 1000) > 0)
        {
            int len = (int)SertaintyCore.uxpba_getSize(bufferHandle);
            byte[] data = SertaintyCore.uxpba_getData(bufferHandle).ReadBytes(len);
            sw.Write(data, 0, len);
        }
        sw.Close();
        SertaintyCore.uxpfile_closeVirtualFile(appHandle, fileHandle);

    }
}
```

#### Handle Errors
To check if there are any errors after each operation, we can use `uxpsys_hasError(IntPtr handle)` method. It requires a handle as an input parameter.  If the operation is file related, we need to pass a file handle, otherwies it will be a callStatusHandle or other type of hanlde. (Handle for getting status of an operation);

````csharp
/* get call status handle */

IntPtr callStatusHandle = SertaintyCore.uxpsys_newCallStatusHandle();
SertaintyCore.uxpid_publishToFile(callStatusHandle, idFileSpec, doc, 1);

/* handle errors */
if (SertaintyCore.uxpsys_hasError(callStatusHandle)) 
{
    IntPtr errMsgPtr = SertaintyCore.uxpsys_getErrorMessage(callStatusHandle);
    string errMsg = errMsgPtr.ReadString();
    Console.WriteLine("Error creating ID: {0}", errMsg);
}
````

## API reference

See the full API reference [here](). 
