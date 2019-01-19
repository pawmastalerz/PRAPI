using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PRAPI.Data;
using PRAPI.Dtos;
using PRAPI.Helpers;
using PRAPI.Models;
using PRAPI.Services;

namespace PRAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly ICarRepository carRepo;
        private readonly IOrderRepository repo;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;

        public OrdersController(
            ITokenService tokenService,
            ICarRepository carRepo,
            IOrderRepository repo,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment)
        {
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.carRepo = carRepo;
            this.repo = repo;
            this.hostingEnvironment = hostingEnvironment; ;
        }

        [AllowAnonymous]
        [HttpPost("cprice")]
        public async Task<IActionResult> CalculatePrice([FromBody] OrderParams orderParams)
        {
            try
            {
                if (orderParams.ReservedFrom > orderParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");
                else if (orderParams.ReservedFrom < DateTime.Now.AddDays(-1) || orderParams.ReservedTo < DateTime.Now.AddDays(-1))
                    return BadRequest("Reservation's date is lower than current time");
                else if (orderParams.ReservedFrom > DateTime.Now.AddMonths(6) || orderParams.ReservedTo > DateTime.Now.AddMonths(6))
                    return BadRequest("Reservation's date is bigger than 6 months");

                var dayDifference = (orderParams.ReservedTo - orderParams.ReservedFrom).TotalDays;
                var carFromRepo = await this.carRepo.GetCar(orderParams.CarId);
                var calculatedPrice = this.repo.CalculatePrice(dayDifference, carFromRepo.Price);

                return Ok(calculatedPrice);
            }
            catch (System.Exception)
            {
                return BadRequest("Problem calculating price");
            }
        }

        [Authorize]
        [HttpPut("create")]
        public IActionResult CreateOrder([FromBody] OrderParams orderParams)
        {
            try
            {
                if (orderParams.ReservedFrom > orderParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");
                else if (orderParams.ReservedFrom < DateTime.Now.AddDays(-1) || orderParams.ReservedTo < DateTime.Now.AddDays(-1))
                    return BadRequest("Reservation's date is lower than current time");
                else if (orderParams.ReservedFrom > DateTime.Now.AddMonths(6) || orderParams.ReservedTo > DateTime.Now.AddMonths(6))
                    return BadRequest("Reservation's date is bigger than 6 months");

                var bearerToken = Request.Headers["Authorization"].ToString();
                if (!this.tokenService.CheckIfSameUser(bearerToken, orderParams.UserId))
                    return Unauthorized();

                var newOrder = this.mapper.Map<Order>(orderParams);

                if (this.repo.CreateOrder(newOrder))
                    return Ok();

                return BadRequest("Requested car is unavaliable");
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentlyOrdered()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString();

                var currentOrders = await this.repo.GetCurrentOrdersForUser(
                    Int32.Parse(this.tokenService.GetUserId(bearerToken)));

                if (currentOrders != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<OrderDetailDto>>(currentOrders);
                    return Ok(ordersToReturn);
                }
                else return BadRequest("Problem fetching currently ordered cars list");
            }
            catch (AppException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderedHistory()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString();

                var ordersHistory = await this.repo.GetAllOrdersForUser(
                    Int32.Parse(this.tokenService.GetUserId(bearerToken)));

                if (ordersHistory != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<OrderDetailDto>>(ordersHistory);
                    return Ok(ordersToReturn);
                }
                else return BadRequest("Problem fetching currently ordered cars list");
            }
            catch (AppException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("return/{orderId}")]
        public IActionResult ReturnCar(int orderId)
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString();
                var orderToMark = this.repo.GetOrderById(orderId);

                if (!this.tokenService.CheckIfSameUser(bearerToken, orderToMark.UserId))
                    return Unauthorized();

                if (this.repo.MarkAsReturned(orderId) != false)
                    return Ok();
                else return BadRequest("Problem fetching currently ordered cars list");
            }
            catch (AppException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("admin/all")]
        public async Task<IActionResult> AdminGetAllOrders()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString();
                if (!this.tokenService.CheckIfAdmin(bearerToken))
                    return Unauthorized();

                var allOrders = await this.repo.AdminGetAllOrders();

                if (allOrders != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<OrderDetailDto>>(allOrders);
                    return Ok(ordersToReturn);
                }
                else return BadRequest("Problem fetching all orders for admin");
            }
            catch (AppException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("admin/current")]
        public async Task<IActionResult> AdminGetAllCurrentOrders()
        {
            try
            {
                var bearerToken = Request.Headers["Authorization"].ToString();
                if (!this.tokenService.CheckIfAdmin(bearerToken))
                    return Unauthorized();

                var allOrders = await this.repo.AdminGetAllCurrentOrders();

                if (allOrders != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<OrderDetailDto>>(allOrders);
                    return Ok(ordersToReturn);
                }
                else return BadRequest("Problem fetching all current orders for admin");
            }
            catch (AppException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
