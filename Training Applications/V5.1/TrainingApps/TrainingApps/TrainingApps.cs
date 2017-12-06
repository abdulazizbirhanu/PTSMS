#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Mvc;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.View;
using Kooboo.Web.Mvc;

namespace TrainingApps
{
    public class Maintenance : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-mro-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("firstName"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("studyProgram"),
                            certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                            terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");

                        try
                        {
                            MailMessage mail = new MailMessage();
                            mail.From = new MailAddress(email);
                            mail.To.Add("EAASBD2@ethiopianairlines.com");
                            //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                            //objMail.CC.Add(emails);
                            mail.Body =
                                "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                                "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td> " + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                                "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"background: #f2f2f2;\">Educational Background</td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                                "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\">Applying For</td></tr><tr><td><b>Applying for program</b>:</td><td> " + program + "</td></tr>" +
                                "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"></td></tr><tr><td><b>Operational and Systems Certificate</b>:</td><td> " + certificate + "</td></tr>" +
                                "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                            mail.IsBodyHtml = true;
                            mail.Subject = "Aircraft Maintenance Training Application Submission: " + firstname + " " + middlename + " " + lastname;

                            //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                            SmtpClient smtpClient = new SmtpClient("localhost", 25);
                            smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                            smtpClient.Send(mail);

                            MailMessage reply = new MailMessage();
                            reply.From = new MailAddress("EAASBD2@ethiopianairlines.com", "Ethiopian Aviation Academy");
                            reply.To.Add(email);
                            //objMail.CC.Add(emails);
                            reply.Body =

                                            "<p>Dear Applicant, </p>" +
                                            "<p>Thank you for your interest to study with Ethiopian Airlines Aviation Academy. This e-mail is sent to you to confirm receipt of your application.  The entrance exam date, physical and medical screening date will be communicated to you through telephone and e-mail you gave while registering. </p>" +
                                            "<p>You may check the requirement details of the program that you registered for from the below table:- </p>" +
                                            "<table border=\"1\">" +
                                            "<tbody>" +
                                            "<tr style=\"height: 13px; background-color: #517842; color: fff; text-align: center\">" +
                                             "<td style=\"height: 13px;\" width=\"185\"><strong> Programs</strong></td>" +
                                            "<td style=\"height: 13px;\" width=\"112\"><strong>Age Limit</strong></td>" +
                                            "<td style=\"height: 13px;\" width=\"167\"><strong>Educational Qualification</strong></td>" +
                                            "<td style=\"height: 13px;\" width=\"191\"><strong>Duration</strong></td>" +
                                            "<td style=\"height: 13px;\" width=\"327\"><strong>Requirements</strong></td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 65px; \">" +
                                             "<td style=\"height: 65px;background-color: #517842; color:fff;\" width=\"185\"><strong>Cabin Crew Training</strong></td>" +
                                            "<td style=\"height: 65px;\" width=\"112\">18-28 years</td>" +
                                            "<td style=\"height: 65px;\" width=\"167\">Min. High School graduate</td>" +
                                            "<td style=\"height: 65px;\" width=\"191\">3 months<br /> Full day on all week days</td>" +
                                            "<td style=\"height: 65px;\" width=\"327\"> - Physical Screening- Height 159 cm <br/> - 212 cm while standing on tiptoe <br/> - No tattoo or scars on visible areas, and other detailed screening applies<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 39px; \">" +
                                             "<td style=\"height: 39px;background-color: #517842; color:fff;\" width=\"185\"><strong>Customer Service (Ticket Offices, Airport, Cargo Operations, Call Center)</strong></td>" +
                                            "<td style=\"height: 39px;\" width=\"112\">Min. 18 years</td>" +
                                            "<td style=\"height: 39px;\" width=\"167\">Min. High School graduate</td>" +
                                            "<td style=\"height: 39px;\" width=\"191\">3 to 6 months<br /> Full day on all week days</td>" +
                                            "<td style=\"height: 39px;\" width=\"327\">- Written Exam applies</td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 26px; \">" +
                                             "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Aircraft Maintenance Training</strong></td>" +
                                            "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                            "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                            "<td style=\"height: 26px;\" width=\"191\">22 months<br /> Full day on all week days</td>" +
                                            "<td style=\"height: 26px;\" width=\"327\">- Physical Screening applies - Min. Height 160 cm - Medical Screening applies - Written Exam applies</td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 52px; \">" +
                                             "<td style=\"height: 52px;background-color: #517842; color:fff;\" width=\"185\"><strong>Pilot Training</strong></td>" +
                                            "<td style=\"height: 52px;\" width=\"112\">Min. 18 years</td>" +
                                            "<td style=\"height: 52px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                            "<td style=\"height: 52px;\" width=\"191\">18 to 20 months<br /> Full day on all week days</td>" +
                                            "<td style=\"height: 52px;\" width=\"327\">- Physical Screening- Min. Height 167 cm for female applicants and 170cm for male<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 26px; \">" +
                                             "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Basic Travel Agency Training</strong></td>" +
                                            "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                            "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                            "<td style=\"height: 26px;\" width=\"191\">3 months in night class</td>" +
                                            "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                            "</tr>" +
                                            "<tr style=\"height: 26px; \">" +
                                             "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>IATA Foundation in Travel &amp; Tourism</strong></td>" +
                                            "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                            "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                            "<td style=\"height: 26px;\" width=\"191\">6 months in night class;<br /> 4 months in day class</td>" +
                                            "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                            "</tr>" +
                                            "</tbody>" +
                                            "</table>" +
                                            "<p><br />If you need any additional information or clarification, please contact our office through e-mail on <a href=\"mailto:EAAINFO@ethiopianairlines.com\"> EAAINFO@ethiopianairlines.com </a> or call us at + 251 - 115174016 / 4023. </p><p>We look forward to see you training with us. </p><p>Best regards, <br/>Ethiopian Airlines Aviation Academy </p> ";

                            //reply.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/Personal History Form.pdf")));
                            reply.IsBodyHtml = true;
                            reply.Subject = "RE: Aircraft Maintenance Training Application";

                            smtpClient.Send(reply);
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        //save to database
                        var database = new Dev_CMSEntities();

                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Aircraft Maintenance Training",
                            CellPhone = cellphone,
                            CertificateType = certificate,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception e)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<b style='color: red;'>We cannot process your request right now. Please try again later</b>" + e;
                }
            }
            return null;
        }
    }
    public class Leadership : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-leadership-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("given-name"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("applying-for");
                        //certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                        //terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("EAASBD2@ethiopianairlines.com");
                        //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td> " + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                            "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                            "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Applying For</b></td></tr><tr><td><b>Program</b>:</td><td> " + program + "</td></tr>" +
                            "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        mail.IsBodyHtml = true;
                        mail.Subject = "Leadership Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Leadership Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<b style='color: red;'>We cannot process your request right now. Please try again later</b>";
                }
            }
            return null;
        }
    }
    public class CabinCrew : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-cabincrew-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("given-name"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("applying-for");
                        //certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                        //terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("EAASBD2@ethiopianairlines.com");
                        //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td> " + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                            "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                            "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Applying For</b></td></tr><tr><td><b>Program</b>:</td><td> " + program + "</td></tr>" +
                            "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        mail.IsBodyHtml = true;
                        mail.Subject = "Cabin Crew Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        MailMessage reply = new MailMessage();
                        reply.From = new MailAddress("EAASBD2@ethiopianairlines.com", "Ethiopian Aviation Academy");
                        reply.To.Add(email);
                        //objMail.CC.Add(emails);
                        reply.Body =

                                        "<p>Dear Applicant, </p>" +
                                        "<p>Thank you for your interest to study with Ethiopian Airlines Aviation Academy. This e-mail is sent to you to confirm receipt of your application.  The entrance exam date, physical and medical screening date will be communicated to you through telephone and e-mail you gave while registering. </p>" +
                                        "<p>You may check the requirement details of the program that you registered for from the below table:- </p>" +
                                        "<table border=\"1\">" +
                                        "<tbody>" +
                                        "<tr style=\"height: 13px; background-color: #517842; color: fff; text-align: center\">" +
                                         "<td style=\"height: 13px;\" width=\"185\"><strong> Programs</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"112\"><strong>Age Limit</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"167\"><strong>Educational Qualification</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"191\"><strong>Duration</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"327\"><strong>Requirements</strong></td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 65px; \">" +
                                         "<td style=\"height: 65px;background-color: #517842; color:fff;\" width=\"185\"><strong>Cabin Crew Training</strong></td>" +
                                        "<td style=\"height: 65px;\" width=\"112\">18-28 years</td>" +
                                        "<td style=\"height: 65px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 65px;\" width=\"191\">3 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 65px;\" width=\"327\"> - Physical Screening- Height 159 cm <br/> - 212 cm while standing on tiptoe <br/> - No tattoo or scars on visible areas, and other detailed screening applies<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 39px; \">" +
                                         "<td style=\"height: 39px;background-color: #517842; color:fff;\" width=\"185\"><strong>Customer Service (Ticket Offices, Airport, Cargo Operations, Call Center)</strong></td>" +
                                        "<td style=\"height: 39px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 39px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 39px;\" width=\"191\">3 to 6 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 39px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Aircraft Maintenance Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">22 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Physical Screening applies - Min. Height 160 cm - Medical Screening applies - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 52px; \">" +
                                         "<td style=\"height: 52px;background-color: #517842; color:fff;\" width=\"185\"><strong>Pilot Training</strong></td>" +
                                        "<td style=\"height: 52px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 52px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 52px;\" width=\"191\">18 to 20 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 52px;\" width=\"327\">- Physical Screening- Min. Height 167 cm for female applicants and 170cm for male<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Basic Travel Agency Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">3 months in night class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>IATA Foundation in Travel &amp; Tourism</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">6 months in night class;<br /> 4 months in day class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                        "</table>" +
                                        "<p><br />If you need any additional information or clarification, please contact our office through e-mail on <a href=\"mailto:EAAINFO@ethiopianairlines.com\"> EAAINFO@ethiopianairlines.com </a> or call us at + 251 - 115174016 / 4023. </p><p>We look forward to see you training with us. </p><p>Best regards, <br/>Ethiopian Airlines Aviation Academy </p> ";

                        //reply.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/Personal History Form.pdf")));
                        reply.IsBodyHtml = true;
                        reply.Subject = "RE: Cabin Crew Training Application";

                        smtpClient.Send(reply);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Cabin Crew Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<b style='color: red;'>We cannot process your request right now. Please try again later.</b>";
                }
            }
            return null;
        }
    }

    public class CommercialServices : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-marketing-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("firstName"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("studyProgram"),
                            agent = pageContext.ControllerContext.RequestContext.GetRequestValue("agent");
                        //certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                        //terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");
                        string body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td> " + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                            "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                            "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Applying For</b></td></tr><tr><td><b>Program</b>:</td><td> " + program + "</td></tr>";

                        if (agent == "checked")
                        {
                            body += "<tr><td><b>Special need</b>:</td><td>Foundation in Travel & Tourism/Travel Agent Training</td></tr>";
                        }

                        body += "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("ATCETA@EthiopianAirlines.com");
                        //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            body;
                        mail.IsBodyHtml = true;
                        mail.Subject = "Commercial and Services Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        MailMessage reply = new MailMessage();
                        reply.From = new MailAddress("ServiceAlert@ethiopianairlines.com", "Ethiopian Aviation Academy");
                        reply.To.Add(email);
                        //objMail.CC.Add(emails);

                        reply.Body =

                                    "<p>Dear Applicant, </p>" +
                                    "<p>Thank you for your interest to study with Ethiopian Airlines Aviation Academy. This e-mail is sent to you to confirm receipt of your application.  The entrance exam date, physical and medical screening date will be communicated to you through telephone and e-mail you gave while registering. </p>" +
                                    "<p>You may check the requirement details of the program that you registered for from the below table:- </p>" +
                                    "<table border=\"1\">" +
                                    "<tbody>" +
                                    "<tr style=\"height: 13px; background-color: #517842; color: fff; text-align: center\">" +
                                     "<td style=\"height: 13px;\" width=\"185\"><strong> Programs</strong></td>" +
                                    "<td style=\"height: 13px;\" width=\"112\"><strong>Age Limit</strong></td>" +
                                    "<td style=\"height: 13px;\" width=\"167\"><strong>Educational Qualification</strong></td>" +
                                    "<td style=\"height: 13px;\" width=\"191\"><strong>Duration</strong></td>" +
                                    "<td style=\"height: 13px;\" width=\"327\"><strong>Requirements</strong></td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 65px; \">" +
                                     "<td style=\"height: 65px;background-color: #517842; color:fff;\" width=\"185\"><strong>Cabin Crew Training</strong></td>" +
                                    "<td style=\"height: 65px;\" width=\"112\">18-28 years</td>" +
                                    "<td style=\"height: 65px;\" width=\"167\">Min. High School graduate</td>" +
                                    "<td style=\"height: 65px;\" width=\"191\">3 months<br /> Full day on all week days</td>" +
                                    "<td style=\"height: 65px;\" width=\"327\"> - Physical Screening- Height 159 cm <br/> - 212 cm while standing on tiptoe <br/> - No tattoo or scars on visible areas, and other detailed screening applies<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 39px; \">" +
                                     "<td style=\"height: 39px;background-color: #517842; color:fff;\" width=\"185\"><strong>Customer Service (Ticket Offices, Airport, Cargo Operations, Call Center)</strong></td>" +
                                    "<td style=\"height: 39px;\" width=\"112\">Min. 18 years</td>" +
                                    "<td style=\"height: 39px;\" width=\"167\">Min. High School graduate</td>" +
                                    "<td style=\"height: 39px;\" width=\"191\">3 to 6 months<br /> Full day on all week days</td>" +
                                    "<td style=\"height: 39px;\" width=\"327\">- Written Exam applies</td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 26px; \">" +
                                     "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Aircraft Maintenance Training</strong></td>" +
                                    "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                    "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                    "<td style=\"height: 26px;\" width=\"191\">22 months<br /> Full day on all week days</td>" +
                                    "<td style=\"height: 26px;\" width=\"327\">- Physical Screening applies - Min. Height 160 cm - Medical Screening applies - Written Exam applies</td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 52px; \">" +
                                     "<td style=\"height: 52px;background-color: #517842; color:fff;\" width=\"185\"><strong>Pilot Training</strong></td>" +
                                    "<td style=\"height: 52px;\" width=\"112\">Min. 18 years</td>" +
                                    "<td style=\"height: 52px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                    "<td style=\"height: 52px;\" width=\"191\">18 to 20 months<br /> Full day on all week days</td>" +
                                    "<td style=\"height: 52px;\" width=\"327\">- Physical Screening- Min. Height 167 cm for female applicants and 170cm for male<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 26px; \">" +
                                     "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Basic Travel Agency Training</strong></td>" +
                                    "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                    "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                    "<td style=\"height: 26px;\" width=\"191\">3 months in night class</td>" +
                                    "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                    "</tr>" +
                                    "<tr style=\"height: 26px; \">" +
                                     "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>IATA Foundation in Travel &amp; Tourism</strong></td>" +
                                    "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                    "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                    "<td style=\"height: 26px;\" width=\"191\">6 months in night class;<br /> 4 months in day class</td>" +
                                    "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                    "</tr>" +
                                    "</tbody>" +
                                    "</table>" +
                                    "<p><br />If you need any additional information or clarification, please contact our office through e-mail on <a href=\"mailto:EAAINFO@ethiopianairlines.com\"> EAAINFO@ethiopianairlines.com </a> or call us at + 251 - 115174016 / 4023. </p><p>We look forward to see you training with us. </p><p>Best regards, <br/>Ethiopian Airlines Aviation Academy </p> ";

                        //reply.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/Personal History Form.pdf")));


                        reply.IsBodyHtml = true;
                        reply.Subject = "RE: Commercial and Services Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        smtpClient.Send(reply);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Commercial and Services Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<b style='color: red;'>We cannot process your request right now. Please try again later</b>";
                }
            }
            return null;
        }
    }
    public class Pilot : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-pilot-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("given-name"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("applying-for");
                        //certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                        //terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("EAASBD2@ethiopianairlines.com");
                        //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td>" + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                            "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                            "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Applying For</b></td></tr><tr><td><b>Program</b>:</td><td> " + program + "</td></tr>" +
                            "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        mail.IsBodyHtml = true;
                        mail.Subject = "Pilot Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        MailMessage reply = new MailMessage();
                        reply.From = new MailAddress("EAASBD2@ethiopianairlines.com", "Ethiopian Aviation Academy");
                        reply.To.Add(email);
                        //objMail.CC.Add(emails);
                        reply.Body =

                                        "<p>Dear Applicant, </p>" +
                                        "<p>Thank you for your interest to study with Ethiopian Airlines Aviation Academy. This e-mail is sent to you to confirm receipt of your application.  The entrance exam date, physical and medical screening date will be communicated to you through telephone and e-mail you gave while registering. </p>" +
                                        "<p>You may check the requirement details of the program that you registered for from the below table:- </p>" +
                                        "<table border=\"1\">" +
                                        "<tbody>" +
                                        "<tr style=\"height: 13px; background-color: #517842; color: fff; text-align: center\">" +
                                         "<td style=\"height: 13px;\" width=\"185\"><strong> Programs</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"112\"><strong>Age Limit</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"167\"><strong>Educational Qualification</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"191\"><strong>Duration</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"327\"><strong>Requirements</strong></td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 65px; \">" +
                                         "<td style=\"height: 65px;background-color: #517842; color:fff;\" width=\"185\"><strong>Cabin Crew Training</strong></td>" +
                                        "<td style=\"height: 65px;\" width=\"112\">18-28 years</td>" +
                                        "<td style=\"height: 65px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 65px;\" width=\"191\">3 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 65px;\" width=\"327\"> - Physical Screening- Height 159 cm <br/> - 212 cm while standing on tiptoe <br/> - No tattoo or scars on visible areas, and other detailed screening applies<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 39px; \">" +
                                         "<td style=\"height: 39px;background-color: #517842; color:fff;\" width=\"185\"><strong>Customer Service (Ticket Offices, Airport, Cargo Operations, Call Center)</strong></td>" +
                                        "<td style=\"height: 39px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 39px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 39px;\" width=\"191\">3 to 6 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 39px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Aircraft Maintenance Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">22 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Physical Screening applies - Min. Height 160 cm - Medical Screening applies - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 52px; \">" +
                                         "<td style=\"height: 52px;background-color: #517842; color:fff;\" width=\"185\"><strong>Pilot Training</strong></td>" +
                                        "<td style=\"height: 52px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 52px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 52px;\" width=\"191\">18 to 20 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 52px;\" width=\"327\">- Physical Screening- Min. Height 167 cm for female applicants and 170cm for male<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Basic Travel Agency Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">3 months in night class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>IATA Foundation in Travel &amp; Tourism</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">6 months in night class;<br /> 4 months in day class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                        "</table>" +
                                        "<p><br />If you need any additional information or clarification, please contact our office through e-mail on <a href=\"mailto:EAAINFO@ethiopianairlines.com\"> EAAINFO@ethiopianairlines.com </a> or call us at + 251 - 115174016 / 4023. </p><p>We look forward to see you training with us. </p><p>Best regards, <br/>Ethiopian Airlines Aviation Academy </p> ";

                        //reply.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/Personal History Form.pdf")));
                        reply.IsBodyHtml = true;
                        reply.Subject = "RE: Pilot Training Application";

                        smtpClient.Send(reply);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Pilot Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<br/><b style='color: red;'>We cannot process your request right now. Please try again later.</b>";
                }
            }
            return null;
        }
    }

    public class Language : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-language-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            salutation = pageContext.ControllerContext.RequestContext.GetRequestValue("salutation"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("given-name"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            kifleketema = pageContext.ControllerContext.RequestContext.GetRequestValue("kifle-ketema"),
                            kebele = pageContext.ControllerContext.RequestContext.GetRequestValue("kebele"),
                            housenumber = pageContext.ControllerContext.RequestContext.GetRequestValue("house-no"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            pobox = pageContext.ControllerContext.RequestContext.GetRequestValue("po-box"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            //confemail = pageContext.ControllerContext.RequestContext.GetRequestValue("conf-email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education"),
                            program = pageContext.ControllerContext.RequestContext.GetRequestValue("applying-for");
                        //certificate = pageContext.ControllerContext.RequestContext.GetRequestValue("certificateType"),
                        //terms = pageContext.ControllerContext.RequestContext.GetRequestValue("terms");

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("LCDC@ethiopianairlines.com");
                        //mail.To.Add("mesfins@ethiopianairlines.com,stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + salutation + " " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td> " + country + "</td></tr>";
                        if (country == "Ethiopia")
                            mail.Body += "<tr><td><b>Kifle ketema</b>:</td><td> " + kifleketema + "</td></tr><tr><td><b>Kebele</b>:</td><td> " + kebele + "</td></tr><tr><td><b>House number</b>:</td><td> " + housenumber + "</td></tr>";
                        else
                            mail.Body += "<tr><td><b>City</b>:</td><td> " + city + "</td></tr>";
                        mail.Body +=
                                "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                                "<tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Applying For</b></td></tr><tr><td><b>Program</b>:</td><td> " + program + "</td></tr>" +
                                "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        mail.IsBodyHtml = true;
                        mail.Subject = "Language and Communication Development Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw03.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        MailMessage replyMail = new MailMessage();
                        replyMail.From = new MailAddress("ServiceAlert@ethiopianairlines.com", "Ethiopian Aviation Academy");
                        replyMail.To.Add(email);
                        //objMail.CC.Add(emails);
                        replyMail.Body =

                                        "<p>Dear Applicant, </p>" +
                                        "<p>Thank you for your interest to study with Ethiopian Airlines Aviation Academy. This e-mail is sent to you to confirm receipt of your application.  The entrance exam date, physical and medical screening date will be communicated to you through telephone and e-mail you gave while registering. </p>" +
                                        "<p>You may check the requirement details of the program that you registered for from the below table:- </p>" +
                                        "<table border=\"1\">" +
                                        "<tbody>" +
                                        "<tr style=\"height: 13px; background-color: #517842; color: fff; text-align: center\">" +
                                         "<td style=\"height: 13px;\" width=\"185\"><strong> Programs</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"112\"><strong>Age Limit</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"167\"><strong>Educational Qualification</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"191\"><strong>Duration</strong></td>" +
                                        "<td style=\"height: 13px;\" width=\"327\"><strong>Requirements</strong></td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 65px; \">" +
                                         "<td style=\"height: 65px;background-color: #517842; color:fff;\" width=\"185\"><strong>Cabin Crew Training</strong></td>" +
                                        "<td style=\"height: 65px;\" width=\"112\">18-28 years</td>" +
                                        "<td style=\"height: 65px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 65px;\" width=\"191\">3 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 65px;\" width=\"327\"> - Physical Screening- Height 159 cm <br/> - 212 cm while standing on tiptoe <br/> - No tattoo or scars on visible areas, and other detailed screening applies<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 39px; \">" +
                                         "<td style=\"height: 39px;background-color: #517842; color:fff;\" width=\"185\"><strong>Customer Service (Ticket Offices, Airport, Cargo Operations, Call Center)</strong></td>" +
                                        "<td style=\"height: 39px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 39px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 39px;\" width=\"191\">3 to 6 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 39px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Aircraft Maintenance Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">22 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Physical Screening applies - Min. Height 160 cm - Medical Screening applies - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 52px; \">" +
                                         "<td style=\"height: 52px;background-color: #517842; color:fff;\" width=\"185\"><strong>Pilot Training</strong></td>" +
                                        "<td style=\"height: 52px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 52px;\" width=\"167\">Min. High School graduate<br /> in Natural Science field</td>" +
                                        "<td style=\"height: 52px;\" width=\"191\">18 to 20 months<br /> Full day on all week days</td>" +
                                        "<td style=\"height: 52px;\" width=\"327\">- Physical Screening- Min. Height 167 cm for female applicants and 170cm for male<br /> - Medical Screening applies<br /> - Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>Basic Travel Agency Training</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">3 months in night class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "<tr style=\"height: 26px; \">" +
                                         "<td style=\"height: 26px;background-color: #517842; color:fff;\" width=\"185\"><strong>IATA Foundation in Travel &amp; Tourism</strong></td>" +
                                        "<td style=\"height: 26px;\" width=\"112\">Min. 18 years</td>" +
                                        "<td style=\"height: 26px;\" width=\"167\">Min. High School graduate</td>" +
                                        "<td style=\"height: 26px;\" width=\"191\">6 months in night class;<br /> 4 months in day class</td>" +
                                        "<td style=\"height: 26px;\" width=\"327\">- Written Exam applies</td>" +
                                        "</tr>" +
                                        "</tbody>" +
                                        "</table>" +
                                        "<p><br />If you need any additional information or clarification, please contact our office through e-mail on <a href=\"mailto:EAAINFO@ethiopianairlines.com\"> EAAINFO@ethiopianairlines.com </a> or call us at + 251 - 115174016 / 4023. </p><p>We look forward to see you training with us. </p><p>Best regards, <br/>Ethiopian Airlines Aviation Academy </p> ";

                        replyMail.IsBodyHtml = true;
                        replyMail.Subject = "RE: Language and Communication Development Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        smtpClient.Send(replyMail);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            ApplyingForProgram = program,
                            Category = "Language and Communication Development Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename,
                            Salutation = salutation
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<b style='color: red;'>We cannot process your request right now. Please try again later</b> ";
                }
            }
            return null;
        }
    }
    public class Summer : IPagePlugin
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(Page_Context pageContext, PagePositionContext positionContext)
        {
            if (pageContext.ControllerContext.HttpContext.Request.HttpMethod == "POST")
            {
                try
                {
                    if (pageContext.ControllerContext.RequestContext.GetRequestValue("submit-btn") == "submit-pilot-application")
                    {
                        string
                            country = pageContext.ControllerContext.RequestContext.GetRequestValue("country"),
                            city = pageContext.ControllerContext.RequestContext.GetRequestValue("city"),
                            firstname = pageContext.ControllerContext.RequestContext.GetRequestValue("given-name"),
                            middlename = pageContext.ControllerContext.RequestContext.GetRequestValue("middle-name"),
                            lastname = pageContext.ControllerContext.RequestContext.GetRequestValue("last-name"),
                            gender = pageContext.ControllerContext.RequestContext.GetRequestValue("gender"),
                            homephone = pageContext.ControllerContext.RequestContext.GetRequestValue("home-phone"),
                            cellphone = pageContext.ControllerContext.RequestContext.GetRequestValue("cell-phone"),
                            email = pageContext.ControllerContext.RequestContext.GetRequestValue("email"),
                            education = pageContext.ControllerContext.RequestContext.GetRequestValue("education");

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(email);
                        mail.To.Add("EAASBD2@ethiopianairlines.com");
                        //mail.To.Add("stephanosa@ethiopianairlines.com");
                        //objMail.CC.Add(emails);
                        mail.Body =
                            "<table><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Private Applicant Form Result</b></td></tr><tr><td style=\"width: 20%;\"><b>Full name</b>:</td><td> " + firstname + " " + middlename + " " + lastname + "</td></tr>" +
                            "<tr><td><b>Gender</b>:</td><td> " + gender + "</td></tr><tr><td><b>Country</b>:</td><td>" + country + "</td></tr><tr><td><b>City</b>:</td><td> " + city + "</td></tr>" +
                            "<tr><td><b>Home phone</b>:</td><td> " + homephone + "</td></tr>" + "<tr><td><b>Cell phone</b>:</td><td> " + cellphone + "</td></tr><tr><td><b>E-Mail</b>:</td><td> " + email + "</td></tr><tr><td colspan=\"2\" style=\"color:white;background: #517842;\"><b>Educational Background</b></td></tr><tr><td><b>Education</b>:</td><td> " + education + "</td></tr>" +
                            "<tr><td><b>Submitted on</b>:</td><td> " + DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss") + "</td></tr></table>";

                        mail.IsBodyHtml = true;
                        mail.Subject = "Summer Training Application Submission:" + " " + firstname + " " + middlename + " " + lastname;

                        //SmtpClient smtpClient = new SmtpClient("svhqsgw02.ethiopianairlines.com", 25);
                        SmtpClient smtpClient = new SmtpClient("localhost", 25);
                        smtpClient.Credentials = new NetworkCredential("ServiceAlert", "abcd@1234");
                        smtpClient.Send(mail);

                        MailMessage reply = new MailMessage();
                        reply.From = new MailAddress("EAASBD2@ethiopianairlines.com", "Ethiopian Aviation Academy");
                        reply.To.Add(email);
                        //objMail.CC.Add(emails);
                        reply.Body =
                            "<p>Thank you for registering for our special summer program. Please be informed of the following important details:- </p>" +
                            "<p>Training starts on - 17th July 2017</p>" +
                            "<p>Duration - 80 hours (Monday – Friday during working hours)</p>" +
                            "<p>Payment - ETB2500.00 (excluding 15 % VAT)</p>" +
                            "<p>For further information, please call us at 115174016 / 4023.</p>" +
                            "<p>Looking forward to see you in the training.</p>" +
                            "<p>Ethiopian Aviation Academy</p>";
                        //reply.Attachments.Add(new Attachment(System.Web.HttpContext.Current.Server.MapPath("~/Attachments/Personal History Form.pdf")));
                        reply.IsBodyHtml = true;
                        reply.Subject = "RE: Summer Training Application";

                        smtpClient.Send(reply);

                        //save to database
                        var database = new Dev_CMSEntities();
                        TrainingApplication application = new TrainingApplication
                        {
                            Category = "Summer Training",
                            CellPhone = cellphone,
                            City = city,
                            Country = country,
                            EducationalLevel = education,
                            Email = email,
                            FirstName = firstname,
                            Gender = gender,
                            HomePhone = homephone,
                            LastName = lastname,
                            MiddleName = middlename
                        };
                        //database.TrainingApplications.Add(application);
                        //database.SaveChanges();
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://svhqbas01/");
                        //client.BaseAddress = new Uri("http://localhost:57570/");
                        client.DefaultRequestHeaders.Accept.Clear();

                        HttpResponseMessage ApiResponse = client.PostAsJsonAsync("TraineeInfoBOES/Create", application).Result;
                        pageContext.ControllerContext.Controller.TempData["Result"] = "<b style='color: #517842;'>Your application is submitted successfully. Please check your email for further information.</b>";
                    }
                }
                catch (Exception e)
                {
                    pageContext.ControllerContext.Controller.TempData["Errors"] = "<br/><b style='color: red;'>We cannot process your request right now. Please try again later.</b>" + e.Message;
                }
            }
            return null;
        }
    }
}
