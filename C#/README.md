# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

The Sertainty SDK helps to protect, encrypt, and share your data securely. Authorized individuals may access the data if they authenticate through a challenge verification process.

- [Sertainty SDK](#sertainty-sdk)
  - [How to Run the Code](#how-to-run-the-code)
    - [Step 1: Get the Sertainty License](#step-1-get-the-sertainty-license)
    - [Step 2: Get the Code](#step-2-get-the-code)
    - [Step 3: Configure the Development Environment](#step-3-configure-the-development-environment)
    - [Step 4: Install the Sertainty SDK](#step-4-install-the-sertainty-sdk)
    - [Step 5: Run the Code](#step-5-run-the-code)
  - [Walkthrough](#walkthrough)
    - [Initialize Sertainty](#initialize-sertainty)
    - [Generate an ID File](#generate-an-id-file)
    - [Generate a UXP and Protect Data](#generate-a-uxp-and-protect-data)
    - [Access the Protected Data](#access-the-protected-data)
    - [Files](#files)

## How to Run the Code

**Note: this guide is for Windows**

### Step 1: Get the Sertainty License
This tutorial requires the latest Sertainty Tools and a Sertainty license. Please contact `tech-support@sertainty.com` for help with this.

### Step 2: Get the Code 
You may clone this repository: 
```
git clone https://github.com/sertainty/Sertainty-Wrappers.git
```

or [download it as a zip file](https://github.com/sertainty/Sertainty-Wrappers/archive/master.zip)

### Step 3: Configure the Development Environment
So that the Sertainty Tools can be found, set the install location of the Sertainty Tools using `SERTAINTY_HOME` system environment variable.

`setx SERTAINTY_HOME /M "%ProgramFiles%\Sertainty"`

### Step 4: Install the Sertainty SDK

You can install [the Sertainty SDK](https://www.nuget.org/packages/SertaintySDK/) using any of the following methods:

1. From Visual Studio:
  - Right click on your Visual Studio project in the Solution Explorer. 
  - Select Add -> Add Nuget Packages 
  - Search "SertaintySDK" and Add package for the project 

2. From Package Manager:
  - `Install-Package SertaintySDK -Version 1.0.2`

3. From .Net CLI:

  - `dotnet add package SertaintySDK --version 1.0.2`

### Step 5: Run the Code

Use the following credentials to authenticate within the sample program:

| Prompt       | Response               |
| ------------ | ---------------------- |
| Username     | sampleuser@myemail.com |
| Challenge 1  | Response 1             |
| Challenge 2  | Response 2             |
| Challenge 3  | Response 3             |
| ...          | ...                    |
| Challenge 10 | Response 10            |

## Walkthrough

In this example we are using Sertainty Technology to generate a digital id and protect data using that id. We also demonstrate how to access the protected data with the authentication process. 

### Initialize Sertainty
You need to initialize the Sertainty SDK with a valid `licenseFile` and `appKey` before using it. 

```csharp
IntPtr bufferHandle = SertaintyCore.uxpba_newHandle();

string licenseFile = "sertainty.lic";
string appKey = "SertaintyONE";
string logPrefix = "sertainty-tutorial";
string logVersion = "1.0";

long status = SertaintyCore.uxpsys_initLibrary(bufferHandle, args.LongLength, args, licenseFile, appKey, logPrefix, logVersion);

/* make sure that Sertainty initialized correctly */
if (status == 0)
{
  errorstr = SertaintyCore.uxpba_getData(bufferHandle).ReadString();
  Console.WriteLine("Error initializing environment: {0}", errorstr);
}

Console.WriteLine("Sertainty initialized successfully");
```

### Generate an ID File
See the `sampleid.xml` file in the `examples` directory. Open this and go through it. It contains application-related data and challenge questions and answers.

This is private for each user of the Sertainty SDK. You need this file to generate a digital id (.iic) for each user.

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

### Generate a UXP and Protect Data
We'll create a UXP file (with a .uxp extension) which will encapsulate encrypted data. This function requires two main inputs:
- the data to be encrypted
- The ID (.iic) file

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

### Access the Protected Data
You need to pass the authorization before accessing the data. The application will provide random challenges based on the ID (.iic) file which the user will need to answer within a limited amount of time.

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
The following files are used in the examples:
- `sampleid.xml`: This is used to generate `sampleid.iic`. It contains random challenge question and answers.
- `sampleid.iic`: This acts as a public digital id for the user.
- `sample.uxp`: UXP files are protected data. In order to protect data for a recipient, you need the recipient's ID (.iic) file and the data to be protected. 
