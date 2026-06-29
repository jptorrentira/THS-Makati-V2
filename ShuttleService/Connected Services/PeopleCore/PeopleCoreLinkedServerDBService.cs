using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ShuttleService.Models;

public class PeopleCoreLinkedServerDBService
{
    private readonly string _connectionString;
    public PeopleCoreLinkedServerDBService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PeopleCoreLinkedConnection");
    }

    public async Task<PeopleCoreEmployeeInfo> GetUserInfoByEmployeeNo(string employeeNo)
    {
        try
        {
            var employeeInfo = new PeopleCoreEmployeeInfo();
            var query =
                $@"
                    SELECT
                        FirstName,
                        MiddleName,
                        LastName,
	                    Email,
	                    MobileNo,
	                    PositionDesc
                    FROM [192.168.70.49].[PCV5_DMCI].[HRDEV].[EmployeeDetails]
                    WHERE EmployeeNumber = '{employeeNo}'   
                ";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employeeInfo = new PeopleCoreEmployeeInfo
                        {
                            FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                            MiddleName = reader["MiddleName"]?.ToString() ?? string.Empty,
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString() ?? string.Empty,
                            MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                            Position = reader["PositionDesc"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }
            return employeeInfo;
        }
        catch(Exception e)
        {
            var error = e.Message;
            return null;
        }       
    }
    public async Task<PeopleCoreEmployeeInfo> GetUserInfoByEmployeeName(string employeeName)
    {
        try
        {
            var employeeInfo = new PeopleCoreEmployeeInfo();
            var query =
                $@"
                    SELECT
                        FirstName,
                        MiddleName,
                        LastName,
	                    Email,
	                    MobileNo,
	                    PositionDesc,
                        DepartmentCode
                    FROM [192.168.70.49].[PCV5_DMCI].[HRDEV].[EmployeeDetails]
                    WHERE FullName = '{employeeName}'
                ";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employeeInfo = new PeopleCoreEmployeeInfo
                        {
                            FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                            MiddleName = reader["MiddleName"]?.ToString() ?? string.Empty,
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString() ?? string.Empty,
                            MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                            Position = reader["PositionDesc"]?.ToString() ?? string.Empty,
                            DepartmentCode = reader["DepartmentCode"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }
            return employeeInfo;
        }
        catch (Exception e)
        {
            var error = e.Message;
            return null;
        }
    }
    public async Task<List<string>> GetUserInfoByEmployeeNoOrName(string employee)
    {
        try
        {
            var query =
                $@"
                    SELECT
                        Fullname
                    FROM [192.168.70.49].[PCV5_DMCI].[HRDEV].[EmployeeDetails]
                    WHERE EmployeeNumber like '%{employee}%'
                        OR Fullname like '%{employee}%'
                ";
            var employees = new List<string>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(reader["Fullname"]?.ToString() ?? string.Empty);
                    }
                }
            }
            return employees;
        }
        catch (Exception e)
        {
            var error = e.Message;
            return null;
        }
    }
}
