const ffi = require('ffi');
const { intType, longType, voidType, stringType, boolType, stringArrType, intPtrType } = require('./data-types');
const config = require('./config');

// config
const SERTAINTY_PATH  = config.SERTAINTY_HOME + "\\bin\\"+ config.DLL_NAME;

// method wrappers for SertaintyCore.dll

// uxpsys
// setLogFile
const uxpsys_setLogFile = [voidType, [stringType, stringType]];
// installLicense
const uxpsys_installLicense = [longType, [intPtrType, stringType]]
// hasError
const uxpsys_hasError = [boolType, [intPtrType]];
// getErrorMessage
const uxpsys_getErrorMessage = [intPtrType, [intPtrType]];
// initLibrary
const uxpsys_initLibrary = [longType, [intPtrType, longType, stringArrType, stringType, stringType, stringType, stringType]];
// uxpsys_newCallStatusHandle
const uxpsys_newCallStatusHandle = [intPtrType, []];
// freeCallStatusHandle
const uxpsys_freeCallStatusHandle = [voidType, [intPtrType]];
// fileReadAll
const uxpsys_fileReadAll = [voidType, [intPtrType, stringType, intPtrType]];

// uxpba
// newHandle
const uxpba_newHandle = [intPtrType, []];
// getData
const uxpba_getData = [intPtrType, [intPtrType]];
// getSize
const uxpba_getSize = [longType, [intPtrType]];
// setData
const uxpba_setData = [voidType, [intPtrType, stringType, longType]];
// clearData
const uxpba_clearData = [voidType, [intPtrType]];
// freeHandle
const uxpba_freeHandle = [voidType, [intPtrType]];

// uxpfile
// newHandle
const uxpfile_newHandle = [intPtrType, []];
// openNewFile
const uxpfile_openNewFile = [voidType, [intPtrType, stringType, stringType, longType, longType, longType]];
// addVirtualFromFile
const uxpfile_addVirtualFromFile = [voidType, [intPtrType, stringType, stringType, longType, longType, longType]];
// openVirtualFile - struct
const uxpfile_openVirtualFile = [intPtrType, [intPtrType, stringType, longType]];
// readVirtualFile 
const uxpfile_readVirtualFile = [longType, [intPtrType, intPtrType, intPtrType, longType]];
// closeVirtualFile
const uxpfile_closeVirtualFile = [voidType, [intPtrType, intPtrType]];
// compareExternalFile
const uxpfile_compareExternalFile = [boolType, [intPtrType, stringType, stringType]];
// close
const uxpfile_close = [voidType, [intPtrType]];
// openFile - struct
const uxpfile_openFile = [voidType, [intPtrType, stringType, longType]];
// uxpfile_authenticate - struct
const uxpfile_authenticate = [longType, [intPtrType]];
// getChallengeCount
const uxpfile_getChallengeCount = [intType, [intPtrType]];
// getChallenge
const uxpfile_getChallenge = [intPtrType, [intPtrType, intType]];
// addResponse
const uxpfile_addResponse = [voidType, [intPtrType, intPtrType]];
// freeHandle
const uxpfile_freeHandle = [voidType, [intPtrType]];

// uxpid
// publishToFile
const uxpid_publishToFile = [voidType, [intPtrType, stringType, stringType, longType]];

// uxpch
// freeHandle
const uxpch_freeHandle = [voidType, [intPtrType]];
// getPrompt
const uxpch_getPrompt = [voidType, [intPtrType, intPtrType]];
// setValueString
const uxpch_setValueString = [voidType, [intPtrType, stringType]];
// startTimer
const uxpch_startTimer = [voidType, [intPtrType]];
// endTimer
const uxpch_endTimer = [voidType, [intPtrType]];

// initialize library interface with ffi
const sertainty = ffi.Library(SERTAINTY_PATH, {
    // uxpsys
    uxpsys_setLogFile,
    // uxpsys_installLicense,
    uxpsys_hasError,
    uxpsys_getErrorMessage,
    uxpsys_initLibrary,
    uxpsys_newCallStatusHandle,
    uxpsys_freeCallStatusHandle,
    uxpsys_fileReadAll,
    // uxpba
    uxpba_newHandle,
    uxpba_getData,
    uxpba_getSize,
    uxpba_setData,
    // uxpba_clearData,
    uxpba_freeHandle,
    // // uxpfile
    uxpfile_newHandle,
    uxpfile_openNewFile,
    uxpfile_addVirtualFromFile,
    uxpfile_openVirtualFile,
    uxpfile_readVirtualFile,
    uxpfile_closeVirtualFile,
    uxpfile_compareExternalFile,
    uxpfile_close,
    uxpfile_openFile,
    uxpfile_authenticate,
    uxpfile_getChallengeCount,
    uxpfile_getChallenge,
    uxpfile_addResponse,
    uxpfile_freeHandle,
    // // uxpid
    uxpid_publishToFile,
    uxpch_freeHandle,
    uxpch_getPrompt,
    uxpch_setValueString,
    uxpch_startTimer,
    uxpch_endTimer
});

module.exports = sertainty;
