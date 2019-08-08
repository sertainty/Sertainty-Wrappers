const core  = require('../sertainty-core');
const handle = require('./handle');
const utils = require('../utils');

const getDataAsString = function getData(bufferHandle) {
  const dataHandle = core.uxpba_getData(buffer);
  const data = utils.readAsString(dataHandle);
  return data;
};

const getDataAsBuffer = function getDataAsBuffer(bufferHandle, size) {
  const dataHandle = core.uxpba_getData(buffer);
  const data  = utils.readAsBuffer(handle, size);
};

module.exports = {
  handle,
  getDataAsBuffer,
  getDataAsString
};
