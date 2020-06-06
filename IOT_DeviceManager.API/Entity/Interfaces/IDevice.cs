﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IOT_DeviceManager.API.Entity.Interfaces
{
    public interface IDevice
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        [RegularExpression(@"^[\w'À-ÿ\.\-\/@][\w' À-ÿ\.\-\/@]*[\w'À-ÿ\.\-\/@]+$")]
        public string DeviceType { get; set; }
        [MaxLength(50)]
        [RegularExpression(@"^[\w'À-ÿ\.\-\/@][\w' À-ÿ\.\-\/@]*[\w'À-ÿ\.\-\/@]+$")]
        public string DeviceName { get; set; }
        public IDeviceStatus Status { get; set; }
        public IEnumerable<IDeviceMeasurement> Measurements { get; set; }
    }
}