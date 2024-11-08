using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace ServiceNowFilesToFTP
{
    class clFTP
    {
       
        public static void putFile()
        {
            List<string> msg = new List<string>();

            try
            {
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = System.Configuration.ConfigurationSettings.AppSettings["ftpSite"],
                    UserName = System.Configuration.ConfigurationSettings.AppSettings["ftpUser"],
                    Password = System.Configuration.ConfigurationSettings.AppSettings["ftpPass"],
                    SshHostKeyFingerprint = System.Configuration.ConfigurationSettings.AppSettings["ftpFingerPrint"],
                    //HostName = proc.ftpinfo.FTPServer,
                    //UserName = proc.ftpinfo.UserName,
                    //Password = proc.ftpinfo.Word,
                    //SshHostKeyFingerprint = proc.ftpinfo.SFTPKey,
                    PortNumber = 22,
                    TimeoutInMilliseconds = 300000

                };
                using (Session session = new Session())
                {
                    session.SessionLogPath = "";
                    session.Open(sessionOptions);

                    TransferOptions tranferOptions = new TransferOptions();
                    tranferOptions.TransferMode = TransferMode.Binary;
                    tranferOptions.FilePermissions = null;
                    tranferOptions.PreserveTimestamp = false;
                    tranferOptions.ResumeSupport.State = TransferResumeSupportState.Off;

                    WinSCP.TransferOperationResult transferResult;

                    
                        bool allFIlesOK = true;
                        foreach(FileInfo f in new DirectoryInfo("").GetFiles().Where(c=>c.LastWriteTime==DateTime.Now.Date))
                        {
                            transferResult = session.PutFiles(f.FullName, "FTPPATH" + f.Name, false, tranferOptions);
                            transferResult.Check();

                            if (transferResult.IsSuccess)
                            {
                                msg.Add(f.FullName + " Sent.");
                            }
                            else
                            {
                            //it didn't transfer
                            //proc.ftpOK = false;
                            //proc.errMsgs.Add("putFile: unable to transfer file:" + f.FullName);
                                msg.Add(f.FullName + " Unable to transfer");
                            }
                        }                                     
                }      
            }
            catch (Exception ee)
            {
                int a = 0;
                //proc.ftpOK = false;
                //proc.errMsgs.Add("clFTP putFile:" + ee.Message);
            }
            finally
            {

            }

            //return proc;
        }

     

        public static string getLocalLastFileByName(string localPath, string filePart, bool useExactFileName=false,bool getLatestByToday = true)
        {
            string s = "";

            try
            {
                if(useExactFileName==true)
                {
                    if (getLatestByToday == true)
                    {
                        s = new DirectoryInfo(localPath).GetFiles().Where(c => c.Name == filePart && c.LastWriteTime.Date==DateTime.Now.Date).Select(c=>c.Name).SingleOrDefault();
                    }
                    else
                    {
                        s = new DirectoryInfo(localPath).GetFiles().Where(c => c.Name == filePart).OrderByDescending(c=>c.LastWriteTime).Select(c => c.Name).SingleOrDefault();
                    }
                }
                else
                {
                    if(getLatestByToday==true)
                    {
                        s= new DirectoryInfo(localPath).GetFiles().Where(c => c.Name.Contains(filePart) && c.LastWriteTime.Date==DateTime.Now.Date).OrderByDescending(c => c.LastWriteTime).Select(c => c.Name).SingleOrDefault();
                    }
                    else
                    {
                        s = new DirectoryInfo(localPath).GetFiles().Where(c => c.Name.Contains(filePart)).OrderByDescending(c => c.LastWriteTime).Select(c => c.Name).SingleOrDefault();
                    }
                }
            }
            catch(Exception ee)
            {
                s = "ERROR:" + ee.Message;
            }

            return s;
        }

        public static string getLastFileByName(string filePath, Session session, string filePart,bool useExactFileName = false, bool getLatestByToday = true)
        {
            string s = "";

            try
            {
                RemoteDirectoryInfo dir = session.ListDirectory(filePath);

                if(useExactFileName==true)
                {
                    if(getLatestByToday==true)
                    {
                        s = dir.Files.Where(c => c.Name == filePart && c.IsDirectory == false && c.LastWriteTime.Date == DateTime.Now.Date).Select(c => c.Name).FirstOrDefault();
                    }
                    else
                    {
                        s = dir.Files.Where(c => c.Name == filePart && c.IsDirectory == false).OrderByDescending(c=>c.LastWriteTime).Select(c => c.Name).FirstOrDefault();
                    }
                    
                }
                else
                {
                    if (getLatestByToday == true)
                    {
                        //get a file from today only.
                        s = dir.Files.Where(c => c.Name.Contains(filePart) && c.IsDirectory == false && c.LastWriteTime.Date == DateTime.Now.Date).OrderByDescending(c => c.LastWriteTime).Select(c => c.Name).FirstOrDefault();
                    }
                    else
                    {
                        //get whatever the file is with the latest modified date.
                        s = dir.Files.Where(c => c.Name.Contains(filePart) && c.IsDirectory == false).OrderByDescending(c => c.LastWriteTime).Select(c => c.Name).FirstOrDefault();
                    }
                }

                if(s==null)
                {
                    s = "clFTP getLastFileByName ERROR: NO FILE";
                }
            }
            catch(Exception ee)
            {
                s = "clFTP getLastFileByName ERROR:" + ee.Message;
            }

            return s;
        }
    }
}
