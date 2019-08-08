# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)

- [Sertainty SDK](#sertainty-sdk)
  - [Getting Started with Sertainty](#getting-started-with-sertainty)
      - [Installing](#installing)
    - [Usage](#usage)
  - [API reference](#api-reference)

## Getting Started with Sertainty
This is the official C# SDK for Sertainty; Which provides functionality for generating ID files and protect data and control access for specific user/time/device/number of times to access.

#### Installing

- __Prerequisites__
  we use [node-gyp](https://github.com/nodejs/node-gyp) for buiding necessary addons: 
    -  install [Python](https://www.python.org/downloads/release/python-272/) 2.x.x
    - `npm install -g node-gyp`
    - `npm install -g --production windows-build-tools`

-  __Install Sertainty SDK__
    - `npm install --save sertainty-sdk`

-  __Set `SERTAINTY_HOME` environment variable__
  
    you can set the `SERTAINTY_HOME` with Command Prompt (run as Administrator).
    - `setx SERTAINTY_HOME /M "path\to\sertainty"`
    - `ex: setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"` 

### Usage
```javascript
/* import the core module */
const { core } = require("sertainty-sdk");
/* use core functions */
const bufferHandle = core.uxpba_newHandle();
```


## API reference

See the full API reference [here](). 
