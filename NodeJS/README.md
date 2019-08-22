# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

The Sertainty SDK helps to protect, encrypt, and share your data securely. Authorized individuals may access the data if they authenticate through a challenge verification process.

- [Sertainty SDK](#sertainty-sdk)
  - [How to Run the Code](#how-to-run-the-code)
    - [Step 1: Get the Sertainty License](#step-1-get-the-sertainty-license)
    - [Step 2: Get the Code](#step-2-get-the-code)
    - [Step 3: Configure the Development Environment](#step-3-configure-the-development-environment)
    - [Step 4: Install the Dependencies](#step-4-install-the-dependencies)
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
So that the Sertainty Tools can be found, set the install location of the Sertainty Tools using the `SERTAINTY_HOME` system environment variable.

`setx SERTAINTY_HOME /M "%ProgramFiles%\Sertainty"`

### Step 4: Install the Dependencies 
Install the following:
1. [Python 2.x.x](https://www.python.org/downloads/release/python-272/) 
2. [Node JS](https://nodejs.org/en/)
3. [npm](https://www.npmjs.com/get-npm)
4. [node-gyp](https://github.com/nodejs/node-gyp) (`npm install -g node-gyp`)
5. [Windows Build Tools](https://www.npmjs.com/package/windows-build-tools) (`npm install -g --production windows-build-tools`)

### Step 5: Run the Code
1. `cd Sertainty-Wrappers/NodeJS/examples`
2. `npm install`
3. `npm start`
4. Use the following credentials to authenticate within the sample program:

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

```javascript
const bufferHandle = core.uxpba_newHandle();

const licenseFile = "sertainty.lic";
const appKey = "SertintyONE";
const logPrefix = "sertainty-tutorial";
const logVersion = "1.0";
const args = [];

const status = core.uxpsys_initLibrary(bufferHandle, args.length, args, licenceFile, appKey, logPrefix, logVersion);

/* make sure that Sertainty initialized correctly */

if (status == 0) {
  const errHandle = core.uxpba_getData(bufferHandle);
  const errText = ref.readCString(errHandle);
  console.error(`Error initializing the Environment: ${errText}`);
} 

console.log("Sertainty initialized successfully");
```

### Generate an ID File
See the `sampleid.xml` file in the `examples` directory. Open this and go through it. It contains application-related data and challenge questions and answers.

This is private for each user of the Sertainty SDK. You need this file to generate a digital id (.iic) for each user.

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
### Generate a UXP and Protect Data
We'll create a UXP file (with a .uxp extension) which will encapsulate encrypted data. This function requires two main inputs:
- the data to be encrypted
- The ID (.iic) file

#### Generate a UXP File
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

#### Protect Data
```csharp
core.uxpfile_addVirtualFromFile(appHandle, "data.pdf", dataPdfSpec, -1, -1, 8);
  
if (core.uxpsys_hasError(appHandle)) {
    const errHandle = core.uxpsys_getErrorMessage(appHandle);
    const errText = ref.readCString(errHandle);
    console.error("Error creating virtual file: ${errText}");
}

console.log("File has been encrypted");
```

### Access the Protected Data
You need to pass the authorization before accessing the data. The application will provide random challenges based on the ID (.iic) file which the user will need to answer within a limited amount of time.

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
            // get remaining challenge count
            const challengeCount = core.uxpfile_getChallengeCount(appHandle);
            for (let i = 0; i < challengeCount; i++) {
                const challengePtr = core.uxpfile_getChallenge(appHandle,i);
                // print the challenge question and wait for the user response and save the response
                getResponse(challengePtr);
                core.uxpfile_addResponse(appHandle, challengePtr);
                core.uxpch_freeHandle(challengePtr);
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

### Files
The following files are used in the examples:
- `sampleid.xml`: This is used to generate `sampleid.iic`. It contains random challenge question and answers.
- `sampleid.iic`: This acts as a public digital id for the user.
- `sample.uxp`: UXP files are protected data. In order to protect data for a recipient, you need the recipient's ID (.iic) file and the data to be protected. 
