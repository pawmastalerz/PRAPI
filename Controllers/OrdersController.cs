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
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("cprice")]
        public async Task<IActionResult> CalculatePrice([FromBody] OrderParams orderParams)
        {
            try
            {
                if (orderParams.ReservedFrom > orderParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");
                if (orderParams.ReservedFrom < DateTime.Now.AddDays(-1) || orderParams.ReservedTo < DateTime.Now.AddDays(-1))
                    return BadRequest("Reservation's date is lower than current time");
                if (orderParams.ReservedFrom > DateTime.Now.AddMonths(6) || orderParams.ReservedTo > DateTime.Now.AddMonths(6))
                    return BadRequest("Reservation's date is bigger than 6 months");

                var dayDifference = (orderParams.ReservedTo - orderParams.ReservedFrom).TotalDays;
                var carFromRepo = await this.carRepo.GetCar(orderParams.Id);

                return Ok(this.repo.CalculatePrice(dayDifference, carFromRepo.Price));
            }
            catch (System.Exception)
            {
                return BadRequest("Problem calculating price");
            }
        }
    }
}
