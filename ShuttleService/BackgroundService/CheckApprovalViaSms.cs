using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShuttleService.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json;
using ShuttleService.Data;
using ShuttleService.Controllers;
using Microsoft.AspNetCore.Identity;

namespace ShuttleService.BackgroundService
{
    public class CheckApprovalViaSms : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning; // Flag to track if ProcessMessagesFromSimAndPhoneAsync is currently running
        private readonly IHttpClientFactory _httpClientFactory;

        //public ApproverResponseService(ISmsService smsService, ApplicationDbContext context)
        public CheckApprovalViaSms(IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _isRunning = false; // Initialize flag
            _httpClientFactory = httpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start the timer to run DoWork every 10 seconds
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            if (_isRunning) return;
            _isRunning = true;

            try
            {
                await UpdateStatus();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DoWork: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public async Task UpdateStatus() {
            string forApprovalSystemName = "THSM";
            string forApprovalReferenceNo = "";
            string forApprovalShuttleReferenceNo = "";
            //string apiUrl = "http://192.168.70.74/smsWebApi/api/SmsApi/getApprovalViaSms"; //Old API using GSM Modem
            string apiUrl = "http://aluminum/SMARTSMS_SMPC/api/SendSmsApi/getApprovalViaSms"; //Live API using SMART SMS

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                //For Driver Passenger
                // Retrieve data approval via sms from the database
                var forApprovals = await _context.DriverPassengers
                    .Where(m => m.Status == 1 && !string.IsNullOrEmpty(m.SmsId))
                    .OrderBy(m => m.Id)
                    .ToListAsync();

                // Check if there is any record with Status = 2 in the forApprovals result
                bool hasStatus2 = forApprovals.Any(m => m.Status == 2); //additional code 09232024

                // Validate if messages is not null and contains items
                if (forApprovals != null && forApprovals.Any())
                {
                    foreach (var forApproval in forApprovals)
                    {
                        forApprovalReferenceNo = forApproval.SmsId;

                        try
                        {
                            // Create an HttpClient instance using the factory
                            HttpClient httpClient = _httpClientFactory.CreateClient();

                            // Set the Accept header to "application/json"
                            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Create the JSON payload
                            var payload = new { SystemName = forApprovalSystemName, ReferenceNo = forApprovalReferenceNo };
                            string jsonPayload = JsonConvert.SerializeObject(payload);
                            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Send POST request with JSON payload
                            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                            // Ensure success status code
                            response.EnsureSuccessStatusCode();

                            // Read the response content
                            string responseData = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON response into a single ApprovalMessage object
                            var approvalMessage = JsonConvert.DeserializeObject<ApprovalMessageViewModel>(responseData);


                            // Access specific data
                            if (approvalMessage != null)
                            {
                                string[] requestorInfo = forApproval.EncodedBy.Split("/");
                                string requestorEmail = "";
                                string requestorMobileNumber = "";
                                if (requestorInfo[0].Trim().Equals("smcdacon", StringComparison.OrdinalIgnoreCase))
                                {
                                    requestorEmail = requestorInfo[1].Trim() + "@semirarampc.com";
                                }
                                else if (requestorInfo[0].Trim().Equals("semcalaca", StringComparison.OrdinalIgnoreCase))
                                {
                                    requestorEmail = requestorInfo[1].Trim() + "@semcalaca.com";
                                }

                                // Check if requestorEmail is not null, empty, or whitespace
                                if (!string.IsNullOrWhiteSpace(requestorEmail))
                                {
                                    // Retrieve requestor info from the database
                                    var requestorData = await _context.Employees
                                        .Where(m => m.CompanyEmail == requestorEmail)
                                        .FirstOrDefaultAsync();

                                    if (requestorData != null)
                                    {
                                        requestorMobileNumber = requestorData.MobileNumber;
                                    }
                                }

                                string passengerMessage = "";
                                string approverMessage = "";
                                string systemName = "THS Makati";
                                string apiResponse = approvalMessage.Response;
                                if (string.Equals(apiResponse, "1", StringComparison.OrdinalIgnoreCase))
                                {
                                    forApproval.Status = 2;
                                    forApproval.ApprovedDatetime = DateTime.Now;
                                    passengerMessage = "Your request with THSR ID " + forApproval.TransactionId + " is now approved and now queued for vehicle assignment, Thank you.";
                                    approverMessage = "Ref code: " + approvalMessage.ReferenceNo + " is now approved and now queued for vehicle assignment, Thank you.";
                                }
                                else
                                {
                                    forApproval.Status = 3;
                                    forApproval.ApprovedDatetime = DateTime.Now;
                                    passengerMessage = "Your request with THSR ID " + forApproval.TransactionId + " has been disapproved. Please contact your supervisor for confirmation, Thank you.";
                                    approverMessage = "Ref code: " + approvalMessage.ReferenceNo + " is now disapproved, Thank you.";
                                }

                                //additional code 09232024
                                if (forApproval.Status != 2)
                                {
                                    if (!hasStatus2)
                                    {
                                        var driverPassengerHeader = _context.DriverPassengerHeaders
                                            .FirstOrDefault(s => s.TransactionId.Equals(forApproval.TransactionId));

                                        if (driverPassengerHeader != null)
                                        {
                                            // Save changes to the database
                                            driverPassengerHeader.Status = forApproval.Status;
                                            _context.Update(driverPassengerHeader);
                                            await _context.SaveChangesAsync(); // Ensure async save
                                        }
                                    }
                                }
                                else
                                {
                                    // Save changes to the database
                                    var driverPassengerHeader = _context.DriverPassengerHeaders.FirstOrDefault(s => s.TransactionId.Equals(forApproval.TransactionId));
                                    driverPassengerHeader.Status = forApproval.Status;
                                    _context.Update(driverPassengerHeader);
                                    await _context.SaveChangesAsync();
                                }
                                //end additional code 09232024

                                // Save changes to the database
                                //await _context.SaveChangesAsync();

                                //Send SMS for the passenger
                                string[] passengerMobileNumber = forApproval.ContactNo.Split("/");
                                string passengerReferenceNo = forApproval.TransactionId;
                                // Call the SendSmsAsync method
                                string passenger = await new ReportsController().SendSmsAsync(passengerMobileNumber[1].Trim(), passengerMessage, passengerReferenceNo, systemName);

                                //Send SMS for the requestor
                                string requestorReferenceNo = forApproval.TransactionId;
                                // Call the SendSmsAsync method
                                string requestor = await new ReportsController().SendSmsAsync(requestorMobileNumber, passengerMessage, requestorReferenceNo, systemName);

                                //Send SMS for the approver
                                string approverMobileNumber = approvalMessage.PhoneNumber;
                                string approverReferenceNo = approvalMessage.ReferenceNo;
                                // Call the SendSmsAsync method
                                string approver = await new ReportsController().SendSmsAsync(approverMobileNumber, approverMessage, approverReferenceNo, systemName);
                            }
                        }
                        catch (HttpRequestException)
                        {
                            // Log the HTTP-specific error
                            //return BadRequest("HTTP Error calling SMS API: " + httpEx.Message);
                        }
                        catch (Exception)
                        {
                            // Handle other errors
                            //return BadRequest("Error calling SMS API: " + ex.Message);
                        }
                    }
                }

                //For Shuttle
                // Retrieve data approval via sms from the database
                var forApprovalsShuttle = await _context.ShuttlePassengers
                    .Where(m => m.Status == 1 && !string.IsNullOrEmpty(m.SmsId))
                    .OrderBy(m => m.Id)
                    .ToListAsync();

                // Validate if messages is not null and contains items
                if (forApprovalsShuttle != null && forApprovalsShuttle.Any())
                {
                    foreach (var forApproval in forApprovalsShuttle)
                    {
                        forApprovalShuttleReferenceNo = forApproval.SmsId;

                        try
                        {
                            // Create an HttpClient instance using the factory
                            HttpClient httpClient = _httpClientFactory.CreateClient();

                            // Set the Accept header to "application/json"
                            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Create the JSON payload
                            var payload = new { SystemName = forApprovalSystemName, ReferenceNo = forApprovalShuttleReferenceNo };
                            string jsonPayload = JsonConvert.SerializeObject(payload);
                            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                            // Send POST request with JSON payload
                            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                            // Ensure success status code
                            response.EnsureSuccessStatusCode();

                            // Read the response content
                            string responseData = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON response into a single ApprovalMessage object
                            var approvalMessage = JsonConvert.DeserializeObject<ApprovalMessageViewModel>(responseData);


                            // Access specific data
                            if (approvalMessage != null)
                            {
                                string[] requestorInfo = forApproval.EncodedBy.Split("/");
                                string requestorEmail = "";
                                string requestorMobileNumber = "";
                                if (requestorInfo[0].Trim().Equals("smcdacon", StringComparison.OrdinalIgnoreCase))
                                {
                                    requestorEmail = requestorInfo[1].Trim() + "@semirarampc.com";
                                }
                                else if (requestorInfo[0].Trim().Equals("semcalaca", StringComparison.OrdinalIgnoreCase))
                                {
                                    requestorEmail = requestorInfo[1].Trim() + "@semcalaca.com";
                                }

                                // Check if requestorEmail is not null, empty, or whitespace
                                if (!string.IsNullOrWhiteSpace(requestorEmail))
                                {
                                    // Retrieve requestor info from the database
                                    var requestorData = await _context.Employees
                                        .Where(m => m.CompanyEmail == requestorEmail)
                                        .FirstOrDefaultAsync();

                                    if (requestorData != null)
                                    {
                                        requestorMobileNumber = requestorData.MobileNumber;
                                    }
                                }

                                string passengerMessage = "";
                                string approverMessage = "";
                                string systemName = "THS Makati";
                                string apiResponse = approvalMessage.Response;
                                if (string.Equals(apiResponse, "1", StringComparison.OrdinalIgnoreCase))
                                {
                                    forApproval.Status = 2;
                                    forApproval.ApprovedDatetime = DateTime.Now;
                                    passengerMessage = "Your request with THSR ID " + forApproval.TransactionId + " is now approved and now queued for vehicle assignment, Thank you.";
                                    approverMessage = "Ref code: " + approvalMessage.ReferenceNo + " is now approved and now queued for vehicle assignment, Thank you.";
                                }
                                else
                                {
                                    forApproval.Status = 3;
                                    forApproval.ApprovedDatetime = DateTime.Now;
                                    passengerMessage = "Your request with THSR ID " + forApproval.TransactionId + " has been disapproved. Please contact your supervisor for confirmation, Thank you.";
                                    approverMessage = "Ref code: " + approvalMessage.ReferenceNo + " is now disapproved, Thank you.";
                                }
                                
                                // Save changes to the database
                                await _context.SaveChangesAsync();

                                //Send SMS for the passenger
                                string[] passengerMobileNumber = forApproval.ContactNo.Split("/");
                                string passengerReferenceNo = forApproval.TransactionId;
                                // Call the SendSmsAsync method
                                string passenger = await new ReportsController().SendSmsAsync(passengerMobileNumber[1].Trim(), passengerMessage, passengerReferenceNo, systemName);

                                //Send SMS for the requestor
                                string requestorReferenceNo = forApproval.TransactionId;
                                // Call the SendSmsAsync method
                                string requestor = await new ReportsController().SendSmsAsync(requestorMobileNumber, passengerMessage, requestorReferenceNo, systemName);

                                //Send SMS for the approver
                                string approverMobileNumber = approvalMessage.PhoneNumber;
                                string approverReferenceNo = approvalMessage.ReferenceNo;
                                // Call the SendSmsAsync method
                                string approver = await new ReportsController().SendSmsAsync(approverMobileNumber, approverMessage, approverReferenceNo, systemName);
                            }
                        }
                        catch (HttpRequestException)
                        {
                            // Log the HTTP-specific error
                            //return BadRequest("HTTP Error calling SMS API: " + httpEx.Message);
                        }
                        catch (Exception)
                        {
                            // Handle other errors
                            //return BadRequest("Error calling SMS API: " + ex.Message);
                        }
                    }
                }
            }
        }



    }
}
