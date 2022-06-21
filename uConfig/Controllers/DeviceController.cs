using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        public DeviceController(ILogger<DeviceController> logger, MySqlConnection dbConnection)
        {
            _logger = logger;
            _deviceRepository = new DeviceRepository(dbConnection);
            _deviceActivityRepository = new DeviceActivityRepository(dbConnection);
            _authenticationService = new AuthenticationService(dbConnection.ConnectionString);
        }

        #region Device Management

        [HttpPost]
        public async Task<IActionResult> RegisterDevice(Device device)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.Role.Equals("demo"))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Register Device call from {email}, new device name: {deviceName}", loggedInUser.Email, device.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            device.UserID = loggedInUser.UserID;

            if (!await _deviceRepository.IsDeviceAlreadyRegistered(device))
            {
                // happy path, 201 created after adding
                await _deviceRepository.RegisterDevice(device);
                return Created("/device/" + device.DeviceID, device);
            } else
            {
                // sending 409 conflict in case resource already exist
                return Conflict();
            }
        }

        [HttpPut]
        [Route("{deviceId}")]
        public async Task<IActionResult> UpdateDevice(Guid deviceId, Device device)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.Role.Equals("demo"))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Update Device call from {email}, new device name: {deviceName}", loggedInUser.Email, device.Name);

            if (!ModelState.IsValid || deviceId != device.DeviceID)
            {
                return BadRequest();
            }

            List<Device> devices = await _deviceRepository.GetDevices(loggedInUser.UserID);
            Device deviceToUpdate = devices.Find(registeredDevice => registeredDevice.DeviceID == deviceId);
            if (deviceToUpdate.UserID != loggedInUser.UserID)
            {
                return Unauthorized();
            }

            if (devices.Find(registeredDevice => registeredDevice.Name.Equals(device.Name)) != null)
            {
                return Conflict();
            }

            device.UserID = loggedInUser.UserID;

            await _deviceRepository.UpdateDevice(device);
            return NoContent();
        }

        [HttpDelete]
        [Route("{deviceId}")]
        public async Task<IActionResult> DeleteDevice(Guid deviceId)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.Role.Equals("demo"))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Delete Device call from {email}", loggedInUser.Email);

            Device deviceToDelete = await _deviceRepository.GetDeviceById(deviceId);
            if (deviceToDelete == null)
            {
                return NotFound();
            }

            if (deviceToDelete.UserID != loggedInUser.UserID)
            {
                return Unauthorized();
            }

            await _deviceRepository.DeleteDevice(deviceId);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Get Devices call from {email}", loggedInUser.Email);

            return Ok(
                new GetDevicesResponse(
                    await _deviceRepository.GetDevices(loggedInUser.UserID),
                    await _deviceActivityRepository.GetLastSeenForDevices(loggedInUser.UserID)
                )
            );
        }

        #endregion Device Management

        #region Device Config Management

        [HttpPut]
        [Route("{deviceId}/config")]
        public async Task<IActionResult> CreateOrUpdateDeviceConfig(Guid deviceId, DeviceConfig deviceConfig)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.Role.Equals("demo"))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Create/Update Device Config call from {email}", loggedInUser.Email);

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound();
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized();
            }

            await _deviceRepository.CreateOrUpdateDeviceConfig(deviceId, deviceConfig);
            return NoContent();
        }

        /// <summary>
        /// Called by Device, UI
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{deviceId}/config")]
        public async Task<IActionResult> GetDeviceConfig(Guid deviceId)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Get Device Config call from {email}", loggedInUser.Email);

            Device device = await _deviceRepository.GetDeviceById(deviceId);
            DeviceConfig deviceConfig = await _deviceRepository.GetDeviceConfig(deviceId);

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
                await _deviceActivityRepository.RegisterDeviceCheckin(deviceId.ToString(), "config");
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
        public async Task<IActionResult> GetDeviceLogs(Guid deviceId)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Get device logs call from device {device}", deviceId);

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            return Ok(await _deviceActivityRepository.GetLogs(deviceId.ToString()));
        }

        /// <summary>
        /// Called by Device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="checkinRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{deviceId}/logs")]
        public async Task<IActionResult> StoreLogs(Guid deviceId, StoreLogsRequest checkinRequest)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.Role.Equals("demo"))
            {
                return Unauthorized();
            }

            _logger.LogInformation("Device checkin call from device {device}", deviceId);

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            await _deviceActivityRepository.RegisterDeviceCheckin(deviceId.ToString(), "logs");
            await _deviceActivityRepository.AppendLogs(deviceId.ToString(), Encoding.UTF8.GetString(Convert.FromBase64String(checkinRequest.Logs)));

            return NoContent();
        }

        [HttpGet]
        [Route("{deviceId}/activity")]
        public async Task<IActionResult> GetDeviceActivity(Guid deviceId)
        {
            LoggedInUser loggedInUser = await _authenticationService.Authenticate(HttpContext.Request.Headers["Authorization"]);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            _logger.LogInformation("Get Device Config call from {email}", loggedInUser.Email);

            Device device = await _deviceRepository.GetDeviceById(deviceId);

            if (device == null)
            {
                return NotFound(new ErrorResponse() { Message = "Device not found" });
            }

            if (device.UserID != loggedInUser.UserID)
            {
                return Unauthorized(new ErrorResponse() { Message = "Device belongs to another user" });
            }

            DateTime? lastSeen = await _deviceActivityRepository.GetLastSeen(deviceId.ToString());
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
