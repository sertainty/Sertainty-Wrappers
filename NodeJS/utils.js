const ref = require('ref');
const readBytes = require('@arunwij/read-bytes');

// read as string
exports.readAsString = function readAsString(handle) {
  const string = ref.readCString(handle, 0);
  return string;
};

exports.readAsBuffer = function readAsBuffer(handle, size) {
  const buffer = readBytes(handle, size);
  return buffer;
};
