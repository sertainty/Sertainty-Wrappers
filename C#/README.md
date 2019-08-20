# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

Sertainty SDK helps to protect and encrypt your data and share secure manner. It allows desired parties to access the data if they are authenticated from a challenge verification process.

- [Sertainty SDK](#sertainty-sdk)
  - [How to run the code](#how-to-run-the-code)
    - [Step 1: Get the Sertainty license](#step-1-get-the-sertainty-license)
    - [Step 2: Get the code](#step-2-get-the-code)
    - [Step 3: Configure development environment](#step-3-configure-development-environment)
    - [Step 4: Run the code](#step-4-run-the-code)
  - [Walkthrough](#walkthrough)
    - [Initialize sertainty](#initialize-sertainty)
    - [Generate an ID file](#generate-an-id-file)
    - [Generate UXP and protect data](#generate-uxp-and-protect-data)
      - [Generate UXP file](#generate-uxp-file)
      - [Protect data](#protect-data)
    - [Access protected data](#access-protected-data)
    - [Files](#files)

## How to run the code

### Step 1: Get the Sertainty license
You need to have the latest Sertainty Tools and the Sertainty license for running this tutorial. 

Please contact us through `tech-support@sertainty.com`

### Step 2: Get the code 
- Clone this repository with 
   `git clone https://github.com/sertainty/Sertainty-Wrappers.git`

- or Download the Zip

### Step 3: Configure development environment
We reference necessary files from Sertainty Tools.

Set the install location of the Seratainty Tools using __SERTAINTY_HOME__ system environmental variable.

`setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"`

### Step 4: Run the code
You can install [SertaintySDK](https://www.nuget.org/packages/SertaintySDK/) using one of these methods.

1. From Visual Studio 
   - Right click on your Visual Studio project in the Solution Explorer. 
   - Select Add -> Add Nuget Packages 
   - Search "SertaintySDK" and Add package for the project 

2. From Package Manager 
   
    `Install-Package SertaintySDK -Version 1.0.2`

3. From .Net CLI

    `dotnet add package SertaintySDK --version 1.0.2`

4. Use these to answer the security questions you get in the sample program. 

    `Username: SampleUser@myemail.com`

    `Challenge 1: Response 1`

    `Challenge 2: Response 2`

    `Challenge 3: Response 3`

    ...........

    `Challenge 10: Response 10`

## Walkthrough

In this example we are using Sertainty Technology to generate digital id and protect data using that id. We also demonstrate how to access protected data with authentication process. 

### Initialize sertainty
You need to initialize sertainty sdk with valid licenseFile and appKey before using it. 

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

### Generate an ID file
See the `sampleid.xml` file in the examples directory. Open this and go through it. It contains application related data and challenge questions and answers.

This is private for each user of the Sertainty SDK. You need this file to generate an digital id(.iic) for each user.

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

### Generate UXP and protect data
We'll create a file with .uxp extension that can encapsulate data in encrypted mode. This function requires two main inputs,
- Data(to be encrypted)
- ID(.iic) file

#### Generate UXP file
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

#### Protect data
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

### Access protected data
You need to pass the authorization before accessing the data. Application will provide randomize challenge questions based on the ID(.iic) file and user need to answer them within limited time.

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

### Files
We use different files to run this examples. We'll see what they are. 
- `sampleid.xml` - This file used to generate `sampleid.iic`. Contains random challenge question and answers.
- `sampleid.iic` - Act as a digital id for the user of this application. Can be shared with other parties.
- `sample.uxp` - Uxp files are protected data. In order to protect data for intended recipiant you need the recipient's `id(.iic)` file and the data to be protected. 
