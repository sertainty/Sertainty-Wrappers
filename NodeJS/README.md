# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

Sertainty SDK helps to protect and encrypt your data and share secure manner. It allows desired parties to access the data if they are authenticated from a challenge verification process.

- [Sertainty SDK](#sertainty-sdk)
  - [How to run the code](#how-to-run-the-code)
      - [Step 1: Get the Sertainty license](#step-1-get-the-sertainty-license)
      - [Step 2: Get the code](#step-2-get-the-code)
      - [Step 3: Configure development environment](#step-3-configure-development-environment)
      - [Step 4: Install dependencies](#step-4-install-dependencies)
      - [Step 5: Run the code](#step-5-run-the-code)
  - [Walkthrough](#walkthrough)
      - [Initialize sertainty](#initialize-sertainty)
      - [Generate an ID file](#generate-an-id-file)
      - [Generate UXP and protect data](#generate-uxp-and-protect-data)
        - [Generate UXP file](#generate-uxp-file)
        - [Protect data](#protect-data)
      - [Access protected data](#access-protected-data)
      - [Files](#files)

## How to run the code

#### Step 1: Get the Sertainty license
You need to have the latest Sertainty Tools and the Sertainty license for running this tutorial. 

Please contact us through `tech-support@sertainty.com`

#### Step 2: Get the code 
- Clone this repository with 
   `git clone https://github.com/sertainty/Sertainty-Wrappers.git`

- or Download the Zip

#### Step 3: Configure development environment
We reference necessary files from Sertainty Tools.

Set the install location of the Seratainty Tools using __SERTAINTY_HOME__ system environmental variable.

`setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"`

#### Step 4: Install dependencies 
1. You need to have [Node JS](https://nodejs.org/en/) and [npm](https://www.npmjs.com/get-npm) installed in your PC. 

2. We use [node-gyp](https://github.com/nodejs/node-gyp) for buiding necessary addons:
     - install [Python](https://www.python.org/downloads/release/python-272/) 2.x.x
     - `npm install -g node-gyp`
     - `npm install -g --production windows-build-tools`

#### Step 5: Run the code
1. `cd Sertainty-Wrappers/NodeJS/examples`
2. `npm install`
3. `npm start`
4. Use these to answer the security questions you      get in the sample program. 

    `Username: sampleuser@myemail.com`

    `Challenge 1: Response 1`

    `Challenge 2: Response 2`

    `Challenge 3: Response 3`

    ...........

    `Challenge 10: Response 10`

## Walkthrough

In this example we are using Sertainty Technology to generate digital id and protect data using that id. We also demonstrate how to access protected data with authentication process. 

#### Initialize sertainty
You need to initialize sertainty sdk with valid licenseFile and appKey before using it. 

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
See the `sampleid.xml` file in the examples directory. Open this and go through it. It contains application related data and challenge questions and answers.

This is private for each user of the Sertainty SDK. You need this file to generate an digital id(.iic) for each user.

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
#### Generate UXP and protect data
We'll create a file with .uxp extension that can encapsulate data in encrypted mode. This function requires two main inputs,
- Data(to be encrypted)
- ID(.iic) file

##### Generate UXP file
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

#### Access protected data
You need to pass the authorization before accessing the data. Application will provide randomize challenge questions based on the ID(.iic) file and user need to answer them within limited time.

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
#### Files
We use different files to run this examples. We'll see what they are. 
- `sampleid.xml` - This file used to generate `sampleid.iic`. Contains random challenge question and answers.
- `sampleid.iic` - Act as a digital id for the user of this application. Can be shared with other parties.
- `sample.uxp` - Uxp files are protected data. In order to protect data for intended recipiant you need the recipient's `id(.iic)` file and the data to be protected. 
