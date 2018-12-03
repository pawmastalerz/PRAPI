using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly IOrderRepository repo;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICloudinaryService cloudinaryService;
        private Cloudinary cloudinary;

        public OrdersController(
            ITokenService tokenService,
            IOrderRepository repo,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment,
            ICloudinaryService cloudinaryService)
        {
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.repo = repo;
            this.hostingEnvironment = hostingEnvironment;
            this.cloudinaryService = cloudinaryService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromForm]Order order)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var orderForCreate = this.mapper.Map<Order>(order);
                this.repo.CreateOrder(orderForCreate);
                await this.repo.SaveAll();
                return Ok("Order created successfully");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with saving order in database");
            }
        }

        [Authorize]
        [HttpGet("full/all")]
        public async Task<IActionResult> GetAllOrdersFull()
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var ordersFromRepo = await this.repo.GetAllOrders();

                if (ordersFromRepo != null)
                    return Ok(ordersFromRepo);

                return BadRequest("Problem fetching orders full description list");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching orders full description list");
            }
        }

        [Authorize]
        [HttpGet("full/{id}")]
        public async Task<IActionResult> GetOrderFull(int id)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var orderFromRepo = await this.repo.GetOrder(id);
                if (orderFromRepo != null)
                    return Ok(orderFromRepo);

                return BadRequest("Problem fetching order full description");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching order full description");
            }
        }

        [HttpGet("user/all")]
        public async Task<IActionResult> GetAllOrdersForUser()
        {
            try
            {
                var ordersFromRepo = await this.repo.GetAllOrders();

                if (ordersFromRepo != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<Order>>(ordersFromRepo);
                    return Ok(ordersToReturn);
                }

                return BadRequest("Problem fetching orders user description list");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching orders user description list");
            }
        }

        [HttpPost("user/search")]
        public async Task<IActionResult> SearchForUserOrders([FromBody] SearchParams searchParams)
        {
            try
            {
                if (searchParams.ReservedFrom > searchParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");

                var ordersFromRepo = await this.repo.SearchForUserOrders(searchParams);

                if (ordersFromRepo != null)
                {
                    var ordersToReturn = this.mapper.Map<List<Order>, List<Order>>(ordersFromRepo);
                    return Ok(ordersToReturn);
                }

                return BadRequest("Problem fetching searched orders for user");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching searched orders for user");
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetOrderForUser(int id)
        {
            try
            {
                var orderFromRepo = await this.repo.GetOrder(id);
                if (orderFromRepo != null)
                {
                    var orderToReturn = this.mapper.Map<Order, Order>(orderFromRepo);
                    return Ok(orderToReturn);
                }

                return BadRequest("Problem fetching order user description");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching order user description");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrder([FromForm]Order orderForUpdate)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var orderFromRepo = await this.repo.GetOrder(orderForUpdate.OrderId);

                this.mapper.Map(orderForUpdate, orderFromRepo);
                await this.repo.SaveAll();
                return Ok("Order updated successfully");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with updating order in database");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var orderInRepo = await this.repo.GetOrder(id);
                this.repo.Delete(orderInRepo);
                await this.repo.SaveAll();
                return Ok("Order with id " + id + " deleted successfully");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with deleting order in database");
            }
        }
    }
}
