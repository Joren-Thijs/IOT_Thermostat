﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IOT_Thermostat.API.DTO.Interfaces;

namespace IOT_Thermostat.API.DTO
{
    public class DeviceDto : IDeviceDto
    {
        public string Id { get; set; }
        public string DeviceName { get; set; }
        public IDeviceStatusDto Status { get; set; }
    }
}