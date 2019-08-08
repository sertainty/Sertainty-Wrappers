const core = require('../sertainty-core');

let fileHandle = null;

const create = function create() {
  fileHandle = core.uxpfile_newHandle();
};

const get = function get() {
  if(!fileHandle) {
    create();
  }
  return fileHandle;
};

module.exports = {
  get
};
