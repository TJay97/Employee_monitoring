using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;

namespace ReadFromLog
{
    class Program
    {

        static SqlConnection con = new SqlConnection("Server=tcp:studentmanagementsys.database.windows.net,1433;Initial Catalog=BranchMonitoringSystem;Persist Security Info=False;User ID=studentmgt;Password=corona@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        //static string textFile = @"C:\Users\Thisal Jayasekara\Downloads\FaceRecognition_DressDetection_Final\webcam_FrontCamera.log";
        static string textFile = @"D:\Research Projects\567 Video Employee Detection\Project 05\FINAL CODE\FaceRecognition_DressDetection_Final\webcam_FrontCamera.log";
        static string textFileOutCamera = @"D:\Research Projects\567 Video Employee Detection\Project 05\FINAL CODE\FaceRecognition_DressDetection_Final\webcam_FrontCameraOut.log";
        static string Faces = "Faces:";
        static string GetTime = "GetTime:";
        static string Timestamp = "Timestamp:";
        static string Names = "Names:";
        static string Dresses = "Dresses:";
        static string EndLine = "EndLine";


        #region old code
        //static string EmployeeName1 = "kumudu";
        //static string EmployeeName2 = "supun";
        //static string EmployeeName3 = "Oshen";
        //static bool isInEmp1 = false;
        //static bool isInEmp2 = false;
        //static bool isInEmp3 = false;
        //static int currentSecondEmployee01 = -1;
        //static int currentSecondEmployee02 = -1;
        //static int currentSecondEmployee03 = -1;
        #endregion old code

        static List<EmployeeVM> employeeVMs = new List<EmployeeVM>();

        static DateTime mainDateTime = Convert.ToDateTime("2020-07-01 08:00:00");

        static void Main(string[] args)
        {
            try
            {
                employeeVMs.Add(new EmployeeVM()
                {
                    CurrentTimeStamp = -1,
                    IsInEmployee = false,
                    EmployeeName = "haritha",
                    EmployeeInOutVM = new List<EmployeeInOutVM>()
                });
                employeeVMs.Add(new EmployeeVM()
                {
                    CurrentTimeStamp = -1,
                    IsInEmployee = false,
                    EmployeeName = "pamudi",
                    EmployeeInOutVM = new List<EmployeeInOutVM>()
                });

                IList<FaceDressVM> faceDressVMs = new List<FaceDressVM>();
                string[] lines = File.ReadAllLines(textFile);
                foreach (var item in employeeVMs)
                {
                    FaceDressVM faceDressVM = new FaceDressVM();
                    foreach (string line in lines)
                    {
                        if (line.Contains(Faces))
                        {
                            faceDressVM.FaceCount = int.Parse(line.Substring(line.IndexOf(Faces) + Faces.Length));
                        }
                        if (line.Contains(GetTime))
                        {
                            faceDressVM.DateTimeNow = DateTime.Parse(line.Substring(line.IndexOf(GetTime) + GetTime.Length));
                        }
                        if (line.Contains(Timestamp))
                        {
                            faceDressVM.TimeStamp = Decimal.Parse(line.Substring(line.IndexOf(Timestamp) + Timestamp.Length));
                            faceDressVM.Second = Convert.ToInt32(Math.Floor(faceDressVM.TimeStamp / 1000));
                        }
                        if (line.Contains(Names))
                        {
                            string nameList = line.Substring(line.IndexOf(Names) + Names.Length);
                            nameList = nameList.Substring(1, nameList.Length - 1);
                            nameList = nameList.Remove(nameList.Length - 1);

                            faceDressVM.Names = nameList;
                        }
                        if (line.Contains(Dresses))
                        {
                            string dressList = line.Substring(line.IndexOf(Dresses) + Dresses.Length);
                            dressList = dressList.Substring(1, dressList.Length - 1);
                            dressList = dressList.Remove(dressList.Length - 1);

                            faceDressVM.Dresses = dressList;
                        }
                        if (line.Contains(EndLine))
                        {
                            if (faceDressVM.Names.Contains(item.EmployeeName) && item.CurrentTimeStamp < faceDressVM.Second)
                            {
                                item.EmployeeInOutVM.Add(new EmployeeInOutVM()
                                {
                                    EmployeeName = item.EmployeeName,
                                    Second = faceDressVM.Second,
                                    In = true,
                                    Dress = faceDressVM.Dresses
                                });

                                item.CurrentTimeStamp = faceDressVM.Second;
                            }
                            faceDressVMs.Add(faceDressVM);
                            faceDressVM = new FaceDressVM();
                        }
                    }
                }

                string[] linesOutCamera = File.ReadAllLines(textFileOutCamera);

                foreach (var item in employeeVMs)
                {
                    item.CurrentTimeStamp = -1;
                    FaceDressVM faceDressVM = new FaceDressVM();
                    foreach (string line in linesOutCamera)
                    {
                        if (line.Contains(Faces))
                        {
                            faceDressVM.FaceCount = int.Parse(line.Substring(line.IndexOf(Faces) + Faces.Length));
                        }
                        if (line.Contains(GetTime))
                        {
                            faceDressVM.DateTimeNow = DateTime.Parse(line.Substring(line.IndexOf(GetTime) + GetTime.Length));
                        }
                        if (line.Contains(Timestamp))
                        {
                            faceDressVM.TimeStamp = Decimal.Parse(line.Substring(line.IndexOf(Timestamp) + Timestamp.Length));
                            faceDressVM.Second = Convert.ToInt32(Math.Floor(faceDressVM.TimeStamp / 1000));
                        }
                        if (line.Contains(Names))
                        {
                            string nameList = line.Substring(line.IndexOf(Names) + Names.Length);
                            nameList = nameList.Substring(1, nameList.Length - 1);
                            nameList = nameList.Remove(nameList.Length - 1);

                            faceDressVM.Names = nameList;
                        }
                        if (line.Contains(Dresses))
                        {
                            string dressList = line.Substring(line.IndexOf(Dresses) + Dresses.Length);
                            dressList = dressList.Substring(1, dressList.Length - 1);
                            dressList = dressList.Remove(dressList.Length - 1);

                            faceDressVM.Dresses = dressList;
                        }
                        if (line.Contains(EndLine))
                        {
                            if (faceDressVM.Names.Contains(item.EmployeeName) && item.CurrentTimeStamp < faceDressVM.Second)
                            {
                                item.EmployeeInOutVM.Add(new EmployeeInOutVM()
                                {
                                    EmployeeName = item.EmployeeName,
                                    Second = faceDressVM.Second,
                                    In = false
                                });

                                item.CurrentTimeStamp = faceDressVM.Second;
                            }
                            faceDressVMs.Add(faceDressVM);
                            faceDressVM = new FaceDressVM();
                        }
                    }
                }

                #region old code
                //List<EmployeeInOutVM> employee01 = new List<EmployeeInOutVM>();
                //List<EmployeeInOutVM> employee02 = new List<EmployeeInOutVM>();
                //List<EmployeeInOutVM> employee03 = new List<EmployeeInOutVM>();

                //IList<FaceDressVM> faceDressVMs = new List<FaceDressVM>();
                //string[] lines = File.ReadAllLines(textFile);
                //FaceDressVM faceDressVM = new FaceDressVM();
                //foreach (string line in lines)
                //{
                //    if (line.Contains(Faces))
                //    {
                //        faceDressVM.FaceCount = int.Parse(line.Substring(line.IndexOf(Faces) + Faces.Length));
                //    }
                //    if (line.Contains(GetTime))
                //    {
                //        faceDressVM.DateTimeNow = DateTime.Parse(line.Substring(line.IndexOf(GetTime) + GetTime.Length));
                //    }
                //    if (line.Contains(Timestamp))
                //    {
                //        faceDressVM.TimeStamp = Decimal.Parse(line.Substring(line.IndexOf(Timestamp) + Timestamp.Length));
                //        faceDressVM.Second = Convert.ToInt32(Math.Floor(faceDressVM.TimeStamp / 1000));
                //    }
                //    if (line.Contains(Names))
                //    {
                //        string nameList = line.Substring(line.IndexOf(Names) + Names.Length);
                //        nameList = nameList.Substring(1, nameList.Length - 1);
                //        nameList = nameList.Remove(nameList.Length - 1);

                //        faceDressVM.Names = nameList;
                //    }
                //    if (line.Contains(Dresses))
                //    {
                //        string dressList = line.Substring(line.IndexOf(Dresses) + Dresses.Length);
                //        dressList = dressList.Substring(1, dressList.Length - 1);
                //        dressList = dressList.Remove(dressList.Length - 1);

                //        faceDressVM.Dresses = dressList;
                //    }
                //    if (line.Contains(EndLine))
                //    {
                //        if (faceDressVM.Names.Contains(EmployeeName1) && currentSecondEmployee01 < faceDressVM.Second)
                //        {
                //            isInEmp1 = !isInEmp1;
                //            employee01.Add(new EmployeeInOutVM()
                //            {
                //                EmployeeName = EmployeeName1,
                //                Second = faceDressVM.Second,
                //                In = isInEmp1,
                //                Dress = faceDressVM.Dresses
                //            });

                //            currentSecondEmployee01 = faceDressVM.Second;
                //        }
                //        if (faceDressVM.Names.Contains(EmployeeName2) && currentSecondEmployee02 < faceDressVM.Second)
                //        {
                //            isInEmp2 = !isInEmp2;
                //            employee02.Add(new EmployeeInOutVM()
                //            {
                //                EmployeeName = EmployeeName2,
                //                Second = faceDressVM.Second,
                //                In = isInEmp2,
                //                Dress = faceDressVM.Dresses
                //            });

                //            currentSecondEmployee02 = faceDressVM.Second;
                //        }
                //        if (faceDressVM.Names.Contains(EmployeeName3) && currentSecondEmployee03 < faceDressVM.Second)
                //        {
                //            isInEmp3 = !isInEmp3;
                //            employee03.Add(new EmployeeInOutVM()
                //            {
                //                EmployeeName = EmployeeName3,
                //                Second = faceDressVM.Second,
                //                In = isInEmp3,
                //                Dress = faceDressVM.Dresses
                //            });

                //            currentSecondEmployee03 = faceDressVM.Second;
                //        }
                //        faceDressVMs.Add(faceDressVM);
                //        faceDressVM = new FaceDressVM();
                //    }
                //}
                #endregion old code


                //string htmlBody = getHtml(employee01,EmployeeName1, employee02, EmployeeName2, employee03, EmployeeName3);
                //string htmlBody = getHtml(employeeVMs);
                //SendEmail(htmlBody);


                con.Open();

                foreach (var item in employeeVMs)
                {
                    SqlCommand cmd = new SqlCommand("[dbo].[Insert_EmployeeInOut]", con);
                    cmd.Parameters.Add("@Employee", SqlDbType.VarChar).Value = JsonConvert.SerializeObject(item.EmployeeInOutVM);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
        }

        public static void SendEmail(string htmlString)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("hi@sarvodayafusion01.onmicrosoft.com");
                message.To.Add(new MailAddress("kaviliya123@gmail.com"));
                message.To.Add(new MailAddress("thilini1997@gmail.com")); 
                message.Subject = "Employee progress report for " + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;

                SmtpClient client = new SmtpClient();
                client.Host = "smtp.office365.com";
                client.Credentials = new System.Net.NetworkCredential("hi@sarvodayafusion01.onmicrosoft.com", "Cortana@123");
                //client.Port = 25;
                client.EnableSsl = true;
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public static string getHtml(List<EmployeeInOutVM> employee01, string Employee01Name, List<EmployeeInOutVM> employee02, string Employee02Name, List<EmployeeInOutVM> employee03, string Employee03Name)
        //{
        //    try
        //    {
        //        string messageBody = "<font>The following are the records for Employees </font><br><br>";

        //        string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
        //        string htmlTimeBreakCaption = "<caption> "+ Employee01Name + " </caption>";
        //        string htmlTimeBreakCaption2 = "<caption>  " + Employee02Name + " </caption>";
        //        string htmlTimeBreakCaption3 = "<caption>  " + Employee03Name + " </caption>";
        //        string htmlTableEnd = "</table>";
        //        string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
        //        string htmlHeaderRowEnd = "</tr>";
        //        string htmlTrStart = "<tr style=\"color:#555555;\">";
        //        string htmlTrEnd = "</tr>";
        //        string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
        //        string htmlTdEnd = "</td>";
        //        messageBody += htmlTableStart;
        //        messageBody += htmlTimeBreakCaption;
        //        messageBody += htmlHeaderRowStart;
        //        messageBody += htmlTdStart + "Log Time" + htmlTdEnd;
        //        messageBody += htmlTdStart + "In/Out" + htmlTdEnd;
        //        messageBody += htmlTdStart + "Attire" + htmlTdEnd;
        //        messageBody += htmlHeaderRowEnd;
        //        for (int i = 0; i <= employee01.Count - 1; i++)
        //        {
        //            messageBody = messageBody + htmlTrStart;
        //            messageBody = messageBody + htmlTdStart + GetTimeFromSecond(employee01[i].Second) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + getInOut(employee01[i].In) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + employee01[i].Dress + htmlTdEnd;
        //            messageBody = messageBody + htmlTrEnd;
        //        }
        //        messageBody = messageBody + htmlTableEnd;

        //        messageBody = messageBody + "<br><br>";

        //        messageBody += htmlTableStart;
        //        messageBody += htmlTimeBreakCaption2;
        //        messageBody += htmlHeaderRowStart;
        //        messageBody += htmlTdStart + "Log Time" + htmlTdEnd;
        //        messageBody += htmlTdStart + "In/Out" + htmlTdEnd;
        //        messageBody += htmlTdStart + "Attire" + htmlTdEnd;
        //        messageBody += htmlHeaderRowEnd;
        //        for (int i = 0; i <= employee02.Count - 1; i++)
        //        {
        //            messageBody = messageBody + htmlTrStart;
        //            messageBody = messageBody + htmlTdStart + GetTimeFromSecond(employee02[i].Second) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + getInOut(employee02[i].In) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + employee02[i].Dress + htmlTdEnd;
        //            messageBody = messageBody + htmlTrEnd;
        //        }
        //        messageBody = messageBody + htmlTableEnd;


        //        messageBody = messageBody + "<br><br>";

        //        messageBody += htmlTableStart;
        //        messageBody += htmlTimeBreakCaption3;
        //        messageBody += htmlHeaderRowStart;
        //        messageBody += htmlTdStart + "Log Time" + htmlTdEnd;
        //        messageBody += htmlTdStart + "In/Out" + htmlTdEnd;
        //        messageBody += htmlTdStart + "Attire" + htmlTdEnd;
        //        messageBody += htmlHeaderRowEnd;
        //        for (int i = 0; i <= employee02.Count - 1; i++)
        //        {
        //            messageBody = messageBody + htmlTrStart;
        //            messageBody = messageBody + htmlTdStart + GetTimeFromSecond(employee03[i].Second) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + getInOut(employee03[i].In) + htmlTdEnd;
        //            messageBody = messageBody + htmlTdStart + employee03[i].Dress + htmlTdEnd;
        //            messageBody = messageBody + htmlTrEnd;
        //        }
        //        messageBody = messageBody + htmlTableEnd;


        //        return messageBody;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public static string getHtml(List<EmployeeVM> employees)
        {
            try
            {
                string messageBody = "<font>The following are the records for Employees </font><br><br>";

                foreach (var item in employees)
                {
                    string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                    string htmlTimeBreakCaption = "<caption> " + item.EmployeeName + " </caption>";
                    string htmlTableEnd = "</table>";
                    string htmlHeaderRowStart = "<tr style=\"background-color:#6FA1D2; color:#ffffff;\">";
                    string htmlHeaderRowEnd = "</tr>";
                    string htmlTrStart = "<tr style=\"color:#555555;\">";
                    string htmlTrEnd = "</tr>";
                    string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                    string htmlTdEnd = "</td>";
                    messageBody += htmlTableStart;
                    messageBody += htmlTimeBreakCaption;
                    messageBody += htmlHeaderRowStart;
                    messageBody += htmlTdStart + "Log Time" + htmlTdEnd;
                    messageBody += htmlTdStart + "In/Out" + htmlTdEnd;
                    messageBody += htmlTdStart + "Attire" + htmlTdEnd;
                    messageBody += htmlHeaderRowEnd;
                    for (int i = 0; i <= item.EmployeeInOutVM.Count - 1; i++)
                    {
                        messageBody = messageBody + htmlTrStart;
                        messageBody = messageBody + htmlTdStart + GetTimeFromSecond(item.EmployeeInOutVM[i].Second) + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + getInOut(item.EmployeeInOutVM[i].In) + htmlTdEnd;
                        messageBody = messageBody + htmlTdStart + item.EmployeeInOutVM[i].Dress + htmlTdEnd;
                        messageBody = messageBody + htmlTrEnd;
                    }
                    messageBody = messageBody + htmlTableEnd;
                    messageBody = messageBody + "<br><br><br><br>";
                }
                


                return messageBody;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GetTimeFromSecond(int seconds)
        {
            DateTime newDate = mainDateTime.AddSeconds(seconds);
            string hour = newDate.Hour.ToString().PadLeft(2, '0');
            string minute = newDate.Minute.ToString().PadLeft(2, '0');
            string second = newDate.Second.ToString().PadLeft(2, '0');
            return (hour + ":" + minute + ":" + second);
        }

        public static string getInOut(bool inOut)
        {
            if (inOut)
            {
                return "In";
            }else
            {
                return "Out";
            }
        }
    }
}
