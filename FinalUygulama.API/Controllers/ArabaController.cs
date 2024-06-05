using AutoMapper;
using FinalUygulama.API.DTOs;
using FinalUygulama.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalUygulama.API.Controllers
{
    [Route("api/Araba")]
    [ApiController]
    [Authorize]
    public class ArabaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        ResultDto result = new ResultDto();
        public ArabaController(AppDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<List<ArabaDto>> List()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = User.FindAll(ClaimTypes.Role);
            var arabalar = new List<Araba>();

            //if (roles.Any(c => c.Value == "Admin"))
            //{
                arabalar = await _context.Arabalar.OrderBy(o => o.Order).ToListAsync();
            //}
            //else
            //{
            //    arabalar = await _context.Arabalar.Where(s => s.AppUserId == userId).OrderBy(o => o.Order).ToListAsync();
            //}

            var arabalarDto = _mapper.Map<List<ArabaDto>>(arabalar);
            return arabalarDto;
        }

        [HttpGet("{id}")]
        public async Task<ArabaDto> Get(int id)
        {
            var arabalar = await _context.Arabalar.Where(s => s.Id == id).SingleOrDefaultAsync();
            var arabaDto = _mapper.Map<ArabaDto>(arabalar);
            return arabaDto;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Route("Add")]
        public async Task<ResultDto> Add(ArabaDto dto)
        {
            if (_context.Arabalar.Count(c => c.Id == dto.Id) > 0)
            {
                result.Status = false;
                result.Message = "Girilen Araç Kayıtlıdır!";
                return result;
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = _context.Arabalar.Where(s => s.AppUserId == userId).Count() + 1;

            var araba = _mapper.Map<Araba>(dto);
            araba.AppUserId = userId;
            araba.Created = DateTime.Now;
            araba.Updated = DateTime.Now;
            araba.Order = order;
            await _context.Arabalar.AddAsync(araba);
            await _context.SaveChangesAsync();

            result.Status = true;
            result.Message = "Kayıt Eklendi";
            return result;
        }

        [HttpPut]
        public async Task<ResultDto> Update(ArabaDto dto)
        {
            var araba = await _context.Arabalar.Where(s => s.Id == dto.Id).SingleOrDefaultAsync();
            if (araba == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;
            }
            araba.Marka = dto.Marka;
            araba.Model = dto.Model;
            araba.GunlukUcret = dto.GunlukUcret;
            araba.Order = dto.Order;
            araba.Resim = dto.Resim;
            araba.Updated = DateTime.Now;

            _context.Arabalar.UpdateRange(araba);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Güncellendi";
            return result;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<ResultDto> Upload(ArabaDto dto)
        {
            var araba = await _context.Arabalar.Where(s => s.Id == dto.Id).SingleOrDefaultAsync();
            if (araba == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;
            }

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/Cars");
            string ArabaResim = araba.Resim;

            if (ArabaResim != "defaultcar.png")
            {

                var ArabaResimUrl = Path.Combine(path, ArabaResim);

                if (System.IO.File.Exists(ArabaResimUrl))
                {
                    System.IO.File.Delete(ArabaResimUrl);
                }
            }
            string data = dto.Resim;
            string base64 = data.Substring(data.IndexOf(',') + 1);
            base64 = base64.Trim('\0');
            byte[] imageBytes = Convert.FromBase64String(base64);
            string filePath = Guid.NewGuid().ToString() + dto.PicExt;


            var picPath = Path.Combine(path, filePath);

            System.IO.File.WriteAllBytes(picPath, imageBytes);

            araba.Resim = filePath;
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Resim Güncellendi";
            return result;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultDto> Delete(int id)
        {
            var araba = await _context.Arabalar.Where(s => s.Id == id).SingleOrDefaultAsync();
            if (araba == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;
            }
            if (_context.Kiralamalar.Count(c => c.ArabaId == id) > 0)
            {
                result.Status = false;
                result.Message = "İşlem Kaydı Mevcuttur Silinemez!";
                return result;
            }

            _context.Arabalar.Remove(araba);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Silindi";
            return result;
        }

        [HttpPost]
        [Route("ArabaOrderAjax")]
        public ResultDto ArabaOrderAjax(int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var araba = _context.Arabalar.Where(s => s.Id == ids[i]).SingleOrDefault();
                araba.Order = i + 1;
                _context.SaveChanges();

            }
            result.Status = true;
            result.Message = "Sıralandı...";
            return result;
        }
    }
}
