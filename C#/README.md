# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

- [Getting Started with Sertainty](#getting-started-with-sertainty)
    - [Installing](#installing)
    - [License](#license)
    - [Usage](#usage)
    - [Tutorial](#tutorial)
- [API reference](#api-reference)

## Getting Started with Sertainty
This is the official NodeJS SDK for Sertainty; Which provides functionality for generating ID files and protect data and control access for specific user/time/device/number of times to access.

#### Installing

- __Prerequisites__
  we use [node-gyp](https://github.com/nodejs/node-gyp) for buiding necessary addons: 
    -  install Sertainty Tools. 
    -  install [Python](https://www.python.org/downloads/release/python-272/) 2.x.x
    - `npm install -g node-gyp`
    - `npm install -g --production windows-build-tools`

-  __Install Sertainty SDK__
    - `npm install --save sertainty-sdk`

-  __Set `SERTAINTY_HOME` environment variable__
  
    you can set the `SERTAINTY_HOME` with Command Prompt (run as Administrator).
    - `setx SERTAINTY_HOME /M "path\to\sertainty"`
    - `ex: setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"` 

### License
Install Sertainty Tools for installing the Sertainty License. 

### Usage
```javascript
/* import the core module */
const { core } = require("sertainty-sdk");
/* use core functions */
const bufferHandle = core.uxpba_newHandle();
```

### Tutorial
We will guide you through how to use Sertainty in your project. You will learn,
  - [Initialize Sertainty](#Initialize-Sertainty)
  - [Generate an ID file](#Generate-an-ID-file)
  - [Protect file with UXP](#Protect-file-with-UXP)
    - [Create UXP file](#Create-UXP-file)
    - [Protect data](#Protect-data)
  - [Authorize UXP](#Authorize-UXP)
    - [Handle authorization](#Handle-authorization)
    - [Handle challange response](#Handle-challange-response)
  - [Read UXP file](#Read-UXP-file)
  - [Handle Errors](#Handle-Errors)

Create sample project and import Sertainty. (Please refer [Usage](#Usage) section)

See the full example [here.](https://gist.github.com/arunwij/867f49a8ceea04e9e0fed352745dfc4c)

#### Initialize Sertainty 
```javascript
const bufferHandle = core.uxpba_newHandle();

const licenseFile = "sertainty.lic";
const appKey = "SertaintyONE";
const logPrefix = "sertainty-tutorial";
const logVersion = "1.0";
const args = [];

const status = core.uxpsys_initLibrary(bufferHandle, args.length, args, licenceFile, appKey, logPrefix, logVersion);

/* make sure if Sertainty initialized correctly */

if (status == 0) {
  const errHandle = core.uxpba_getData(bufferHandle);
  const errText = ref.readCString(errHandle);
  console.error(`Error initializing the Environment: ${errText}`);
} 

console.log("Sertainty initialized successfully");
```

#### Generate an ID file
To generate ID (.iic) file, we need a XML version of ID spec which can generate from Sertainty application. 
```javascript
/* XML id file generated from the sertainty application */
const idXmlSpec = "sampleid.xml";
const idFileSpec = "sampleid.iic";

const callStausHandle = core.uxpsys_newCallStatusHandle();
core.uxpsys_fileReadAll(callStausHandle, idXmlSpec, bufferHandle);

/* check errors in last `uxpsys_fileReadAll` function call */
if (core.uxpsys_hasError(callStausHandle)) {
  const errHandle = core.uxpsys_getErrorMessage(callStausHandle);
  const errText = ref.readCString(errHandle);
  console.error(`Reading ${idXmlSpec}`);
  
} else {
  /* Generate ID file */
  
  console.log(`Read ${idXmlSpec}: done`);
  const dataHandle = core.uxpba_getData(buffer);
  const doc = ref.readCString(dataHandle);
  core.uxpid_publishToFile(callStatusHandle, idFileSpec, doc, 1);
  
  if (core.uxpsys_hasError(callStatusHandle)) {
      const errHandle = core.uxpsys_getErrorMessage(callStatusHandle);
      const errText = ref.readCString(errHandle);
      console.error(`Error creating ID file: ${errText}`);
  }
  
  console.log("${idFileSpec} created");
}
```

#### Protect file with UXP
We'll create a file with .uxp extension that can encapsulate data in encrypted mode. This function requires two main inputs,
- Data(to be encrypted)
- ID(.iic) file

##### Create UXP file
```javascript
/* a file to be encrypted */
const dataPdfSpec = "data.pdf";
/* output file name */
const uxpFileSpec = "sample.uxp";


const appHandle = core.uxpfile_newHandle();
core.uxpfile_openNewFile(appHandle, uxpFileSpec, idFileSpec, 3, 1, 0);

if (core.uxpsys_hasError(appHandle)) {
  const errHandle = core.uxpsys_getErrorMessage(appHandle);
  const errText = ref.readCString(errMsgPtr);
  console.log("Error opening file ${errText}");
  
} else {
  console.log("${uxpFileSpec} created");
}
```

##### Protect data
```csharp
core.uxpfile_addVirtualFromFile(appHandle, "data.pdf", dataPdfSpec, -1, -1, 8);
  
if (core.uxpsys_hasError(appHandle)) {
    const errHandle = core.uxpsys_getErrorMessage(appHandle);
    const errText = ref.readCString(errHandle);
    console.error("Error creating virtual file: ${errText}");
}

console.log("File has been Encrypted");
```

#### Authorize UXP

##### Handle authorization
```javascript
core.uxpfile_openFile(appHandle, uxpFileSpec, Mode.ReadOnly);

let done = false;
let authorized = false;
let status = null;

while (!done) {
// get authentication status
status = core.uxpfile_authenticate(appHandle);

    switch (status) {
        case AUTHORIZATION_STATUS.Authorized: {
            console.log("You're authorized");
            done = true;
            authorized = true;
            break;
        }

        case AUTHORIZATION_STATUS.NotAuthorized: {
            console.log("You're not authorized");
            authorized = false;
            done = true;
            break;
        }

        case AUTHORIZATION_STATUS.Challenged: {
            // get remaining challange count
            const challangeCount = core.uxpfile_getChallengeCount(appHandle);
            for (let i = 0; i < challangeCount; i++) {
                const challangePtr = core.uxpfile_getChallenge(appHandle,i);
                // print the challange question and wait for the user response and save the response
                getResponse(challangePtr);
                core.uxpfile_addResponse(appHandle, challangePtr);
                core.uxpch_freeHandle(challangePtr);
            }
          break;
        }

        default: {
            console.log("Invalid authorization status");
            break;
        }
    }
}
```

##### Handle challange response
```javascript
function getResponse(challangeHandle) {
    /* get question and start the timer */
    const promptHandle = core.uxpba_newHandle();
    core.uxpch_getPrompt(challengeHandle, promptHandle);
    core.uxpch_startTimer(challengeHandle);
    const dataHandle = core.uxpba_getData(promptHandle);
    const question = ref.readCString(dataHandle);
    
    /* read user's answer from console */ 
    const answer = readlineSync.question(`${question}? `);
    const trimmedAnsewer = answer.trim();
    
    /* stop the timer and save the answer */
    core.uxpch_endTimer(challengeHandle);
    core.uxpch_setValueString(challengeHandle, trimmedAnsewer);
    core.uxpba_freeHandle(promptHandle);
}
```

#### Read UXP file
```javascript
/* output file name */
const copy1Spec = "copy1.pdf";

if (authorized) {
  console.log(`Extracting ${dataPdfSpec} to ${copy2Spec}`);
  const fileHandle = core.uxpfile_openVirtualFile(appHandle, dataPdfSpec, MODE.ReadOnly);

  if (core.uxpsys_hasError(appHandle)) {
    const errText = getError(callStatus);
    console.log(`penVirtualFile: ${errText}`);
  } else {

    const wstream = fs.createWriteStream(copy2Spec);

    while (core.uxpfile_readVirtualFile(appHandle, fileHandle, buffer, 1000) > 0) {
      const length = core.uxpba_getSize(buffer);
      const dataPtr = core.uxpba_getData(buffer);
      const data = readBytes(dataPtr, length);
      wstream.write(data);
    }
    wstream.close();
    core.uxpfile_closeVirtualFile(appHandle, fileHandle);
  }
}
```

#### Handle Errors
To check if there are any errors after each operation, we can use `uxpsys_hasError(IntPtr handle)` method. It requires a handle as an input parameter.  If the operation is file related, we need to pass a file handle, otherwies it will be a callStatusHandle. (Handle for getting status of an operation);

````javascript
/* get call status handle */

const callStatusHandle = core.uxpsys_newCallStatusHandle();
core.uxpid_publishToFile(callstatus, idFileSpec, doc, 1);

/* handle errors */
if (core.uxpsys_hasError(callStatusHandle)) {
    const errHandle = core.uxpsys_getErrorMessage(callStatusHandle);
    const errText = ref.readCString(errhandle);
    console.error(`Error creating ID: ${errText}`);
}
````

## API reference

See the full API reference [here](). 
