// check for the OS
// currenty supports windows only
if(process.platform !== "win32") {
  throw new Error("Sorry. We are currently supporting windows only.")
}

const core = require('./sertainty-core');
const types = require('./constants');
const utils = require('./utils');
const sertainty = require('./sertainty');

module.exports = {
  core,
  types,
  utils,
  sertainty
};
