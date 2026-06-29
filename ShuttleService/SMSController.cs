using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShuttleService.Models;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Controllers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShuttleService
{
    [Route("api/ssrs")]
    public class SMSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SMSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        private void ResetContextState() => _context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        // POST api/<controller>
        [HttpPost]
        public async Task<string> PostAsync([FromBody]SmsViewModel model)
        {
            //logs
            Log _logs = new Log();
            _logs.Process = "Connected To SMS API";
            _logs.ProcessedBy = model.MobileNo;
            _logs.ProcessedDate = DateTime.Now;
            _context.Add(_logs);
            _context.SaveChanges();
            //end logs

            //ResetContextState();
            var returnSuccess = 1;
            var returnMessage = "";
            int approveCounter = 0;
            string _message = "";
          
            //using (var transaction = _context.Database.BeginTransaction())
            //{
            try
            {

              
                int SmsCounter = _context.SmsTransactionCodes.Count(s=> s.ReferenceNo.Equals(model.ReferenceNo));
                
                if (SmsCounter > 0)
                {   
                    var smsTransaction = _context.SmsTransactionCodes.FirstOrDefault(s => s.ReferenceNo.Equals(model.ReferenceNo));
                        if (smsTransaction.ReservationTypeId == 1) { //shuttle
                        //approve or reject shuttle reservation

                                        var shuttlePassenger = _context.ShuttlePassengers.Where(s => s.SmsId == model.ReferenceNo && s.Status == 1);
                       
                        if (shuttlePassenger.Count() > 0)
                                        {
                                            await shuttlePassenger.ForEachAsync(e => {
                                                e.Status = Convert.ToInt32(model.Status);
                                                e.ApprovedBy = model.MobileNo;
                                                e.ApprovedDatetime = model.ReplyDateTime;

                                                //sending message to requestor
                                                string _MobileNumber = "";
                                                if (e.PassengerTypeId == 1)
                                                {
                                                    //_MobileNumber = _context.Employees.AsNoTracking().FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                                }
                                                else
                                                {
                                                    _MobileNumber = e.ContactNo;
                                                }

                                                    if (Convert.ToInt32(model.Status) == 2)
                                                    {
                                                        _message = "Your request with THSR ID " + e.TransactionId + " is now approved and is now queued for assigning of vehicle.";
                                                    }
                                                    else if (Convert.ToInt32(model.Status) == 3)
                                                    {
                                                        _message = "Your request with THSR ID " + e.TransactionId + " has been disapproved. Please contact your supervisor for confirmation. ";
                                                    }
                                                    else
                                                    {
                                                        _message = "";
                                                    }

                                                //new ReportsController().SendSMS(_message, _MobileNumber); //JPT commented code 09042024

                                                ////logs
                                                //Log _logsSaving = new Log();
                                                //_logsSaving.Process = "Saving approval...";
                                                //_logsSaving.ProcessedBy = model.MobileNo;
                                                //_logsSaving.ProcessedDate = DateTime.Now;
                                                //_context.Add(_logsSaving);
                                                //_context.SaveChanges();
                                                ////end logs

                                                //new Email notif EBE - ECQ - 05072020

                                                //var _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                                //if (_Email != "")
                                                //{
                                                //    var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                                //    new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Shuttle Service");
                                                //}
                                                //end new email Notif EBE - ECQ - 05072020
                                                //end
                                            });


                                            smsTransaction.Status = Convert.ToInt32(model.Status);
                                            smsTransaction.Remarks = model.Remarks;
                                            _context.Update(smsTransaction);
                                            //_context.SaveChanges();
                                            approveCounter++;

                                            ////logs
                                            Log _logsSaving = new Log();
                                            _logsSaving.Process = "Saving approval...";
                                            _logsSaving.ProcessedBy = model.MobileNo;
                                            _logsSaving.ProcessedDate = DateTime.Now;
                                            _context.Add(_logsSaving);
                                            _context.SaveChanges();
                                            ////end logs
                                        }


                                        //logs
                                        Log _log = new Log();

                                        if (model.Status == "2") _log.Process = "Approved Reservation Trans No.: " + smsTransaction.TransactionId + " via SMS. Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;
                                        else if (model.Status == "3") _log.Process = "Rejected Reservation Trans No.: " + smsTransaction.TransactionId + " via SMS. Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;
                                        else _log.Process = "SMS Approval Code Invalid. Code: " + model.Status + " Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;

                                        _log.ProcessedBy = model.MobileNo;
                                        _log.ProcessedDate = DateTime.Now;
                                        _context.Add(_log);
                                        //end logs

                           

                                    if (approveCounter <= 0)
                                    {
                                        throw new System.Exception("There is no reservation to approve.");
                                    }

                        //end approve or reject shuttle reservation
                        }
                        else if (smsTransaction.ReservationTypeId == 2)
                        { //driver
                          //approve or reject driver reservation
                            int checkthenumbers = 0;
                            var driverPassenger = _context.DriverPassengers.Where(s => s.SmsId == model.ReferenceNo && s.Status.Equals(1));
                            int driverPassengerCount  = _context.DriverPassengers.Count(s => s.SmsId == model.ReferenceNo && s.Status.Equals(1));
                      

                            if (driverPassenger.Count() > 0)
                            {
                                await driverPassenger.ForEachAsync(e =>
                                {
                                    e.Status = Convert.ToInt32(model.Status);
                                    e.ApprovedBy = model.MobileNo;
                                    e.ApprovedDatetime = model.ReplyDateTime;
                                    checkthenumbers++;


                                    //sending message to requestor
                                        string _MobileNumber = "";
                                        if (e.PassengerTypeId == 1)
                                        {
                                            _MobileNumber = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).MobileNumber;
                                        }
                                        else
                                        {
                                            _MobileNumber = e.ContactNo;
                                        }

                                            if (Convert.ToInt32(model.Status) == 2)
                                            {
                                                _message = "Your request with THSR ID " + e.TransactionId + " is now approved and is now queued for assigning of vehicle.";
                                            }
                                            else if (Convert.ToInt32(model.Status) == 3)
                                            {
                                                _message = "Your request with THSR ID " + e.TransactionId + " has been disapproved. Please contact your supervisor for confirmation. ";
                                            }
                                            else
                                            {
                                                _message = "";
                                            }

                                    //new ReportsController().SendSMS(_message, _MobileNumber);  //JPT commented code 09042024

                                    //new Email notif EBE - ECQ - 05072020

                                    //var _Email = _context.Employees.FirstOrDefault(emp => emp.EmployeeNo.Equals(e.EmployeeNo)).CompanyEmail;
                                    //if (_Email != "")
                                    //{
                                    //    var _senderEmail = _context.Employees.FirstOrDefault(emp => emp.Id.Equals(Convert.ToInt32(HttpContext.Session.GetString("Session_employeeId")))).CompanyEmail;

                                    //    new ReportsController().SendEmail("", _Email, _senderEmail, _message, "Driver/Vehicle Reservation");
                                    //}
                                    //end new email Notif EBE - ECQ - 05072020
                                    //end

                                });




                            smsTransaction.Status = Convert.ToInt32(model.Status);
                                smsTransaction.Remarks = model.Remarks;
                                _context.Update(smsTransaction);
                                //_context.SaveChanges();
                                approveCounter++;


                            var driverPassengerHead = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId.Equals(smsTransaction.TransactionId) && s.Status.Equals(1));
                            int remainingCount = _context.DriverPassengers.Count(s => s.TransactionId.Equals(smsTransaction.TransactionId) && s.Status.Equals(1));
                            if ((driverPassengerCount <= 0 || remainingCount == checkthenumbers) && driverPassengerHead != null)
                            {
                                driverPassengerHead.Status = Convert.ToInt32(model.Status);
                            }
                            _context.Update(driverPassengerHead);
                            //_context.SaveChanges();
                        }

                            //logs
                            Log _log = new Log();
                            if (model.Status == "2") _log.Process = "Approved Reservation Trans No.: " + smsTransaction.TransactionId + " via SMS. Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;
                            else if (model.Status == "3") _log.Process = "Rejected Reservation Trans No.: " + smsTransaction.TransactionId + " via SMS. Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;
                            else _log.Process = "SMS Approval Code Invalid. Code: " + model.Status + " Reference: " + model.ReferenceNo + " . Machine Name: " + System.Environment.MachineName;
                            _log.ProcessedBy = model.MobileNo;
                            _log.ProcessedDate = DateTime.Now;
                            _context.Add(_log);
                            //end logs



                            if (approveCounter <= 0)
                            {
                                throw new System.Exception("There is no reservation to approve.");
                            }

                            //end approve or reject driver reservation    
                        }
                        else
                        {
                            throw new System.Exception("Invalid Code.");
                        }


                    }
                    else {


                        throw new System.Exception("Invalid Code.");

                    }
                    _context.SaveChanges();
                    //transaction.Commit();
                    returnMessage = "success";
                }
                catch (Exception e)
                {

                    ResetContextState();
                    returnSuccess = 0;
                    returnMessage = e.Message + "/" + e.InnerException;
                    ////if(transaction != null) { 
                    //    transaction.Rollback();
                    //}
                    //logs
                  

                    ResetContextState();
                    Log _logs2 = new Log();
                    _logs2.Process = "APPROVAL ERROR: " + System.Environment.MachineName + " -- " + returnMessage;
                    _logs2.ProcessedBy = model.MobileNo;
                    _logs2.ProcessedDate = DateTime.Now;
                    _context.Add(_logs2);
                    _context.SaveChanges();
                    //transaction.Commit();
                    //end logs

                //throw e;
                //}

            }

            return returnMessage;
        }

        // GET: api/<controller>
        [Route("checkPendingShuttles")]
        [HttpGet]
        public  IEnumerable<AllRequestViewModel> CheckPendingShuttles()
        {

            List<AllRequestViewModel> allRequestViewModel = new List<AllRequestViewModel>();
            foreach (var s in _context.ShuttlePassengers.Where(e => e.Status.Equals(1)))
            {
                allRequestViewModel.Add(new AllRequestViewModel {
                    TransactionId = s.TransactionId,
                    SmsId = s.SmsId,
                    Status = s.Status,
                    ApprovedBy = s.ApprovedBy,
                    ApprovedDatetime = s.ApprovedDatetime,
                    RequestType = "shuttle"
                });
            }

            foreach (var d in _context.DriverPassengers.Where(e => e.Status.Equals(1)))
            {
                allRequestViewModel.Add(new AllRequestViewModel
                {
                    TransactionId = d.TransactionId,
                    SmsId = d.SmsId,
                    Status = d.Status,
                    ApprovedBy = d.ApprovedBy,
                    ApprovedDatetime = d.ApprovedDatetime,
                    RequestType = "driver"
                });
            }



            return allRequestViewModel.ToList();
        }
    }
}
