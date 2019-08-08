const config = {
  SERTAINTY_HOME: getSertaintyHome(),
  DLL_NAME: 'SertaintyCore2'
};

function getSertaintyHome() {
  const SERTAINTY_HOME = process.env.SERTAINTY_HOME;

  if(SERTAINTY_HOME && SERTAINTY_HOME.length > 0) {
    return SERTAINTY_HOME;
  } else {
    throw new Error("SERTAINTY_HOME env variable not found");
  }
};

module.exports = config;
