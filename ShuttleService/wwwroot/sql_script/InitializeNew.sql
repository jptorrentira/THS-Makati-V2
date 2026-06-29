/*
 * [IMPORTANT]
 * This is for the new version of the THS with Company Group based visibility control
 * RUN this script after deployment
 * Recreated most of the View Tables and Stored Procedure
 * 
 * Author: Jasper Calixtro
 * Date: 08/14/2025
*/
USE [ShuttleReservationDB] -- Change depending on your database
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* DRIVER PASSENGER */

	-- vDriverReservationSummaryRaw_New
	CREATE VIEW [dbo].[vDriverReservationSummaryRaw_New]
	AS
	SELECT        
		TOP (100) PERCENT 
		_passenger.TransactionId, _passenger.Status AS PassengerStatusID, _header.Status AS HeaderStatusID, _status.StatusDescription AS [Passenger Status], 
		_headerstatus.StatusDescription AS [Header Status], 
		CASE 
			WHEN _passenger.ApprovedBy IS NOT NULL AND _passenger.Status <> 3 
				THEN '1' 
			ELSE '0' 
		END AS is_approved, 
		CASE
			WHEN _passenger.DeclinedBy IS NOT NULL 
				THEN '1' 
			ELSE '0' 
		END AS is_declined, 
		CASE 
			WHEN _passenger.CancelledBy IS NOT NULL 
				THEN '1' 
			ELSE '0' 
		END AS is_cancelled, 
		CASE 
			WHEN _passenger.ApprovedBy IS NOT NULL AND _passenger.Status = 2 AND _passenger.ShuttleId > 0 
				THEN '1' 
				ELSE '0' 
		END AS is_served,
		_passenger.ApprovedBy, _passenger.CancelledBy, _passenger.SubmitDate AS [Request Date], _trip.EncodeDate AS [Date Trip Assigned], CONVERT(VARCHAR(10), 
		_passenger.ServiceDate, 101) AS [Trip Date], CONVERT(VARCHAR(8), _passenger.ServiceDate, 108) AS [Trip Time], _passenger.FirstName AS [Passenger First Name], 
		_passenger.LastName AS [Passenger Last Name], _passenger.Destination, 
		CASE 
			WHEN _passenger.ShuttleId > 0 AND _trip.Status = 1 AND _status.StatusDescription = 'Approved' 
				THEN 'Served' 
			ELSE '' 
		END AS Expr1, 
		_dept.DepartmentName AS ChargingDepartments, _company.CompanyName AS ChargingCompanys, _requestor.CompanyGroupId,
		CASE 
			WHEN _trip.Status = 1 
				THEN 'Assigned' 
			WHEN _trip.Status = 0 
				THEN 'Cancelled' 
			ELSE 'No Trip' 
		END AS [Trip Status], _passenger.ShuttleId AS [Trip ID], _trip.TripControlNo AS [Trip Control No], _passenger.Id, _header.DestinationTag, _header.OriginTag,
		_requestor.FirstName AS RequestedByFirstname, _requestor.LastName AS RequestedByLastname, _passenger.Origin, _header.TripTimeFrom, _header.TripTimeTo
	FROM            
		dbo.DriverPassengers AS _passenger LEFT OUTER JOIN
		dbo.DriverPassengerHeaders AS _header ON _passenger.DriverPassengerHeaderId = _header.Id LEFT OUTER JOIN
		dbo.Trip AS _trip ON _passenger.ShuttleId = _trip.Id LEFT OUTER JOIN
		dbo.ShuttlePassengerStatuses AS _status ON _passenger.Status = _status.Id LEFT OUTER JOIN
		dbo.ShuttlePassengerStatuses AS _headerstatus ON _header.Status = _headerstatus.Id LEFT OUTER JOIN
		dbo.ChargingDepartments AS _dept ON _passenger.ChargingDepartmentId = _dept.Id LEFT OUTER JOIN
		dbo.ChargingCompanys AS _company ON _passenger.ChargingCompanyId = _company.Id INNER JOIN
		dbo.Employees AS _requestor ON _passenger.EmployeeId = _requestor.Id
	WHERE
		(_status.StatusDescription NOT IN ('Removed', 'Open'))
	
	ORDER BY 
		_passenger.TransactionId
	GO

	-- vDriverREservationSummary_New
	CREATE VIEW [dbo].[vDriverReservationSummary_New]
	AS
	SELECT
		*
	FROM
		(SELECT
			raw.*, ROW_NUMBER() OVER (PARTITION BY raw.TransactionId
		ORDER BY 
			ID) 
		AS RowNumber
	FROM
		vDriverReservationSummaryRaw_New as raw
		WHERE
			raw.is_approved = 1 AND raw.HeaderStatusid > 1) AS a
	WHERE
		a.RowNumber = 1
	GO

	-- vTripCounts_Approved_New
	CREATE VIEW [dbo].[vTripCounts_Approved_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(ISNULL(Id, 0)) AS TripCount,
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	
	FROM            
		dbo.vDriverReservationSummary_New
	WHERE        
		is_approved = 1
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Cancelled_New
	CREATE VIEW [dbo].[vTripCounts_Cancelled_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(ISNULL(Id, 0)) AS TripCount,
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vDriverReservationSummary_New
	WHERE        
		(is_cancelled = 1)
	GROUP BY CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Declined_New
	CREATE VIEW [dbo].[vTripCounts_Declined_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(ISNULL(Id, 0)) AS TripCount,
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM           
		dbo.vDriverReservationSummary_New
	WHERE        
		(is_declined = 1)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Pending_New
	CREATE VIEW [dbo].[vTripCounts_Pending_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate,
		COUNT(ISNULL(Id, 0)) AS TripCount,
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vDriverReservationSummary_New
	WHERE        
		(is_served = 0) AND (is_approved = 1) AND (is_cancelled = 0) AND (is_declined = 0)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Served_New
	CREATE VIEW [dbo].[vTripCounts_Served_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(ISNULL(Id, 0)) AS TripCount,
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vDriverReservationSummary_New
	WHERE        
		(is_served = 1)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO



/* SHUTTLE PASSENGER */

	-- vShuttleReservationSummaryRaw_New
	CREATE VIEW [dbo].[vShuttleReservationSummaryRaw_New]
	AS
	SELECT        
		TOP (100) PERCENT _passenger.TransactionId, _passenger.Status AS PassengerStatusID, _status.StatusDescription AS [Passenger Status], 
		CASE 
			WHEN _passenger.ApprovedBy IS NOT NULL AND _passenger.Status <> 3 
				THEN '1' 
			ELSE '0' 
		END AS is_approved, 
		CASE 
			WHEN _passenger.DeclinedBy IS NOT NULL 
				THEN '1' 
			ELSE '0' 
		END AS is_declined, 
		CASE 
			WHEN _passenger.CancelledBy IS NOT NULL 
				THEN '1' 
			ELSE '0' 
		END AS is_cancelled, 
		CASE 
			WHEN _passenger.ApprovedBy IS NOT NULL AND _passenger.Status = 2 AND _passenger.ShuttleId > 0 
				THEN '1' 
			ELSE '0' 
		END AS is_served, 
		_passenger.ApprovedBy, _passenger.CancelledBy, _passenger.SubmitDate AS [Request Date], _trip.EncodeDate AS [Date Trip Assigned], 
		CONVERT(VARCHAR(10), _passenger.ServiceDate, 101) AS [Trip Date], CONVERT(VARCHAR(8), _passenger.ServiceDate, 108) AS [Trip Time],
		_passenger.FirstName AS [Passenger First Name], _passenger.LastName AS [Passenger Last Name], _triptype.Description,
		CASE 
			WHEN _passenger.ShuttleId > 0 AND _trip.Status = 1 AND _status.StatusDescription = 'Approved' 
				THEN 'Served' 
			ELSE '' 
		END AS Expr1, 
		_dept.DepartmentName AS ChargingDepartments, _company.CompanyName AS ChargingCompanys, 
			(SELECT 
				CompanyGroupId
			FROM 
				dbo.Employees as employee
			WHERE
				_passenger.EmployeeNo = employee.EmployeeNo
			) as CompanyGroupId,
		CASE 
			WHEN _trip.Status = 1 
				THEN 'Assigned' 
			WHEN _trip.Status = 0 
				THEN 'Cancelled' 
			ELSE 'No Trip' 
		END AS [Trip Status],
		_passenger.ShuttleId AS [Trip ID], _trip.TripControlNo AS [Trip Control No], _passenger.Id
	FROM 
		dbo.ShuttlePassengers AS _passenger LEFT OUTER JOIN
		dbo.Trip AS _trip ON _passenger.ShuttleId = _trip.Id LEFT OUTER JOIN
		dbo.ShuttlePassengerStatuses AS _status ON _passenger.Status = _status.Id LEFT OUTER JOIN
		dbo.ChargingDepartments AS _dept ON _passenger.ChargingDepartmentId = _dept.Id LEFT OUTER JOIN
		dbo.ChargingCompanys AS _company ON _passenger.ChargingCompanyId = _company.Id LEFT OUTER JOIN
		dbo.ReservationType AS _triptype ON _passenger.TripTypeId = _triptype.Id
	WHERE        
		_status.StatusDescription NOT IN ('Removed', 'Open')
	ORDER BY 
		_passenger.TransactionId
	GO

	-- vShuttleReservationSummary_New
	CREATE VIEW [dbo].[vShuttleReservationSummary_New]
	AS
	SELECT        
		*
	FROM            
		(SELECT        
			raw.*, 
			ROW_NUMBER() OVER (PARTITION BY raw.TransactionId
		ORDER BY 
			ID) 
		AS RowNumber
	FROM            
		vShuttleReservationSummaryRaw_New as raw
		WHERE        
			raw.is_approved = 1) AS a
	WHERE        
		a.RowNumber = 1
	GO

	-- vTripCounts_Approved_Shuttle_New
	CREATE VIEW [dbo].[vTripCounts_Approved_Shuttle_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(Id) AS TripCount, 
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New
	WHERE       
		is_approved = 1
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Cancelled_Shuttle_New
	CREATE VIEW [dbo].[vTripCounts_Cancelled_Shuttle_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(Id) AS TripCount, 
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New
	WHERE        
		(is_cancelled = 1)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Declined_Shuttle_New
	CREATE VIEW [dbo].[vTripCounts_Declined_Shuttle_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(Id) AS TripCount, 
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New
	WHERE        
		(is_declined = 1)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Pending_Shuttle_New
	CREATE VIEW [dbo].[vTripCounts_Pending_Shuttle_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(Id) AS TripCount, 
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New
	WHERE        
		(is_served = 0) AND (is_approved = 1) AND (is_cancelled = 0) AND (is_declined = 0)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO

	-- vTripCounts_Served_Shuttle_New
	CREATE VIEW [dbo].[vTripCounts_Served_Shuttle_New]
	AS
	SELECT        
		CONVERT(date, [Trip Date]) AS TripDate, 
		COUNT(Id) AS TripCount, 
		DAY(CONVERT(date, [Trip Date])) AS daynum,
		CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New
	WHERE        
		(is_served = 1)
	GROUP BY 
		CONVERT(date, [Trip Date]),
		CompanyGroupId
	GO


/* MISC */

	-- vTripPerVehicles_New
	CREATE VIEW [dbo].[vTripPerVehicles_New]
	AS
	SELECT        
		TOP (100) PERCENT 
		CONVERT(date, trip.ServiceStartDate) AS TripDate, 
		COUNT(trip.Id) AS TripCount, 
		trip.VehicleListId, 
		ISNULL(vehicle.Model, 'Unknown Model') AS Model,
		summary.CompanyGroupId
	FROM            
		dbo.vDriverReservationSummary_New as summary
		INNER JOIN dbo.Trip as trip 
			ON summary.[Trip ID] = trip.Id 
		INNER JOIN dbo.VehicleLists as vehicle 
			ON trip.VehicleListId = vehicle.Id
	WHERE        
		(trip.Status = 1)
	GROUP BY 
		CONVERT(date,trip.ServiceStartDate), 
		trip.VehicleListId, 
		vehicle.Model,
		summary.CompanyGroupId
	ORDER BY 
		TripDate
	GO

	-- vTripPerVehicles_Shuttle_New
	CREATE VIEW [dbo].[vTripPerVehicles_Shuttle_New]
	AS
	SELECT        
		TOP (100) PERCENT
		CONVERT(date, trip.ServiceStartDate) AS TripDate,
		COUNT(trip.Id) AS TripCount,
		trip.VehicleListId,
		vehicle.Model,
		summary.CompanyGroupId
	FROM            
		dbo.vShuttleReservationSummary_New as summary
		INNER JOIN dbo.Trip as trip
			ON summary.[Trip ID] = trip.Id 
		INNER JOIN dbo.VehicleLists as vehicle
			ON trip.VehicleListId = vehicle.Id
	WHERE        
		(trip.Status = 1)
	GROUP BY 
		CONVERT(date, trip.ServiceStartDate), 
		trip.VehicleListId,
		vehicle.Model,
		summary.CompanyGroupId
	ORDER BY 
		TripDate
	GO

	-- vTripPerVehicleUnion_New
	CREATE VIEW [dbo].[vTripPerVehicleUnion_New]
	AS
	SELECT        
		TOP (100) PERCENT 
		CONVERT(date, trip.ServiceStartDate) AS TripDate,
		COUNT(trip.Id) AS TripCount,
		trip.VehicleListId,
		vehicle.Model,
		req.CompanyGroupId
	FROM            
		dbo.vUnionRequestTrips_New as req
		INNER JOIN dbo.Trip as trip
			ON req.[Trip ID] = trip.Id 
		INNER JOIN dbo.VehicleLists as vehicle
			ON trip.VehicleListId = vehicle.Id
	WHERE        
		(trip.Status = 1)
	GROUP BY 
		CONVERT(date, trip.ServiceStartDate), 
		trip.VehicleListId, 
		vehicle.Model,
		req.CompanyGroupId
	ORDER BY 
		TripDate
	GO

	-- vUnionRequestTrips_New
	CREATE VIEW [dbo].[vUnionRequestTrips_New]
	AS
	select 
		transactionid,
		[Trip ID],
		CompanyGroupId
	from 
		dbo.vShuttleReservationSummary_New
	union all
	select 
		transactionid,
		[Trip ID],
		CompanyGroupId 
	from 
		dbo.vDriverReservationSummary_New
	GO

	-- vReservationServedUnion_New
	CREATE VIEW [dbo].[vReservationServedUnion_New]
	AS
	SELECT       
		TransactionId, 
		is_approved,
		is_declined, 
		is_cancelled,
		is_served, 
		ChargingDepartments,
		ChargingCompanys,
		CompanyGroupId,
		[Trip ID],
		[Trip Control No],
		CONVERT(date, [Trip Date]) AS TripDate,
		RowNumber
	FROM            
		vDriverReservationSummary_New
	UNION ALL
	SELECT        
		TransactionId,
		is_approved,
		is_declined,
		is_cancelled,
		is_served,
		ChargingDepartments,
		ChargingCompanys,
		CompanyGroupId,
		[Trip ID],
		[Trip Control No],
		CONVERT(date, [Trip Date]) AS TripDate,
		RowNumber
	FROM            
		vShuttleReservationSummary_New
	GO

	-- vOutsideMMTrips_New
	CREATE VIEW [dbo].[vOutsideMMTrips_New]
	AS
	SELECT        
		[Trip Date] AS TripDate,
		{ fn CONCAT({ fn CONCAT(TripTimeFrom, ' - ') }, TripTimeTo) } AS TripTime,
		Origin, Destination, ChargingCompanys, ChargingDepartments,
		RequestedByFirstname, RequestedByLastname, TripTimeFrom, TripTimeTo,
		CompanyGroupId
	FROM            
		dbo.vDriverReservationSummary_New
	WHERE        
		(is_served = 1) 
		AND (DestinationTag LIKE '%Outside%') 
		OR (is_served = 1) 
		AND (OriginTag LIKE '%Outside%')
	GO

	-- vSurveyResults_New
	CREATE VIEW [dbo].[vSurveyResults_New]
	AS
	SELECT        
		st.TransactionId, t.ServiceStartDate, e.FirstName AS RequestorFirstName, e.LastName AS RequestorLastName,
		d.FirstName AS DriverFirstName, d.LastName AS DriverLastName, sa.AnswerScore, sa.Remarks, t.DriverId, 
		CASE 
			WHEN AnswerScore = 4 
				THEN 4 
			ELSE 0 
		END AS AnswerScore4, 
		CASE 
			WHEN AnswerScore = 3 
				THEN 3 
			ELSE 0 
		END AS AnswerScore3, 
		CASE 
			WHEN AnswerScore = 2 
				THEN 2 
			ELSE 0 
		END AS AnswerScore2, 
		CASE 
			WHEN AnswerScore = 1 
				THEN 1 
			ELSE 0 
		END AS AnswerScore1, 
		DATEPART(QUARTER, t.ServiceStartDate) AS QuarterNo, DATEPART(YEAR, t.ServiceStartDate) AS YearNo,
		e.CompanyGroupId
	FROM        
		dbo.SurveyTransactions AS st 
		INNER JOIN dbo.Employees AS e ON st.EmployeeId = e.Id 
		INNER JOIN dbo.DriverPassengerHeaders AS dp ON st.TransactionId = dp.TransactionId 
		INNER JOIN dbo.Trip AS t ON t.Id = dp.ShuttleId 
		INNER JOIN dbo.Drivers AS d ON d.Id = t.DriverId 
		INNER JOIN dbo.SurveyAnswers AS sa ON sa.SurveyTransactionId = st.Id
	WHERE        
		(sa.Status = 1) 
		AND (st.Status = 1) 
		AND (sa.SurveyQuestionId = 1)
	GO


/* STORED PROCEDURES */

	-- GetDriverCounts_New
	CREATE PROCEDURE [dbo].[GetDriverCounts_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int

	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;


		WITH theDates AS
			(SELECT 
				@StartDate as theDate
				UNION ALL
				SELECT 
					DATEADD(day, 1, theDate)
				FROM theDates
				WHERE 
					DATEADD(day, 1, theDate) <= @EndDate
			)

		SELECT 
			CONVERT(date, theDate) AS TripDate, 
			FORMAT(theDate, 'dd-MMM-yyyy') as TripDateFormat,		
			(SELECT app.TripCount WHERE app.CompanyGroupId = @CompanyGroupId)
				as 'TripApprovedCount', 
			(SELECT dec.TripCount WHERE dec.CompanyGroupId = @CompanyGroupId)
				as 'TripDeclinedCount',  
			(SELECT can.TripCount WHERE can.CompanyGroupId = @CompanyGroupId)
				as 'TripCancelledCount', 
			(SELECT ser.TripCount WHERE ser.CompanyGroupId = @CompanyGroupId)
				as 'TripServedCount', 
			(SELECT pen.TripCount WHERE pen.CompanyGroupId = @CompanyGroupId)
				as 'TripPendingCount'
		FROM theDates 
			left join vTripCounts_Approved_New as app
				on CONVERT(date, theDate) = CONVERT(date, app.TripDate)
			left join vTripCounts_Declined_New as dec
				on CONVERT(date, theDate) = CONVERT(date, dec.TripDate)
			left join vTripCounts_Cancelled_New as can
				on CONVERT(date, theDate) = CONVERT(date, can.TripDate)
			left join vTripCounts_Served_New as ser
				on CONVERT(date, theDate) = CONVERT(date, ser.TripDate)
			left join vTripCounts_Pending_New as pen
				on CONVERT(date, theDate) = CONVERT(date, pen.TripDate)
		OPTION (MAXRECURSION 0);
	END
	GO
	
	-- GetShuttleCounts_New
	CREATE PROCEDURE [dbo].[GetShuttleCounts_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
	
		WITH theDates AS
			 (SELECT @StartDate as theDate
			  UNION ALL
			  SELECT DATEADD(day, 1, theDate)
				FROM theDates
			   WHERE DATEADD(day, 1, theDate) <= @EndDate
			 )

		SELECT 
			CONVERT(date, theDate) AS TripDate, 
			FORMAT(theDate, 'dd-MMM-yyyy') as TripDateFormat,
			(SELECT app.TripCount WHERE app.CompanyGroupId = @CompanyGroupId)
				as 'TripApprovedCount', 
			(SELECT dec.TripCount WHERE dec.CompanyGroupId = @CompanyGroupId)
				as 'TripDeclinedCount', 
			(SELECT can.TripCount WHERE can.CompanyGroupId = @CompanyGroupId)
				as 'TripCancelledCount', 
			(SELECT ser.TripCount WHERE ser.CompanyGroupId = @CompanyGroupId)
				as 'TripServedCount', 
			(SELECT pen.TripCount WHERE pen.CompanyGroupId = @CompanyGroupId)
				as 'TripPendingCount'
		FROM theDates 
			left join vTripCounts_Approved_Shuttle_New as app
				on CONVERT(date, theDate) = CONVERT(date, app.TripDate)
			left join vTripCounts_Declined_Shuttle_New as dec
				on CONVERT(date, theDate) = CONVERT(date, dec.TripDate)
			left join vTripCounts_Cancelled_Shuttle_New as can
				on CONVERT(date, theDate) = CONVERT(date, can.TripDate)
			left join vTripCounts_Served_Shuttle_New as ser
				on CONVERT(date, theDate) = CONVERT(date, ser.TripDate)
			left join vTripCounts_Pending_Shuttle_New as pen
				on CONVERT(date, theDate) = CONVERT(date, pen.TripDate)
		OPTION (MAXRECURSION 0);
	END
	GO

	-- GetTripVehicleCounts_DriverVehicle_New
	CREATE PROCEDURE [dbo].[GetTripVehicleCounts_DriverVehicle_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		-- Insert statements for procedure here
		SELECT 
			VehicleListId,
			sum(TripCount) as TripCountSum,
			Model
		FROM 
			vTripPerVehicles_New
		WHERE 
			TripDate >= @StartDate 
			AND TripDate <= @EndDate
			AND CompanyGroupId = @CompanyGroupId
		GROUP BY 
			VehicleListId,
			Model
		ORDER BY 
			Model;
	END
	GO

	-- GetTripVehicleCounts_Shuttle_New
	CREATE PROCEDURE  [dbo].[GetTripVehicleCounts_Shuttle_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int 
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		-- Insert statements for procedure here
		SELECT 
			VehicleListId,
			sum(TripCount) as TripCountSum,
			Model 
		FROM 
			vTripPerVehicles_Shuttle_New
		WHERE 
			TripDate >= @StartDate 
			AND TripDate <= @EndDate
			AND CompanyGroupId = @CompanyGroupId
		GROUP BY 
			VehicleListId,
			Model
		ORDER BY 
			Model;
	END
	GO

	-- GetTripVehicleCounts_Union_New
	CREATE PROCEDURE [dbo].[GetTripVehicleCounts_Union_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int 
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		select 
			VehicleListId,
			sum(TripCount) as TripCountSum,
			Model
		from 
			vTripPerVehicleUnion_New
		where 
			TripDate >= @StartDate 
			AND TripDate <= @EndDate
			AND CompanyGroupId = @CompanyGroupId
		group by 
			VehicleListId,
			Model
		order by 
			Model;
	END
	GO

	-- GetTripChargingCompanies_Union_New
	CREATE PROCEDURE [dbo].[GetTripChargingCompanies_Union_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
		
		select 
			ChargingCompanys,
			sum(RowNumber) as TripCountSum 
		from 
			vReservationServedUnion_New
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate
			AND CompanyGroupId = @CompanyGroupId
		group by 
			ChargingCompanys
		order by 
			ChargingCompanys;
	END
	GO

	-- GetTripChargingDepartmentByCompanies_Union_Ranked1_New
	CREATE PROCEDURE  [dbo].[GetTripChargingDepartmentByCompanies_Union_Ranked1_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
	
		IF OBJECT_ID(N'tempdb..#testtable') IS NOT NULL
			BEGIN
				DROP TABLE #testtable;
			END

		SELECT        
			*, 
			DENSE_RANK() OVER (ORDER BY ChargingCompanys) AS Row
		INTO 
			#testtable
		FROM            
			vReservationServedUnion_New
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate;
		select 
			ChargingCompanys,
			ChargingDepartments,
			sum(RowNumber) as TripCountSum,
			[Row] 
		from 
			#testtable
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate
			and CompanyGroupId = @CompanyGroupId
			and [Row] in (1,4,7,10,13,16,19,22,25,28,31,34,37,40,43,46,49,52,55,58,61)
		group by 
			ChargingCompanys,
			ChargingDepartments,
			[Row]
		order by 
			ChargingCompanys,
			ChargingDepartments,
			[Row];
	END
	GO

	-- GetTripChargingDepartmentByCompanies_Union_Ranked2_New
	CREATE PROCEDURE  [dbo].[GetTripChargingDepartmentByCompanies_Union_Ranked2_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
	
		IF OBJECT_ID(N'tempdb..#testtable') IS NOT NULL
			BEGIN
				DROP TABLE #testtable;
			END

		SELECT        
			*, 
			DENSE_RANK() OVER (ORDER BY ChargingCompanys) AS Row
		INTO 
			#testtable
		FROM            
			vReservationServedUnion_New
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate;
		select 
			ChargingCompanys,
			ChargingDepartments,
			sum(RowNumber) as TripCountSum,
			[Row] 
		from 
			#testtable
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate
			and CompanyGroupId = @CompanyGroupId
			and [Row] in (2,5,8,11,14,17,20,23,26,29,32,35,38,41,44,47,50,53,56,59,62)
		group by 
			ChargingCompanys,
			ChargingDepartments,
			[Row]
		order by 
			ChargingCompanys,
			ChargingDepartments,
			[Row];
	END
	GO

	-- GetTripChargingDepartmentByCompanies_Union_Ranked3_New
	CREATE PROCEDURE  [dbo].[GetTripChargingDepartmentByCompanies_Union_Ranked3_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
	
		IF OBJECT_ID(N'tempdb..#testtable') IS NOT NULL
			BEGIN
				DROP TABLE #testtable;
			END

		SELECT        
			*, 
			DENSE_RANK() OVER (ORDER BY ChargingCompanys) AS Row
		INTO 
			#testtable
		FROM            
			vReservationServedUnion_New
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate;
		select 
			ChargingCompanys,
			ChargingDepartments,
			sum(RowNumber) as TripCountSum,
			[Row] 
		from 
			#testtable
		where 
			TripDate >= @StartDate 
			and TripDate <= @EndDate
			and CompanyGroupId = @CompanyGroupId
			and [Row] in (3,6,9,12,15,18,21,24,27,30,33,36,39,42,45,48,51,54,57,60,63)
		group by 
			ChargingCompanys,
			ChargingDepartments,
			[Row]
		order by 
			ChargingCompanys,
			ChargingDepartments,
			[Row];
	END
	GO

	-- GetDriverCounts_DriverVehicle_Monthly_New
	CREATE PROCEDURE  [dbo].[GetDriverCounts_DriverVehicle_Monthly_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT 
			YEAR(app.TripDate) AS year_,
			MONTH(app.TripDate) AS month_,
			DATENAME(month, app.TripDate) AS MonthName,
    
			SUM(CASE WHEN app.CompanyGroupId = @CompanyGroupId THEN app.TripCount ELSE NULL END) AS TripCountApproved,
			SUM(CASE WHEN can.CompanyGroupId = @CompanyGroupId THEN can.TripCount ELSE NULL END) AS TripCountCancelled,
			SUM(CASE WHEN dec.CompanyGroupId = @CompanyGroupId THEN dec.TripCount ELSE NULL END) AS TripCountDeclined,
			SUM(CASE WHEN ser.CompanyGroupId = @CompanyGroupId THEN ser.TripCount ELSE NULL END) AS TripCountServed,
			SUM(CASE WHEN pen.CompanyGroupId = @CompanyGroupId THEN pen.TripCount ELSE NULL END) AS TripCountPending
		FROM 
			vTripCounts_Approved_New app
			LEFT JOIN vTripCounts_Cancelled_New can 
				ON app.TripDate = can.TripDate
			LEFT JOIN vTripCounts_Declined_New dec 
				ON app.TripDate = dec.TripDate
			LEFT JOIN vTripCounts_Served_New ser 
				ON app.TripDate = ser.TripDate
			LEFT JOIN vTripCounts_Pending_New pen 
				ON app.TripDate = pen.TripDate
		WHERE 
			app.TripDate >= @StartDate 
			AND app.TripDate <= @EndDate

		GROUP BY 
			YEAR(app.TripDate),
			MONTH(app.TripDate),
			DATENAME(month, app.TripDate)

		ORDER BY 
			year_,
			month_;

	END
	GO

	-- GetShuttleCounts_Monthly_New
	CREATE PROCEDURE [dbo].[GetShuttleCounts_Monthly_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;
		SELECT 
			YEAR(app.TripDate) AS year_,
			MONTH(app.TripDate) AS month_,
			DATENAME(month, app.TripDate) AS MonthName,
    
			SUM(CASE WHEN app.CompanyGroupId = @CompanyGroupId THEN app.TripCount ELSE NULL END) AS TripCountApproved,
			SUM(CASE WHEN can.CompanyGroupId = @CompanyGroupId THEN can.TripCount ELSE NULL END) AS TripCountCancelled,
			SUM(CASE WHEN dec.CompanyGroupId = @CompanyGroupId THEN dec.TripCount ELSE NULL END) AS TripCountDeclined,
			SUM(CASE WHEN ser.CompanyGroupId = @CompanyGroupId THEN ser.TripCount ELSE NULL END) AS TripCountServed,
			SUM(CASE WHEN pen.CompanyGroupId = @CompanyGroupId THEN pen.TripCount ELSE NULL END) AS TripCountPending
		FROM 
		vTripCounts_Approved_Shuttle_New app 
		LEFT JOIN vTripCounts_Cancelled_Shuttle_New can 
			ON app.TripDate  = can.TripDate 
		LEFT JOIN vTripCounts_Declined_Shuttle_New dec 
			ON app.TripDate  = dec.TripDate
		LEFT JOIN vTripCounts_Served_Shuttle_New ser 
			ON app.TripDate  = ser.TripDate
		LEFT JOIN vTripCounts_Pending_Shuttle_New pen 
			ON app.TripDate  = pen.TripDate
		WHERE 
			app.TripDate >= @StartDate 
			AND app.TripDate <= @EndDate
		GROUP BY 
			YEAR(app.TripDate),
			MONTH(app.TripDate),
			DATENAME(month,app.TripDate)
		ORDER BY 
			year_,
			month_
	END
	GO
	
	-- GetCounts_Monthly_New
	CREATE PROCEDURE [dbo].[GetCounts_Monthly_New]
		@StartDate datetime,
		@EndDate datetime,
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT 
			YEAR(app.TripDate) AS year_,
			MONTH(app.TripDate) AS month_,
			DATENAME(month, app.TripDate) AS MonthName,
    
			SUM(CASE WHEN app.CompanyGroupId = @CompanyGroupId THEN app.TripCount ELSE NULL END) AS TripCountApproved,
			SUM(CASE WHEN can.CompanyGroupId = @CompanyGroupId THEN can.TripCount ELSE NULL END) AS TripCountCancelled,
			SUM(CASE WHEN dec.CompanyGroupId = @CompanyGroupId THEN dec.TripCount ELSE NULL END) AS TripCountDeclined,
			SUM(CASE WHEN ser.CompanyGroupId = @CompanyGroupId THEN ser.TripCount ELSE NULL END) AS TripCountServed,
			SUM(CASE WHEN pen.CompanyGroupId = @CompanyGroupId THEN pen.TripCount ELSE NULL END) AS TripCountPending,
			'Driver' as 'RequestType'
		from 
			vTripCounts_Approved_New app 
			LEFT JOIN vTripCounts_Cancelled_New can 
				on app.TripDate  = can.TripDate 
			LEFT JOIN vTripCounts_Declined_New dec 
				on app.TripDate  = dec.TripDate
			LEFT JOIN vTripCounts_Served_New ser 
				on app.TripDate  = ser.TripDate
			LEFT JOIN vTripCounts_Pending_New pen 
				on app.TripDate  = pen.TripDate
		where 
			app.TripDate >= @StartDate 
			AND app.TripDate <= @EndDate
		group by 
			YEAR(app.TripDate),
			MONTH(app.TripDate),
			DATENAME(month,app.TripDate)

		UNION ALL

		select 
			YEAR(app.TripDate) AS year_,
			MONTH(app.TripDate) AS month_,
			DATENAME(month, app.TripDate) AS MonthName,
    
			SUM(CASE WHEN app.CompanyGroupId = @CompanyGroupId THEN app.TripCount ELSE NULL END) AS TripCountApproved,
			SUM(CASE WHEN can.CompanyGroupId = @CompanyGroupId THEN can.TripCount ELSE NULL END) AS TripCountCancelled,
			SUM(CASE WHEN dec.CompanyGroupId = @CompanyGroupId THEN dec.TripCount ELSE NULL END) AS TripCountDeclined,
			SUM(CASE WHEN ser.CompanyGroupId = @CompanyGroupId THEN ser.TripCount ELSE NULL END) AS TripCountServed,
			SUM(CASE WHEN pen.CompanyGroupId = @CompanyGroupId THEN pen.TripCount ELSE NULL END) AS TripCountPending,
			'Shuttle' as 'RequestType'
		from 
			vTripCounts_Approved_Shuttle_New app 
			left join vTripCounts_Cancelled_Shuttle_New can 
				on app.TripDate  = can.TripDate 
			left join vTripCounts_Declined_Shuttle_New dec 
				on app.TripDate  = dec.TripDate
			left join vTripCounts_Served_Shuttle_New ser 
				on app.TripDate  = ser.TripDate
			left join vTripCounts_Pending_Shuttle_New pen 
				on app.TripDate  = pen.TripDate
		where 
			app.TripDate >= @StartDate 
			AND app.TripDate <= @EndDate
		group by 
			year(app.TripDate),
			month(app.TripDate),
			DATENAME(month, app.TripDate)
		order by 
			year_,
			month_;
	END
	GO

	-- GetSurveyResultsPerDriver_New
	CREATE PROCEDURE [dbo].[GetSurveyResultsPerDriver_New]
		@Quarter varchar(20),
		@Year varchar(20),
		@CompanyGroupId int
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		select 
			DriverId,DriverFirstName,DriverLastName,
			Sum(CASE WHEN AnswerScore = 1 then 1 else 0 END) as SurveyScoreTotal1,
			Sum(CASE WHEN AnswerScore = 2 then 2 else 0 END) as SurveyScoreTotal2,
			Sum(CASE WHEN AnswerScore = 3 then 3 else 0 END) as SurveyScoreTotal3,
			Sum(CASE WHEN AnswerScore = 4 then 4 else 0 END) as SurveyScoreTotal4,
			(Sum(AnswerScore)/COUNT(AnswerScore)) as SurveyScoreTotalRating 
		from 
			vSurveyResults_New
		where 
			QuarterNo >= @Quarter 
			and YearNo <= @Year
			and CompanyGroupId = @CompanyGroupId
		group by 
			DriverId,
			DriverFirstName,
			DriverLastName;
	END
	GO