const core = require('../sertainty-core');

let bufferHandle = null;

const create = function create() {
  bufferHandle = core.uxpba_newHandle();
};

const get = function get() {
  if(!bufferHandle) {
    create();
  }
  return bufferHandle;
};

module.exports = {
  get
};
