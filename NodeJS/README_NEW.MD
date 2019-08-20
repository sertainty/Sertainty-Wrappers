# Sertainty SDK
![Sertainty logo](https://i.ibb.co/ngjkVxN/Screen-Shot-2018-12-11-at-4-39-35-PM.png)


- [Sertainty SDK](#sertainty-sdk)
  - [How to run the code](#how-to-run-the-code)
      - [Step 1: Get the Sertainty license](#step-1-get-the-sertainty-license)
      - [Step 2: Get the code](#step-2-get-the-code)
      - [Step 3: Setup Dev Environment](#step-3-setup-dev-environment)
      - [Step 4: Install Dependencies](#step-4-install-dependencies)
      - [Step 5: Run the code](#step-5-run-the-code)
  - [Walkthrough](#walkthrough)
      - [Files](#files)
      - [Results](#results)
  - [[See the full example]()](#see-the-full-example)

## How to run the code

#### Step 1: Get the Sertainty license
You need to have the latest Sertainty Tools and the Sertainty license for running this tutorial. 

Please contact us through `tech-support@sertainty.com`

#### Step 2: Get the code 
- Clone this repository with 
   `git clone https://github.com/sertainty/Sertainty-Wrappers.git`

- or Download the Zip

#### Step 3: Setup Dev Environment
We reference necessary files from Sertainty Tools.

Set the install location of the Seratainty Tools using __SERTAINTY_HOME__ system environmental variable.

`setx SERTANITY_HOME /M "%ProgramFiles%\Sertainty"`

#### Step 4: Install Dependencies 
1. You need to have [Node JS](https://nodejs.org/en/) and [npm](https://www.npmjs.com/get-npm) installed in your PC. 

2. We use [node-gyp](https://github.com/nodejs/node-gyp) for buiding necessary addons:
     - install [Python](https://www.python.org/downloads/release/python-272/) 2.x.x
     - `npm install -g node-gyp`
     - `npm install -g --production windows-build-tools`

#### Step 5: Run the code
1. `cd Sertainty-Wrappers/NodeJS/examples`
2. `npm install`
3. `npm start`
4. Use these to answer the security questions you      get in the sample program. 

    `Username: sampleuser@myemail.com`

    `Challenge 1: Response 1`

    `Challenge 2: Response 2`

    `Challenge 3: Response 3`

    ...........

    `Challenge 10: Response 10`

## Walkthrough


#### Files

We use different files to run this examples. We'll see what they are. 
- sample.xml
- 

#### Results

After running the example you will see __sample.iic__ __sample.uxp__ files are generated inside the examples folder. 




## [See the full example]()


