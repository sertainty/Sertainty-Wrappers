const utils = require('../utils');
const core = require('../sertainty-core');

// check if error exist
const isExist = function isExist(callStatusHandle) {
  const isErrorExist = core.uxpsys_hasError(callStatusHandle);
  return isErrorExist;
};

// get error string
const getString = function getString(callStatusHandle) {
  const errorHandle = core.uxpsys_getErrorMessage(callStatusHandle);
  const errorString = utils.readAsString(errorHandle);
  return errorString;
};

// get error and throw if there any
const get = function get(callStatusHandle) {
  const isErrorExist = isExist(callStatusHandle);
  if(isErrorExist) {
    const errorString = getString(callStatusHandle);
    throw new Error(errorString);
  }
  return;
};

module.exports = {
  isExist,
  getString,
  get
};
