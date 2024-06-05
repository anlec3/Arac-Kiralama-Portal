using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FinalUygulama.API.DTOs;
using FinalUygulama.API.Models;

namespace FinalUygulama.API.Controllers
{
    [Route("api/Kiralama")]
    [ApiController]
    [Authorize]
    public class KiralamaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        ResultDto result = new ResultDto();
        public KiralamaController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<List<KiralamaDto>> List(int id)
        {
            var kiralamalar = await _context.Kiralamalar.Where(s => s.ArabaId == id).OrderBy(o => o.Order).ToListAsync();
            var kiralamaDtos = _mapper.Map<List<KiralamaDto>>(kiralamalar);
            return kiralamaDtos;
        }

        [HttpGet()]
        [Route("GetById/{id}")]
        public async Task<KiralamaDto> Get(int id)
        {
            var kiralama = await _context.Kiralamalar.Where(s => s.Id == id).SingleOrDefaultAsync();
            var kiralamaDto = _mapper.Map<KiralamaDto>(kiralama);
            return kiralamaDto;
        }

        [HttpPost]
        public async Task<ResultDto> Add(KiralamaDto dto)
        {
            if (_context.Kiralamalar.Count(c => c.Title == dto.Title && c.ArabaId == dto.ArabaId) > 0)
            {
                result.Status = false;
                result.Message = "Girilen Başlık Kayıtlıdır!";
                return result;
            }

            var order = _context.Kiralamalar.Where(s => s.ArabaId == dto.ArabaId).Count() + 1;

            var Kiralama = _mapper.Map<Kiralama>(dto);

            Kiralama.Created = DateTime.Now;
            Kiralama.Updated = DateTime.Now;
            Kiralama.StartDate = dto.StartDate;
            Kiralama.EndDate = dto.EndDate;
            Kiralama.Order = order;
            await _context.Kiralamalar.AddAsync(Kiralama);
            await _context.SaveChangesAsync();

            //ScoreCalcualte(dto.ArabaId);
            result.Status = true;
            result.Message = "Kayıt Eklendi";
            return result;
        }

        [HttpPut]
        public async Task<ResultDto> Update(KiralamaDto dto)
        {
            var Kiralama = await _context.Kiralamalar.Where(s => s.Id == dto.Id).SingleOrDefaultAsync();
            if (Kiralama == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;
            }

            Kiralama.Title = dto.Title;
            Kiralama.StartDate = dto.StartDate;
            Kiralama.EndDate = dto.EndDate;
            Kiralama.Updated = DateTime.Now;

            _context.Kiralamalar.UpdateRange(Kiralama);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Güncellendi";
            //ScoreCalcualte(dto.ArabaId);
            return result;
        }

        [HttpDelete]
        [Route("id")]
        public async Task<ResultDto> Delete(int id)
        {
            var Kiralama = await _context.Kiralamalar.Where(s => s.Id == id).SingleOrDefaultAsync();
            if (Kiralama == null)
            {
                result.Status = false;
                result.Message = "Kayıt Bulunamadı!";
                return result;
            }

            _context.Kiralamalar.Remove(Kiralama);
            await _context.SaveChangesAsync();
            result.Status = true;
            result.Message = "Kayıt Silindi";
            //ScoreCalcualte(id);
            return result;
        }

        [HttpPost]
        [Route("KiralamaOrderAjax")]
        public ResultDto KiralamaOrderAjax(int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var Kiralama = _context.Kiralamalar.Where(s => s.Id == ids[i]).SingleOrDefault();
                Kiralama.Order = i + 1;
                _context.SaveChanges();
            }

            result.Status = true;
            result.Message = "Sıralandı...";
            return result;
        }

//        private void ScoreCalcualte(int ArabaId)
//        {
//            int totalscore = _context.Kiralamalar.Where(s => s.ArabaId == ArabaId).Sum(x => x.Score);
//            int okscore = _context.Kiralamalar.Where(s => s.ArabaId == ArabaId && s.Status == 2).Sum(x => x.Score);
//            int score = 0;

//            if (okscore > 0 && totalscore > 0)
//            {
//                score = 100 * okscore / totalscore;
//            }

//            var kiralama = _context.Kiralamalar.Where(s => s.Id == ArabaId).FirstOrDefault();
//            kiralama.Score = score;
//            _context.SaveChanges();
//        }
    }
}
