﻿using AutoMapper;
using CarRental.API.DTOs;
using CarRental.API.Models;
using CarRental.API.Repositories.Interfaces;
using CarRental.API.Validation;
using ClosedXML.Excel;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private IReservationRepository _reservationRepository;
        private ICustomerRepository _customerRepository;
        private IVehicleRepository _vehicleRepository;
        private IMapper _mapper;
        private IValidator<ReservationDTO> _reservationValidator;

        public ReservationController(IReservationRepository reservationRepo, ICustomerRepository customerRepo,
            IVehicleRepository vehicleRepo, IMapper mapper, IValidator<ReservationDTO> reservationValidator)
        {
            _reservationRepository = reservationRepo;
            _customerRepository = customerRepo;
            _vehicleRepository = vehicleRepo;
            _mapper = mapper;
            _reservationValidator = reservationValidator;
        }

        [HttpGet("AllReservations")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> GetAllReservationsAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync();
            return Ok(reservations);
        }

        [HttpGet("ExportToExcel")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> ExportToExcelAsync()
        {
            var reservations = await _reservationRepository.GetAllReservationsAsync(); 
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reservations");

            var headers = new[] { "ReservationID", "Description", "DateFrom", "DateTo", "CustomerID", "VehicleID" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Fill.BackgroundColor = XLColor.Glaucous;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
            }

            int rowIndex = 2;
            foreach (var reservation in reservations)
            {
                worksheet.Cell(rowIndex, 1).Value = reservation.ReservationID;
                worksheet.Cell(rowIndex, 2).Value = reservation.Description;
                worksheet.Cell(rowIndex, 3).Value = reservation.DateFrom;
                worksheet.Cell(rowIndex, 4).Value = reservation.DateTo;
                worksheet.Cell(rowIndex, 5).Value = reservation.CustomerID;
                worksheet.Cell(rowIndex, 6).Value = reservation.VehicleID;
                rowIndex++;
            }

            worksheet.ColumnsUsed().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            string fileName = $"Reservations_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("Reservation{id:int}")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> GetReservationByIdAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound("The reservation doesn't exist.");
            }
            return Ok(reservation);
        }

        [HttpPost("NewReservation")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> CreateReservationAsync(ReservationDTO reservationDTO)
        {
            var existingReservations = await _reservationRepository.GetAllReservationsAsync();

            var reservedVehicleBySameCustomer = existingReservations.FirstOrDefault(r => r.VehicleID == reservationDTO.VehicleID
                && r.CustomerID == reservationDTO.CustomerID && r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom);

            var reservedVehicleByOther = existingReservations.FirstOrDefault(r => r.VehicleID == reservationDTO.VehicleID
                && r.CustomerID != reservationDTO.CustomerID && r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom);

            var context = new ValidationContext<ReservationDTO>(reservationDTO);

            if (reservedVehicleBySameCustomer != null)
            {
                context.RootContextData["ReservationVehicleConflict"] = "sameCustomer";
                context.RootContextData["ExistingReservationDates"] = $"{reservedVehicleBySameCustomer.DateFrom:yyyy-MM-dd} to {reservedVehicleBySameCustomer.DateTo:yyyy-MM-dd}";
            }
            else if (reservedVehicleByOther != null)
            {
                context.RootContextData["ReservationVehicleConflict"] = "otherCustomer";
                context.RootContextData["ExistingReservationDates"] = $"{reservedVehicleByOther.DateFrom:yyyy-MM-dd} to {reservedVehicleByOther.DateTo:yyyy-MM-dd}";
            }
            context.RootContextData["ExistingReservations"] = existingReservations;
            context.RootContextData["OperationType"] = "create";

            var validationResult = _reservationValidator.Validate(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            var reservation = _mapper.Map<Reservation>(reservationDTO);
            var createdReservation = await _reservationRepository.AddReservationAsync(reservation);
            var resultDTO = _mapper.Map<ReservationDTO>(createdReservation);

            return Ok(resultDTO);
        }

        [HttpPut("ModifyReservation{id:int}")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> UpdateReservationAsync(int id, ReservationDTO reservationDTO)
        {
            var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (existingReservation == null)
            {
                return NotFound("The reservation doesn't exist.");
            }

            if (reservationDTO.CustomerID == 0)
            {
                reservationDTO.CustomerID = existingReservation.CustomerID;
            }

            var existingReservations = (await _reservationRepository.GetAllReservationsAsync())
                .Where(r => r.ReservationID != id)
                .ToList();

            var reservedVehicleBySameCustomer = existingReservations.FirstOrDefault(r => r.VehicleID == reservationDTO.VehicleID
                && r.CustomerID == reservationDTO.CustomerID && r.DateFrom <= reservationDTO.DateTo && r.DateTo >= reservationDTO.DateFrom);

            var reservedVehicleByOther = existingReservations.FirstOrDefault(r => r.VehicleID == reservationDTO.VehicleID
                && r.CustomerID != reservationDTO.CustomerID && r.DateFrom < reservationDTO.DateTo && r.DateTo > reservationDTO.DateFrom);

            var context = new ValidationContext<ReservationDTO>(reservationDTO);

            if (reservedVehicleBySameCustomer != null)
            {
                context.RootContextData["ReservationVehicleConflict"] = "sameCustomer";
                context.RootContextData["ExistingReservationDates"] = $"{reservedVehicleBySameCustomer.DateFrom:yyyy-MM-dd} to {reservedVehicleBySameCustomer.DateTo:yyyy-MM-dd}";
            }
            else if (reservedVehicleByOther != null)
            {
                context.RootContextData["ReservationVehicleConflict"] = "otherCustomer";
                context.RootContextData["ExistingReservationDates"] = $"{reservedVehicleByOther.DateFrom:yyyy-MM-dd} to {reservedVehicleByOther.DateTo:yyyy-MM-dd}";
            }

            context.RootContextData["ExistingReservations"] = existingReservations;
            context.RootContextData["OperationType"] = "update";

            var validationResult = _reservationValidator.Validate(context);

            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            var updatedReservation = _mapper.Map(reservationDTO, existingReservation);
            updatedReservation.ReservationID = id;
            updatedReservation.CustomerID = existingReservation.CustomerID;

            var result = await _reservationRepository.UpdateReservationAsync(id, updatedReservation);
            var resultDTO = _mapper.Map<ReservationDTO>(result);

            return Ok(resultDTO);
        }

        [HttpDelete("RemoveReservation{id:int}")]
        [Authorize(Policy = "AdminAndUserPolicy")]
        public async Task<IActionResult> CancelReservationAsync(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound("The reservation doesn't exist.");
            }

            var reservationDTO = _mapper.Map<ReservationDTO>(reservation);

            var context = new ValidationContext<ReservationDTO>(reservationDTO);
            context.RootContextData["OperationType"] = "delete";

            var validationResult = _reservationValidator.Validate(context);
            if (!validationResult.IsValid)
            {
                var errorResponse = ValidationErrorHandler.ErrorHandler(validationResult.Errors);
                return BadRequest(errorResponse);
            }

            await _reservationRepository.DeleteReservationAsync(id);
            return Ok();
        }
    }
}
