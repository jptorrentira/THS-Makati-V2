using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Models;

namespace ShuttleService.Controllers
{
    public class SurveyTransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurveyTransactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SurveyTransactions.Include(s => s.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize]
        [HttpPost]
        public ActionResult CheckSurveyCounts()
        {
            int surveyCount = 0;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int empId = Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")); 
            var st = _context.SurveyTransactions.OrderBy(o=> o.TransactionId).Where(e => e.Status.Equals(1) && e.IsAnswered.Equals(0) && e.EmployeeId.Equals(empId)).Select(u=> new {
                u.SurveyHash,
                u.TransactionId
            });
            surveyCount = st.Count();

            var jsonData = new
            {
                surveyCount,
                surveys = st.ToList()
            };

            return new JsonResult(jsonData);
        }

        public async Task<IActionResult> AnswerSurvey(string u)
        {
            var getTrans = await _context.SurveyTransactions.FirstOrDefaultAsync(e => e.SurveyHash.Equals(u));

            ViewData["SurveyQuestion"] =  _context.SurveyQuestions.Where(e => e.Status.Equals(1)).ToList();
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> SaveSurvey(int[] SurveyQuestionId, int[] AnswerScore, string[] Remarks, string HashCode)
        {
            var returnSuccess = 1;
            var returnMessage = "";


            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var surveyTrans = _context.SurveyTransactions.FirstOrDefault(s => s.SurveyHash.Equals(HashCode) && s.IsAnswered.Equals(0));
                    if(surveyTrans == null)
                    {
                        throw new Exception("Survey Transaction not found");
                    }

                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(e => e.TransactionId.Equals(surveyTrans.TransactionId));
                    if (driverPassengerHeader != null)
                    {
                        driverPassengerHeader.Status = 10;
                        _context.Update(driverPassengerHeader);
                    }

                    surveyTrans.IsAnswered = 1;
                    surveyTrans.AnsweredDateTime = DateTime.Now;
                    _context.Update(surveyTrans);

                    int x = 0;
                    foreach(int q in SurveyQuestionId)
                    {
                        var answer = new SurveyAnswer();
                        answer.SurveyQuestionId = q;
                        answer.SurveyTransactionId = surveyTrans.Id;
                        answer.AnswerScore = AnswerScore[x];
                        answer.Remarks = Remarks[x];
                        answer.Status = 1;
                        x++;
                        _context.Add(answer);
                    }

                    await _context.SaveChangesAsync();

                    //logs
                    Log _log = new Log();
                    _log.Process = "Answered Survey for " + surveyTrans.TransactionId ;
                    _log.ProcessedBy = HttpContext.Session.GetString("Session_userDomainWithName");
                    _log.ProcessedDate = DateTime.Now;
                    _context.Add(_log);
                    //end logs

                    returnMessage = "Success";
                    returnSuccess = 1;
                    transaction.Commit();

                }
                catch (Exception e)
                {
                    returnSuccess = 0;
                    returnMessage = e.Message;
                    transaction.Rollback();
                    //throw e;
                }

            }

            var jsonData = new
            {
                message = returnMessage,
                success = returnSuccess
            };
            return new JsonResult(jsonData);

        }
    }
}
