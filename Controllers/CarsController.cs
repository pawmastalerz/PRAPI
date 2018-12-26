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
    public class CarsController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly ICarRepository repo;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;

        public CarsController(
            ITokenService tokenService,
            ICarRepository repo,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment)
        {
            this.mapper = mapper;
            this.tokenService = tokenService;
            this.repo = repo;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        [HttpGet("user/all")]
        public async Task<IActionResult> GetAllCarsForUser()
        {
            try
            {
                var carsFromRepo = await this.repo.GetAllCars();

                if (carsFromRepo != null)
                {
                    var carsToReturn = this.mapper.Map<List<Car>, List<CarDetailsForUserDto>>(carsFromRepo);
                    return Ok(carsToReturn);
                }

                return BadRequest("Problem fetching cars user description list");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching cars user description list");
            }
        }

        [AllowAnonymous]
        [HttpGet("models")]
        public async Task<IActionResult> GetAllCarModels()
        {
            try
            {
                var carsFromRepo = await this.repo.GetAllCarModels();

                if (carsFromRepo != null)
                    return Ok(carsFromRepo);

                return BadRequest("Problem fetching all car models list");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching all car models list");
            }
        }

        [AllowAnonymous]
        [HttpPost("user/search")]
        public async Task<IActionResult> SearchForCarsForUser([FromBody] SearchParams searchParams)
        {
            try
            {
                if (searchParams.ReservedFrom > searchParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");
                if (searchParams.ReservedFrom < DateTime.Now.AddDays(-1) || searchParams.ReservedTo < DateTime.Now.AddDays(-1))
                    return BadRequest("Reservation's date is lower than current time");
                if (searchParams.ReservedFrom > DateTime.Now.AddMonths(6) || searchParams.ReservedTo > DateTime.Now.AddMonths(6))
                    return BadRequest("Reservation's date is bigger than 6 months");

                var carsFromRepo = await this.repo.SearchForCarsForUser(searchParams);

                if (carsFromRepo != null)
                    return Ok(carsFromRepo);

                return BadRequest("Problem fetching searched cars for user");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching searched cars for user");
            }
        }

        [AllowAnonymous]
        [HttpGet("user/{carId}")]
        public async Task<IActionResult> GetCarForUser(int carId)
        {
            try
            {
                var carFromRepo = await this.repo.GetCar(carId);
                if (carFromRepo != null)
                {
                    var carToReturn = this.mapper.Map<Car, CarDetailsForUserDto>(carFromRepo);
                    return Ok(carToReturn);
                }

                return BadRequest("Problem fetching car user description");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching car user description");
            }
        }
    }
}
