// mode
const MODE = {
  ReadOnly: 1,
  WriteOnly: 2,
  ReadWrite: 3
};

// modifiers
const MODIFIER = {
  Replace: 0x00001,
  Merge: 0x00002,
  Recurse: 0x00004,
  Compress: 0x00008,
  IncHidden: 0x00010,
  Shred: 0x00020,
  IncRealities: 0x00040,
  NoOptimize: 0x00080,
  DeleteSrc: 0x00100,
  MinSize: 0x00200,
  Reclaim: 0x00400,
  RecurseVirtual: 0x00800,
  Create: 0x01000,
  Load: 0x02000,
  ReadWrite: 0x04000,
  Protect: 0x08000,
  IncScript: 0x10000,
  Formatted: 0x20000,
  TokenReplace: 0x40000
};

const AUTHORIZATION_STATUS = {
  NotAuthorized: 0x00001,  /*!< Access to UXP is not authorized */
  InvalidUsername: 0x00002,  /*!< Invalid username was provided */
  FileMoved: 0x00004,  /*!< File has been moved */
  ScheduleViolation: 0x00008,  /*!< Schedule violation */
  Authorized: 0x00010,  /*!< User has been authorized */
  ConfigNotFound: 0x00020,  /*!< Address configuration not reconized */
  LocationNotFound: 0x00040,  /*!< Address location not recognized */
  DeviceNotFound: 0x00080,  /*!< Address device not recognized */
  DeviceLocationFound: 0x00100,  /*!< Address location and device pair not recognized */
  Challenged: 0x00200,  /*!< User is challenged */
  Panic: 0x00400,  /*!< User has indicated a panic situation */
  GlobalSchedViolation: 0x00800,  /*!< Global schedule violation */
  Threat: 0x01000,  /*!< Unauthoried access threat detected */
  Canceled: 0x02000,  /*!< User canceled access attempt */
  LdapViolation: 0x04000,  /*!< LDAP approval violation */
  ConfigFound: 0x08000,  /*!< Address configuration was recognized */
  NoSingleSignOn: 0x10000  /*!< Single sign-on attempt failed */
};

const SYSTEM = {
  LogPrefix: 'sertainty-node',
  Version: '1.0'
};

module.exports = {
  MODE,
  MODIFIER,
  AUTHORIZATION_STATUS,
  SYSTEM
}
