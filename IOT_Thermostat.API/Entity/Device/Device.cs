﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IOT_Thermostat.API.Entity.Interfaces;

namespace IOT_Thermostat.API.Entity.Device
{
    public class Device : IDevice
    {
        [Key]
        public string Id { get; set; }
        public string DeviceName { get; set; }
        public IDeviceStatus Status { get; set; }
        public IEnumerable<IDeviceMeasurement> Measurements { get; set; } = new List<DeviceMeasurement>();
    }
}
