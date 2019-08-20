const { 
  core, 
  utils, 
  types: { MODE, AUTHORIZATION_STATUS } 
} = require("@sertainty-eng/sertainty-sdk");
const readlineSync = require("readline-sync");
const fs = require("fs");

// handles
let error = null;
let appHandle = null;
let fileHandle = null;

// other constants
const prefix = "sertainty-examples";
const version = "1.000";
const licenseFile = "sertainty.lic";
const appKey = "SertintyONE";

// input files
const idXmlSpec = "sampleid.xml";
const dataPdfSpec = 'data.pdf';
const dataPdf2Spec = 'data2.pdf';

// these files will be generated when program running
const copy1Spec = "copy1.pdf";
const copy2Spec = "copy2.pdf";
const idFileSpec = "sampleid.iic";
const uxpFileSpec = "sample.uxp";

console.log("******* sertainty-node - sample application *******");

// data/error bufferHandle
var bufferHandle = core.uxpba_newHandle();

// handle for getting function status after calling them
var callStatusHandle = core.uxpsys_newCallStatusHandle();

core.uxpsys_setLogFile(prefix, version);

// initialize sertainty
const ret = core.uxpsys_initLibrary(bufferHandle, 0, [], licenseFile, appKey, prefix, version);

// check for initialization errors
if (ret === 0) {

  const errText = getError(callStatusHandle);
  console.error("Error initializing environment: ", errText);
} else {
  console.log("core initialized");
  core.uxpsys_fileReadAll(callStatusHandle, idXmlSpec, bufferHandle);
  error = core.uxpsys_hasError(callStatusHandle);

  // check if uxpfileReadAll have any errors
  if (error) {
    const errText = getError(callStatusHandle);
    console.log("uxpfileReadAll: ", errText);
  } else {
    console.log("{0} read", idXmlSpec);
    const dataHandle = core.uxpba_getData(bufferHandle);
    const doc = utils.readAsString(dataHandle);

    // generate the iic file
    core.uxpid_publishToFile(callStatusHandle, idFileSpec, doc, 1);

    error = core.uxpsys_hasError(callStatusHandle);
    if (error) {
      const errText = getError(callStatusHandle);
      console.error("publishToFile: ", errText);
    } else {
      console.log("{0} created", idFileSpec);

      // get the uxp file handle
      appHandle = core.uxpfile_newHandle();
      // create the uxp file
      core.uxpfile_openNewFile(appHandle, uxpFileSpec, idFileSpec, 3, 1, 0);

      error = core.uxpsys_hasError(appHandle);

      if (error) {
        const errText = getError(appHandle);
        console.log("openNewFile: ", errText);
      } else {
        console.log("{0} created", uxpFileSpec);

        core.uxpfile_addVirtualFromFile(appHandle, "data.pdf", dataPdfSpec, -1, -1, 8);

        error = core.uxpsys_hasError(appHandle);

        if (error) {
          const errText = getError(appHandle);
          console.error("addVirtualFromFile: ", errText);
        } else {
          console.log("{0} added", dataPdfSpec);
          core.uxpfile_addVirtualFromFile(appHandle,"data2.pdf", dataPdf2Spec, -1, -1, 8);
          error = core.uxpsys_hasError(appHandle);

          if (error) {
            const errText = getError(appHandle);
            console.error("Error: ", errText);
          } else {
            console.log("{0} added", dataPdf2Spec);

            // Now open the first virtual file and write it out to a temporary file.
            fileHandle = core.uxpfile_openVirtualFile(appHandle, "data.pdf", MODE.ReadOnly);

            // check if opening virtual file have any error
            error = core.uxpsys_hasError(appHandle);

            if (error) {
              const errText = getError(appHandle);
              console.log("openVirtualFile: ", errText);
            } else {
              console.log("{0} opened", dataPdfSpec);
              console.log(`Reading ${dataPdfSpec} in loop...`);

              const wstream = fs.createWriteStream(copy1Spec);

              while (isEndOfFile(appHandle, fileHandle, bufferHandle)) {
                const length = core.uxpba_getSize(bufferHandle);
                const dataHandle = core.uxpba_getData(bufferHandle);
                const data = utils.readAsBuffer(dataHandle, length);
                wstream.write(Uint8Array.from(data));
              }

              wstream.end();

              core.uxpfile_closeVirtualFile(appHandle, fileHandle);
              console.log(`Finished reading ${dataPdfSpec}`);

              if (
                core.uxpfile_compareExternalFile(
                  appHandle,
                  "data.pdf",
                  copy1Spec
                )
              ) {
                console.log(`Comparison of data.pdf to copy1.pdf: successful`);
              } else {
                console.log(`Comparison of data.pdf to copy1.pdf: failed`);
              }

              // close the UXP. This will delete the handle as well
              core.uxpfile_close(appHandle);

              // reopen the Data ... includes authentication
              console.log("Opening new Data");

              core.uxpfile_openFile(appHandle, uxpFileSpec, MODE.ReadOnly);

              if (core.uxpsys_hasError(appHandle)) {
                const errorText = getError(appHandle);
                console.log("Error opening UXP: {0}", errorText);
              } else {
                console.log("Credentials necessary to access UXP:");
                console.log("  Username = SampleUser@myemail.com");
                console.log("  Challenge 1 = Response 1");
                console.log("  Challenge 2 = Response 2");
                console.log("  ... ");
                console.log("  Challenge 10 = Response 10\n");

                let done = false;
                let authorized = false;
                let status;

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
                      const challangeCount = core.uxpfile_getChallengeCount(
                        appHandle
                      );

                      for (let i = 0; i < challangeCount; i++) {
                        const challangeHandle = core.uxpfile_getChallenge(
                          appHandle,
                          i
                        );
                        // print the challange question and wait for the user response and save the response
                        getResponse(challangeHandle);
                        core.uxpfile_addResponse(appHandle, challangeHandle);
                        core.uxpch_freeHandle(challangeHandle);
                      }
                      break;
                    }

                    default: {
                      console.log("Invalid authorization status");
                      break;
                    }
                  }
                }

                // if user authorized
                if (authorized) {
                  console.log(`Extracting data.pdf to copy2.pdf}`);
                  fileHandle = core.uxpfile_openVirtualFile(
                    appHandle,
                    "data.pdf",
                    MODE.ReadOnly
                  );

                  if (core.uxpsys_hasError(appHandle)) {
                    const errText = getError(appHandle);
                    console.log("Error openVirtualFile: {0}", errText);
                  } else {
                    const wstream2 = fs.createWriteStream(copy2Spec);

                    while (isEndOfFile(appHandle, fileHandle, bufferHandle)) {
                      const length = core.uxpba_getSize(bufferHandle);
                      const dataHandle = core.uxpba_getData(bufferHandle);
                      const data = utils.readAsBuffer(dataHandle, length);
                      wstream2.write(data);
                    }
                    wstream2.close();
                    core.uxpfile_closeVirtualFile(appHandle, fileHandle);
                  }
                }
              }
            }
          }
        }
      }
      core.uxpfile_close(appHandle);
      core.uxpfile_freeHandle(appHandle);
    }
    core.uxpsys_freeCallStatusHandle(callStatusHandle);
    core.uxpba_freeHandle(bufferHandle);
    console.log("******************** Sample finished *********************");
  }
}

// read file untill it's ended
function isEndOfFile(appHandle, fileHandle, bufferHandle) {
  const readLength = 1000;
  const dataLength = core.uxpfile_readVirtualFile(
    appHandle,
    fileHandle,
    bufferHandle,
    readLength
  );
  return dataLength > 0;
};

// read error from handle
function getError(handle) {
  const errorHandle = core.uxpsys_getErrorMessage(handle);
  const errorText = utils.readAsString(errorHandle);
  return errorText;
};

// get the response for a challange question
function getResponse(challengeHandle) {
  const promptHandle = core.uxpba_newHandle();
  core.uxpch_getPrompt(challengeHandle, promptHandle);
  core.uxpch_startTimer(challengeHandle);
  const dataHandle = core.uxpba_getData(promptHandle);
  const question = utils.readAsString(dataHandle);
  const answer = readlineSync.question(`${question}? `);
  const trimmedAnsewer = answer.trim();
  core.uxpch_endTimer(challengeHandle);
  core.uxpch_setValueString(challengeHandle, trimmedAnsewer);
  core.uxpba_freeHandle(promptHandle);
};
