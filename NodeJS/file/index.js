const core = require('../sertainty-core');
const system = require('../system');
const buffer = require('../buffer');
const handle = require('./handle');

const readFileAsString = function readFileAsString(callStausHandle, idXmlSpec, bufferHandle) {
  core.uxpsys_fileReadAll(callStausHandle, idXmlSpec, bufferHandle);
  system.error.get();
  const stringFile = buffer.getDataAsString(bufferHandle);
  return stringFile;
};

const generateIdFile = function generateIdFile(callStausHandle, idFileSpec, idXmlAsString, mods) {
  core.uxpid_publishToFile(callStausHandle, idFileSpec, idXmlAsString, mods);
};

const open = function open(fileHandle, uxpFileSpec, idFileSpec, type, mods, flags) {

};

module.exports = {
  readFileAsString,
  generateIdFile,
  handle
};
