const core = require('../sertainty-core');

let callStatusHandle = null;

const create = function create() {
  callStatusHandle = core.uxpsys_newCallStatusHandle()
};

const get = function get() {
  if(!callStatusHandle) {
    create();
  }
  return callStatusHandle;
};

module.exports = {
  get
};
