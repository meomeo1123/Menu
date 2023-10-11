using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tester1.Models;

namespace tester1.Controllers
{
    public class SendMailController : Controller
    {
        // GET: SendMail

            private EmailService emailService;

            public SendMailController()
            {
                emailService = new EmailService();
            }

    }
}