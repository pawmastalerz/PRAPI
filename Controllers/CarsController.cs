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
using CloudinaryDotNet;

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
        private readonly ICloudinaryService cloudinaryService;

        public CarsController(
            ITokenService tokenService,
            ICarRepository repo,
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
        public async Task<IActionResult> CreateCar([FromForm]Car carToCreate)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var file = Request.Form.Files[0];
                if (!this.cloudinaryService.CheckFile(file))
                    return BadRequest("Uploaded file is not a jpeg or too big ( > 6 MB )");

                var uploadResult = this.cloudinaryService.UploadFile(file);
                carToCreate.PublicId = uploadResult.PublicId.ToString();
                carToCreate.PhotoUrl = uploadResult.Uri.ToString();
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with uploading car photo to cloudinary");
            }

            try
            {
                var carForCreate = this.mapper.Map<Car>(carToCreate);
                this.repo.CreateCar(carForCreate);
                await this.repo.SaveAll();
                return Ok();
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with saving car in database");
            }
        }

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCars()
        {
            try
            {
                var carsFromRepo = await this.repo.GetAllCars();

                if (carsFromRepo != null)
                {
                    var carsToReturn = this.mapper.Map<List<Car>, List<CarDetailsDto>>(carsFromRepo);
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
        [HttpPost("search")]
        public async Task<IActionResult> SearchForCars([FromBody] SearchParams searchParams)
        {
            try
            {
                int modelIndex = searchParams.Model.IndexOf(" ") + 1;
                searchParams.Model = searchParams.Model.Substring(modelIndex);

                if (searchParams.ReservedFrom > searchParams.ReservedTo)
                    return BadRequest("Reservation's start is bigger than reservation's end");
                if (searchParams.ReservedFrom < DateTime.Now.AddDays(-1) || searchParams.ReservedTo < DateTime.Now.AddDays(-1))
                    return BadRequest("Reservation's date is lower than current time");
                if (searchParams.ReservedFrom > DateTime.Now.AddMonths(6) || searchParams.ReservedTo > DateTime.Now.AddMonths(6))
                    return BadRequest("Reservation's date is bigger than 6 months");

                var carsFromRepo = await this.repo.SearchForCars(searchParams);

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
        [HttpGet("{carId}")]
        public async Task<IActionResult> GetCar(int carId)
        {
            try
            {
                var carFromRepo = await this.repo.GetCar(carId);
                if (carFromRepo != null)
                {
                    var carToReturn = this.mapper.Map<Car, CarDetailsDto>(carFromRepo);
                    return Ok(carToReturn);
                }

                return BadRequest("Problem fetching car user description");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching car user description");
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCar([FromForm]Car carForUpdate)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var carFromRepo = await this.repo.GetCar(carForUpdate.CarId);
                if (Request.Form.Files.Count > 0)
                {
                    try
                    {
                        var file = Request.Form.Files[0];
                        if (!this.cloudinaryService.CheckFile(file))
                            return BadRequest("Uploaded file is not a jpeg or too big ( > 6 MB )");

                        this.cloudinaryService.DeleteFile(carFromRepo.PublicId);

                        var uploadResult = this.cloudinaryService.UploadFile(file);
                        carForUpdate.PublicId = uploadResult.PublicId.ToString();
                        carForUpdate.PhotoUrl = uploadResult.Uri.ToString();
                    }
                    catch (System.Exception)
                    {
                        return BadRequest("Problem with updating car photo to cloudinary");
                    }
                }
                if (Request.Form.Files.Count == 0)
                {
                    carForUpdate.PublicId = carFromRepo.PublicId.ToString();
                    carForUpdate.PhotoUrl = carFromRepo.PhotoUrl.ToString();
                }

                try
                {
                    this.mapper.Map(carForUpdate, carFromRepo);
                    await this.repo.SaveAll();
                    return Ok();
                }
                catch (System.Exception)
                {
                    return BadRequest("Problem with updating car in database");
                }
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with updating car");
            }
        }

        [Authorize]
        [HttpDelete("{carId}")]
        public async Task<IActionResult> DeleteCar(int carId)
        {
            var bearerToken = Request.Headers["Authorization"].ToString();
            if (!this.tokenService.CheckIfAdmin(bearerToken))
                return Unauthorized();

            try
            {
                var carInRepo = await this.repo.GetCar(carId);
                if (this.cloudinaryService.DeleteFile(carInRepo.PublicId))
                {
                    try
                    {
                        this.repo.Delete(carInRepo);
                        await this.repo.SaveAll();
                        return Ok("Car with id " + carId + " deleted successfully");
                    }
                    catch (System.Exception)
                    {
                        return BadRequest("Problem with deleting car in database");
                    }
                }
                return BadRequest("Problem with deleting car photo in cloudinary");

            }
            catch (System.Exception)
            {
                return BadRequest("Problem with deleting car");
            }
        }
    }
}
