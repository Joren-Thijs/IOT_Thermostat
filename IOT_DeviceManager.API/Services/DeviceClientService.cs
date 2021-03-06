﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IOT_DeviceManager.API.DeviceClient;
using IOT_DeviceManager.API.Entity.Device;
using IOT_DeviceManager.API.Entity.Interfaces;
using IOT_DeviceManager.API.Repositories;
using Microsoft.Extensions.Hosting;

namespace IOT_DeviceManager.API.Services
{
    public class DeviceClientService : IHostedService
    {
        private readonly IDeviceClient _deviceClient;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceClientService(IDeviceClient deviceClient, IDeviceRepository deviceRepository)
        {
            _deviceClient = deviceClient ??
                throw new ArgumentNullException(nameof(deviceClient));
            _deviceRepository = deviceRepository ??
                throw new ArgumentNullException(nameof(deviceRepository));
            _deviceClient.DeviceMeasurementReceived += new EventHandler<DeviceMeasurementEventArgs>(
                async (s, e) =>
                {
                    await DeviceClientOnDeviceMeasurementReceived(s, e);
                });
            _deviceClient.DeviceDisconnected += new EventHandler<DeviceDisconnectedEventArgs>(
                async (s, e) =>
                {
                    await DeviceClientOnDeviceDisconnected(s, e);
                });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _deviceClient.StartClientAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _deviceClient.StopClientAsync();
        }

        public async Task<IDeviceStatus> SetDeviceStatusAsync(IDevice device, IDeviceStatus status)
        {
            return await _deviceClient.SetDeviceStatus(device, status);
        }

        private async Task DeviceClientOnDeviceMeasurementReceived(object sender, DeviceMeasurementEventArgs e)
        {
            var device = await _deviceRepository.GetDevice(e.DeviceId);
            if (device == null)
            {
                device = await AddDeviceToDeviceRepository(e);
            }

            await UpdateCurrentDeviceStatus(e.DeviceMeasurement, device);

            await _deviceRepository.AddMeasurement(e.DeviceId, e.DeviceMeasurement);
            await _deviceRepository.Save();
        }

        private async Task UpdateCurrentDeviceStatus(IDeviceMeasurement deviceMeasurement, IDevice device)
        {
            device.Status = deviceMeasurement.Status;
            device.LastSeen = deviceMeasurement.TimeStamp;
            device.Online = true;
            await _deviceRepository.UpdateDevice(device);
            await _deviceRepository.Save();
        }

        private async Task<IDevice> AddDeviceToDeviceRepository(DeviceMeasurementEventArgs e)
        {
            IDevice device = new Device()
            {
                Id = e.DeviceId,
                DeviceType = e.DeviceType,
                Status = e.DeviceMeasurement.Status,
                LastSeen = e.DeviceMeasurement.TimeStamp,
                Online = true,
            };

            device = await _deviceRepository.AddDevice(device);
            await _deviceRepository.Save();
            return device;
        }

        private async Task DeviceClientOnDeviceDisconnected(object s, DeviceDisconnectedEventArgs e)
        {
            var device = await _deviceRepository.GetDevice(e.DeviceId);
            if (device != null)
            {
                device.Online = false;
                device.LastSeen = DateTime.Now;
                await _deviceRepository.UpdateDevice(device);
                await _deviceRepository.Save();
            }
        }
    }
}
