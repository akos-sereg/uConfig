using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using uConfig.Model;
using uConfig.Repository;
using uConfig.Services;

namespace uConfig.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private IDeviceRepository _deviceRepository;
        private AuthenticationService _authenticationService;

        public DeviceController(ILogger<DeviceController> logger)
        {
            _logger = logger;
            _deviceRepository = new InMemoryDeviceRepository();
            _authenticationService = new AuthenticationService();
        }

        [HttpPost]
        public void RegisterDevice(Device device)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser();
            _logger.LogInformation("Register Device call from {email}, new device name: {deviceName}", loggedInUser.Email, device.Name);

            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            device.OwnerEmail = loggedInUser.Email;

            if (!_deviceRepository.IsDeviceAlreadyRegistered(device))
            {
                // happy path, 201 created after adding
                _deviceRepository.RegisterDevice(device);
                HttpContext.Response.StatusCode = 201;
            } else
            {
                // sending 409 conflict in case resource already exist
                HttpContext.Response.StatusCode = 409;
            }
        }

        [HttpPut]
        [Route("{deviceId}/config")]
        public void CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser();
            _logger.LogInformation("Create/Update Device Config call from {email}", loggedInUser.Email);

            Device device = _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if (!device.OwnerEmail.Equals(loggedInUser.Email))
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            _deviceRepository.CreateOrUpdateDeviceConfig(deviceId, deviceConfig);
        }

        [HttpGet]
        [Route("{deviceId}/config")]
        public DeviceConfig GetDeviceConfig(Guid deviceId)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser();
            _logger.LogInformation("Get Device Config call from {email}", loggedInUser.Email);

            Device device = _deviceRepository.GetDeviceById(deviceId);
            DeviceConfig deviceConfig = _deviceRepository.GetDeviceConfig(deviceId);
            if (deviceConfig == null || device == null)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }

            if (!device.OwnerEmail.Equals(loggedInUser.Email))
            {
                HttpContext.Response.StatusCode = 401;
                return null;
            }

            return deviceConfig;
        }

        [HttpGet]
        public List<Device> GetDevices()
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser();
            _logger.LogInformation("Get Devices call from {email}", loggedInUser.Email);

            return _deviceRepository.GetDevices(loggedInUser.Email);
        }
    }
}
