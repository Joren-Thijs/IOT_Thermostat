﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IOT_Thermostat.API.DTO;
using IOT_Thermostat.API.DTO.Interfaces;
using IOT_Thermostat.API.Entity.Device;
using IOT_Thermostat.API.Entity.Interfaces;
using IOT_Thermostat.API.Entity.ThermostatDevice;

namespace IOT_Thermostat.API.Profiles
{
    public class IDeviceProfile : Profile
    {
        public IDeviceProfile()
        {
            CreateMap<IDevice, IDeviceDto>()
                .Include<Device, DeviceDto>()
                .Include<ThermostatDevice, ThermostatDeviceDto>();

            CreateMap<Device, DeviceDto>();
            CreateMap<ThermostatDevice, ThermostatDeviceDto>();
        }
    }
}
