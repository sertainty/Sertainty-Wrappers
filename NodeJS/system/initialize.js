const core = require('../sertainty-core');
const buffer = require('../buffer');
const utils = require('../utils');
const { SYSTEM } = require('../constants');

const initialize = function initialize(licenceFileName, appKey, args=[]) {
  console.log('Starting..');

  // options
  const bufferHandle = buffer.handle.get();
  const argsCount = args.length;
  const prefix = SYSTEM.LogPrefix;
  const version = SYSTEM.Version;

  // set logging settings
  core.uxpsys_setLogFile(prefix, version);
  console.log('Setting log file..');

  // init
  const status = core.uxpsys_initLibrary(bufferHandle, argsCount, args, licenceFileName, appKey, prefix, version);

  if(status === 0) {
    const errorHandle = core.uxpba_getData(bufferHandle);
    const errorString = utils.readAsString(errorHandle);
    console.log('Sertainty initialization failed');
    throw new Error(errorString);
  } else {
    console.log('Sertainty has been initialized.');
  }
  return;
};

module.exports = initialize;