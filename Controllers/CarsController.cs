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


namespace PRAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ICarRepository repo;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IOptions<CloudinarySettings> cloudinarySettings;
        private Cloudinary cloudinary;

        public CarsController(
            ICarRepository repo,
            IMapper mapper,
            IHostingEnvironment hostingEnvironment,
            IOptions<CloudinarySettings> cloudinarySettings)
        {
            this.mapper = mapper;
            this.repo = repo;
            this.hostingEnvironment = hostingEnvironment;
            this.cloudinarySettings = cloudinarySettings;

            Account acc = new Account(
                this.cloudinarySettings.Value.CloudName,
                this.cloudinarySettings.Value.ApiKey,
                this.cloudinarySettings.Value.ApiSecret
            );

            this.cloudinary = new Cloudinary(acc);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateCar([FromForm]CarForCreateDto carForCreateDto)
        {
            var file = Request.Form.Files[0];
            // Accept only .jpg files no bigger than 6 MB
            if
            (
                (file.ContentType.ToLower() != "image/jpg" &&
                file.ContentType.ToLower() != "image/pjpeg" &&
                file.ContentType.ToLower() != "image/jpeg") ||
                (Path.GetExtension(file.FileName).ToLower() != ".jpg" &&
                Path.GetExtension(file.FileName).ToLower() != ".jpeg") ||
                (file.Length > 6291456)
            )
                return BadRequest();

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                        .Height(1000)
                        .Width(1000)
                        .Crop("fill")
                    };

                    uploadResult = this.cloudinary.Upload(uploadParams);
                }
            }

            carForCreateDto.PhotoUrl = uploadResult.Uri.ToString();

            var carForCreate = this.mapper.Map<Car>(carForCreateDto);
            this.repo.CreateCar(carForCreate);

            if (await this.repo.SaveAll())
                return CreatedAtRoute("create", carForCreate);

            return BadRequest();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var carToReturn = await this.repo.GetCar(id);
            return Ok(carToReturn);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCars()
        {
            var carsToReturn = await this.repo.GetAllCars();

            if (carsToReturn != null)
                return Ok(carsToReturn);

            return BadRequest();
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCar()
        {
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var carInRepo = await this.repo.GetCar(id);
            this.repo.DeleteFile(carInRepo);
            this.repo.Delete(carInRepo);

            if (await this.repo.SaveAll())
                return Ok();

            return NoContent();
        }
    }
}
