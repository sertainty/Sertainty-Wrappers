const ref = require('ref');
const ArrayType = require('ref-array');

// we are declaring data types using ref and ref-array modules
// common types
const intType = ref.types.int;
const longType = ref.types.long;
const voidType = ref.types.void;
const stringType = ref.types.CString;
const boolType = ref.types.bool;

// pointer types
const intPtrType = ref.refType(intType);

// arrayTypes
const stringArrType = ArrayType(stringType);

module.exports = {
  intType,
  longType,
  voidType,
  stringType,
  boolType,
  stringArrType,
  intPtrType
};
