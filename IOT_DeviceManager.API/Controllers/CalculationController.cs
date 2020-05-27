﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IOT_DeviceManager.API.DTO.Calculations;
using IOT_DeviceManager.API.DTO.Interfaces;
using IOT_DeviceManager.API.Helpers.Extensions;
using IOT_DeviceManager.API.Repositories;
using IOT_DeviceManager.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IOT_DeviceManager.API.Controllers
{
    [ApiController]
    [Route("api/devices/{deviceId}/measurements/calculations")]
    public class CalculationController : ControllerBase
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly ICalculationService _calculationService;
        private readonly IMapper _mapper;

        public CalculationController(IDeviceRepository deviceRepository, ICalculationService calculationService, IMapper mapper)
        {
            _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
            _calculationService = calculationService ?? throw new ArgumentNullException(nameof(calculationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("totalontime")]
        public async Task<IActionResult> GetTotalOnTime([FromRoute] string deviceId)
        {
            var deviceExists = await _deviceRepository.DeviceExists(deviceId);
            if (!deviceExists)
            {
                return NotFound();
            }

            var measurements = await _deviceRepository.GetMeasurements(deviceId);

            var calculationResult = _calculationService.CalculateTotalOnTime(measurements);

            var result = new CalculationResultDto
            {
                Values = new Dictionary<string, object> {{"totalontime", calculationResult}}
            };

            return Ok(result.SerializeJson());
        }

        [HttpGet("average/{propertyName}")]
        public async Task<IActionResult> GetAverage([FromRoute] string deviceId, [FromRoute] string propertyName)
        {
            var deviceExists = await _deviceRepository.DeviceExists(deviceId);
            if (!deviceExists)
            {
                return NotFound();
            }

            var measurements = await _deviceRepository.GetMeasurements(deviceId);
            if (!measurements.Any())
            {
                return NotFound();
            }

            if (!measurements.FirstOrDefault().Values.ToDictionary(k => k.Key.ToLower(), k => k.Value).ContainsKey(propertyName.ToLower()))
            {
                return BadRequest();
            }

            var calculationResult = _calculationService.CalculateAverage(measurements, propertyName);

            var result = new CalculationResultDto
            {
                Values = new Dictionary<string, object> { { propertyName, calculationResult } }
            };

            return Ok(result.SerializeJson());
        }
    }
}