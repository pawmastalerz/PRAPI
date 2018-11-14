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
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository repo;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ICloudinaryService cloudinaryService;
        private Cloudinary cloudinary;

        public CarsController(
            ICarRepository repo,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment,
            ICloudinaryService cloudinaryService)
        {
            this.mapper = mapper;
            this.repo = repo;
            this.hostingEnvironment = hostingEnvironment;
            this.cloudinaryService = cloudinaryService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCar([FromForm]CarForCreateDto carForCreateDto)
        {
            try
            {
                var file = Request.Form.Files[0];
                if (!this.cloudinaryService.CheckFile(file)) return BadRequest();

                var uploadResult = this.cloudinaryService.UploadFile(file);
                carForCreateDto.PublicId = uploadResult.PublicId.ToString();
                carForCreateDto.PhotoUrl = uploadResult.Uri.ToString();
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with uploading car photo to cloudinary");
            }

            try
            {
                var carForCreate = this.mapper.Map<Car>(carForCreateDto);
                this.repo.CreateCar(carForCreate);
                await this.repo.SaveAll();
                return Ok("Car created successfully");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with saving car in database");
            }
        }

        [Authorize]
        [HttpGet("full/all")]
        public async Task<IActionResult> GetAllCarsFull()
        {
            try
            {
                var carsFromRepo = await this.repo.GetAllCars();

                if (carsFromRepo != null)
                    return Ok(carsFromRepo);

                return BadRequest("Problem fetching cars full description list");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching cars full description list");
            }
        }

        [Authorize]
        [HttpGet("full/{id}")]
        public async Task<IActionResult> GetCarFull(int id)
        {
            try
            {
                var carFromRepo = await this.repo.GetCar(id);
                if (carFromRepo != null)
                    return Ok(carFromRepo);

                return BadRequest("Problem fetching car full description");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem fetching car full description");
            }
        }

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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetCarForUser(int id)
        {
            try
            {
                var carFromRepo = await this.repo.GetCar(id);
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


        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCar([FromForm]CarDetailsFullDto carForUpdate)
        {
            // NOT COMPLETED
            try
            {
                var file = Request.Form.Files[0];
                if (!this.cloudinaryService.CheckFile(file)) return BadRequest();

                var uploadResult = this.cloudinaryService.UploadFile(file);
                carForUpdate.PhotoUrl = uploadResult.Uri.ToString();
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with uploading car photo to cloudinary");
            }

            try
            {
                var carForCreate = this.mapper.Map<Car>(carForUpdate);
                this.repo.CreateCar(carForCreate);
                await this.repo.SaveAll();
                return Ok("Car created successfully");
            }
            catch (System.Exception)
            {
                return BadRequest("Problem with saving car in database");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var carInRepo = await this.repo.GetCar(id);
                if (this.cloudinaryService.DeleteFile(carInRepo.PublicId))
                {
                    this.repo.Delete(carInRepo);
                    await this.repo.SaveAll();
                    return Ok("Car with id " + id + " deleted successfully");
                }
                return BadRequest("Problem with deleting car photo in cloudinary");

            }
            catch (System.Exception)
            {
                return BadRequest("Problem with deleting car in database");
            }
        }
    }
}
