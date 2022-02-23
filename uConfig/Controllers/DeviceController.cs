﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using uConfig.DTOs;
using uConfig.Model;
using uConfig.Repository;
using uConfig.Services;

namespace uConfig.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private IDeviceRepository _deviceRepository;
        private IDeviceActivityRepository _deviceActivityRepository;
        private AuthenticationService _authenticationService;

        public DeviceController(ILogger<DeviceController> logger)
        {
            _logger = logger;
            _deviceRepository = new InMemoryDeviceRepository();
            _deviceActivityRepository = new DeviceActivityRepository();
            _authenticationService = new AuthenticationService();
        }

        #region Device Management

        [HttpPost]
        public void RegisterDevice(Device device)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Register Device call from {email}, new device name: {deviceName}", loggedInUser.Email, device.Name);

            if (!ModelState.IsValid)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            device.UserID = loggedInUser.UserID;

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
        [Route("{deviceId}")]
        public void UpdateDevice(Guid deviceId, Device device)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Update Device call from {email}, new device name: {deviceName}", loggedInUser.Email, device.Name);

            if (!ModelState.IsValid || deviceId != device.DeviceID)
            {
                HttpContext.Response.StatusCode = 400;
                return;
            }

            List<Device> devices = _deviceRepository.GetDevices(loggedInUser.UserID);
            Device deviceToUpdate = devices.Find(registeredDevice => registeredDevice.DeviceID == deviceId);
            if (deviceToUpdate.UserID != loggedInUser.UserID)
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            if (devices.Find(registeredDevice => registeredDevice.Name.Equals(device.Name)) != null)
            {
                HttpContext.Response.StatusCode = 409;
                return;
            }

            device.UserID = loggedInUser.UserID;

            _deviceRepository.UpdateDevice(device);
            HttpContext.Response.StatusCode = 204;
        }

        [HttpDelete]
        [Route("{deviceId}")]
        public void DeleteDevice(Guid deviceId)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Delete Device call from {email}", loggedInUser.Email);

            Device deviceToDelete = _deviceRepository.GetDeviceById(deviceId);
            if (deviceToDelete == null)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if (deviceToDelete.UserID != loggedInUser.UserID)
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            _deviceRepository.DeleteDevice(deviceId);
            HttpContext.Response.StatusCode = 204;
        }

        [HttpGet]
        public List<Device> GetDevices()
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Get Devices call from {email}", loggedInUser.Email);

            return _deviceRepository.GetDevices(loggedInUser.UserID);
        }

        #endregion Device Management

        #region Device Config Management

        [HttpPut]
        [Route("{deviceId}/config")]
        public void CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Create/Update Device Config call from {email}", loggedInUser.Email);

            Device device = _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if (device.UserID != loggedInUser.UserID)
            {
                HttpContext.Response.StatusCode = 401;
                return;
            }

            _deviceRepository.CreateOrUpdateDeviceConfig(deviceId, deviceConfig);
            HttpContext.Response.StatusCode = 204;
        }

        [HttpGet]
        [Route("{deviceId}/config")]
        public IActionResult GetDeviceConfig(Guid deviceId)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Get Device Config call from {email}", loggedInUser.Email);

            Device device = _deviceRepository.GetDeviceById(deviceId);
            DeviceConfig deviceConfig = _deviceRepository.GetDeviceConfig(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Not Found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            // device requested for stored config, so we assume that device is connected and active
            if (!HttpContext.Request.Query.ContainsKey("origin") || !HttpContext.Request.Query["origin"].Equals("web"))
            {
                _deviceActivityRepository.RegisterDeviceCheckin(deviceId.ToString());
            }
            
            if (deviceConfig == null)
            {
                if (HttpContext.Request.ContentType == null || HttpContext.Request.ContentType.Equals("application/json"))
                {
                    return Ok(DeviceConfig.Default);
                } else
                {
                    return Ok(DeviceConfig.Default.ToTextPlain());
                }
                
            }

            if (HttpContext.Request.ContentType == null || HttpContext.Request.ContentType.Equals("application/json"))
            {
                return Ok(deviceConfig);
            } else
            {
                return Ok(deviceConfig.ToTextPlain());
            }
                
        }

        #endregion Device Config Management

        #region Device Activity Management

        [HttpGet]
        [Route("{deviceId}/logs")]
        public IActionResult GetDeviceLogs(Guid deviceId)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Get device logs call from device {device}", deviceId);

            Device device = _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            return Ok(_deviceActivityRepository.GetLogs(deviceId.ToString()));
        }

        [HttpPost]
        [Route("{deviceId}/logs")]
        public IActionResult StoreLogs(Guid deviceId, StoreLogsRequest checkinRequest)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Device checkin call from device {device}", deviceId);

            Device device = _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            _deviceActivityRepository.RegisterDeviceCheckin(deviceId.ToString());
            _deviceActivityRepository.AppendLogs(deviceId.ToString(), Encoding.UTF8.GetString(Convert.FromBase64String(checkinRequest.Logs)));

            return NoContent();
        }

        [HttpGet]
        [Route("{deviceId}/activity")]
        public IActionResult GetDeviceActivity(Guid deviceId)
        {
            LoggedInUser loggedInUser = _authenticationService.GetLoggedInUser(HttpContext.Request.Headers["Authorization"]);
            _logger.LogInformation("Get Device Config call from {email}", loggedInUser.Email);

            Device device = _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            DateTime? lastSeen = _deviceActivityRepository.GetLastSeen(deviceId.ToString());
            if (lastSeen.HasValue)
            {
                return Ok((int)DateTime.Now.Subtract(lastSeen.Value).TotalSeconds);
            } else
            {
                return NoContent();
            }
        }

        #endregion
    }
}
