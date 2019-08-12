using System;

namespace Sertainty
{
  using System.IO;

  class Program
  {
    //Note: This application uses SertaintyWrapper.cs file
    static void Main(string[] args)
    {
      Console.WriteLine("Sample C# Data Application using ID");

      string idXmlSpec = "sampleid.xml";
      string idFileSpec = "sampleid.iic";
      string uxpFileSpec = "sample.uxp";
      string dataPdfSpec = "data.pdf";
      string dataPdf2Spec = "data2.pdf";
      string copy1Spec = "copy1.pdf";
      string copy2Spec = "copy2.pdf";

      IntPtr appHandle;

      IntPtr buffer = SertaintyWrapper.uxpba_newHandle();
      IntPtr callstatus = SertaintyWrapper.uxpsys_newCallStatusHandle();
      SertaintyWrapper.uxpba_setData(buffer, "Test", 4);
      //Console.WriteLine(uxpba_getData(buffer));
      //uxpba_clearData(buffer);
      string errorstr;
      SertaintyWrapper.uxpsys_setLogFile("Sample C#", "Sample C# 1.0");

      long ret = SertaintyWrapper.uxpsys_initLibrary(buffer, args.LongLength, args, "sertainty.lic", "SertintyONE", "Sample C#", "Sample C# 1.0");

      if (ret == 0)
      {
        errorstr = SertaintyWrapper.uxpba_getData(buffer).ReadString();
        Console.WriteLine("Error initializing environment: {0}", errorstr);
      }
      else
      {
        Console.WriteLine("Library Initialized");
        SertaintyWrapper.uxpsys_fileReadAll(callstatus, idXmlSpec, buffer);

        if (SertaintyWrapper.uxpsys_hasError(callstatus))
        {
          IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(callstatus);
          string errMsg = errMsgPtr.ReadString();
          Console.WriteLine("Error reading ID: {0}", errMsg);
        }
        else
        {
          Console.WriteLine("{0} read", idXmlSpec);
          string doc = SertaintyWrapper.uxpba_getData(buffer).ReadString();
          SertaintyWrapper.uxpid_publishToFile(callstatus, idFileSpec, doc, 1);

          if (SertaintyWrapper.uxpsys_hasError(callstatus))
          {
            IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(callstatus);
            string errMsg = errMsgPtr.ReadString();
            Console.WriteLine("Error creating ID: {0}", errMsg);
          }
          else
          {
            Console.WriteLine("{0} created", idFileSpec);
            appHandle = SertaintyWrapper.uxpfile_newHandle();
            SertaintyWrapper.uxpfile_openNewFile(appHandle, uxpFileSpec, idFileSpec, 3, 1, 0);

            if (SertaintyWrapper.uxpsys_hasError(appHandle))
            {
              IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
              string errMsg = errMsgPtr.ReadString();
              Console.WriteLine("Error creating Data: {0}", errMsg);
            }
            else
            {
              Console.WriteLine("{0} created", uxpFileSpec);
              SertaintyWrapper.uxpfile_addVirtualFromFile(appHandle, "data.pdf", dataPdfSpec, -1, -1, 8);

              if (SertaintyWrapper.uxpsys_hasError(appHandle))
              {
                IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
                string errMsg = errMsgPtr.ReadString();
                Console.WriteLine("Error creating virtual file: {0}", errMsg);
              }
              else
              {
                Console.WriteLine("{0} added", dataPdfSpec);
                SertaintyWrapper.uxpfile_addVirtualFromFile(appHandle, "data2.pdf", dataPdf2Spec, -1, -1, 8);

                if (SertaintyWrapper.uxpsys_hasError(appHandle))
                {
                  IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
                  string errMsg = errMsgPtr.ReadString();
                  Console.WriteLine("Error creating virtual file: {0}", errMsg);
                }
                else
                {
                  Console.WriteLine("{0} added", dataPdf2Spec);

                  //Now open the first virtual file and write it out to a temporary file.
                  IntPtr fileHandle = SertaintyWrapper.uxpfile_openVirtualFile(appHandle, "data.pdf", Mode.ReadOnly);

                  if (SertaintyWrapper.uxpsys_hasError(appHandle))
                  {
                    IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
                    string errMsg = errMsgPtr.ReadString();
                    Console.WriteLine("Error opening virtual file: {0}", errMsg);
                  }
                  else
                  {
                    Console.WriteLine("{0} opened", "data.pdf");

                    Console.WriteLine("Reading data.pdf in loop ...");
                    FileStream sw = new FileStream(copy1Spec, FileMode.Create);
                    while (SertaintyWrapper.uxpfile_readVirtualFile(appHandle, fileHandle, buffer, 1000) > 0)
                    {
                      int len = (int)SertaintyWrapper.uxpba_getSize(buffer);
                      byte[] data = SertaintyWrapper.uxpba_getData(buffer).ReadBytes(len);
                      //long len = uxpba_getSize(buffer);
                      sw.Write(data, 0, len);
                    }
                    sw.Close();
                    SertaintyWrapper.uxpfile_closeVirtualFile(appHandle, fileHandle);
                    Console.WriteLine("Finished reading data.pdf");

                    if (SertaintyWrapper.uxpfile_compareExternalFile(appHandle, "data.pdf", copy1Spec))
                    {
                      Console.WriteLine("Comparison of data.pdf to copy1.pdf: successful");
                    }
                    else
                    {
                      Console.WriteLine("Comparison of data.pdf to copy1.pdf: failed");
                    }

                    //Close the UXP. This will delete the handle as well

                    SertaintyWrapper.uxpfile_close(appHandle);

                    //Reopen the Data ... includes authentication

                    Console.WriteLine("Opening new Data");

                    SertaintyWrapper.uxpfile_openFile(appHandle, uxpFileSpec, Mode.ReadOnly);

                    if (SertaintyWrapper.uxpsys_hasError(appHandle))
                    {
                      IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
                      string errMsg = errMsgPtr.ReadString();
                      Console.WriteLine("Error opening UXP: {0}", errMsg);
                    }
                    else
                    {
                      Console.WriteLine("Credentials necessary to access UXP:");
                      Console.WriteLine("  Username = SampleUser@myemail.com");
                      Console.WriteLine("  Challenge 1 = Response 1");
                      Console.WriteLine("  Challenge 2 = Response 2");
                      Console.WriteLine("  ... ");
                      Console.WriteLine("  Challenge 10 = Response 10\n");

                      bool done = false;
                      bool authorized = false;
                      AuthorizationStatus status;
                      while (!done)
                      {
                        status = SertaintyWrapper.uxpfile_authenticate(appHandle);

                        switch (status)
                        {
                          case AuthorizationStatus.Authorized:
                            Console.WriteLine("You're authorized");
                            done = true;
                            authorized = true;
                            break;
                          case AuthorizationStatus.NotAuthorized:
                            Console.WriteLine("You're not authorized");
                            authorized = false;
                            done = true;
                            break;
                          case AuthorizationStatus.Challenged:
                            for (int i = 0; i < SertaintyWrapper.uxpfile_getChallengeCount(appHandle); i++)
                            {
                              IntPtr ch = SertaintyWrapper.uxpfile_getChallenge(appHandle, i);
                              getResponse(ch);
                              SertaintyWrapper.uxpfile_addResponse(appHandle, ch);
                              SertaintyWrapper.uxpch_freeHandle(ch);
                            }
                            break;
                          default:
                            break;
                        }
                      }

                      if (authorized)
                      {
                        Console.WriteLine("Extracting data.pdf to copy2.pdf");

                        fileHandle = SertaintyWrapper.uxpfile_openVirtualFile(appHandle, "data.pdf", Mode.ReadOnly);

                        if (SertaintyWrapper.uxpsys_hasError(appHandle))
                        {
                          IntPtr errMsgPtr = SertaintyWrapper.uxpsys_getErrorMessage(appHandle);
                          string errMsg = errMsgPtr.ReadString();
                          Console.WriteLine("Error opening virtual file: {0}", errMsg);
                        }
                        else
                        {
                          FileStream sw2 = new FileStream(copy2Spec, FileMode.Create);
                          while (SertaintyWrapper.uxpfile_readVirtualFile(appHandle, fileHandle, buffer, 1000) > 0)
                          {
                            int len = (int)SertaintyWrapper.uxpba_getSize(buffer);
                            byte[] data = SertaintyWrapper.uxpba_getData(buffer).ReadBytes(len);
                            //long len = uxpba_getSize(buffer);
                            sw2.Write(data, 0, len);
                          }
                          sw2.Close();
                          SertaintyWrapper.uxpfile_closeVirtualFile(appHandle, fileHandle);

                        }
                      }
                    }
                  }


                }
              }
            }

            SertaintyWrapper.uxpfile_close(appHandle);
            SertaintyWrapper.uxpfile_freeHandle(appHandle);

          }

          SertaintyWrapper.uxpsys_freeCallStatusHandle(callstatus);

          SertaintyWrapper.uxpba_freeHandle(buffer);
          Console.WriteLine("Sample finished running. Press any key to exit...");

          Console.ReadKey();
        }
      }
    }

    private static void getResponse(IntPtr ch_handle)
    {
      IntPtr prompt = SertaintyWrapper.uxpba_newHandle();
      SertaintyWrapper.uxpch_getPrompt(ch_handle, prompt);
      SertaintyWrapper.uxpch_startTimer(ch_handle);

      Console.WriteLine("{0}> ", SertaintyWrapper.uxpba_getData(prompt).ReadString());
      string value = Console.ReadLine().TrimEnd('\r', '\n');

      SertaintyWrapper.uxpch_endTimer(ch_handle);
      SertaintyWrapper.uxpch_setValueString(ch_handle, value);

      SertaintyWrapper.uxpba_freeHandle(prompt);
    }
  }
}
