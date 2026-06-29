using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShuttleService.Data;
using ShuttleService.Models;

namespace ShuttleService.Services
{
    public class ParcelRequestIdSequenceService
    {
        private readonly ApplicationDbContext _context;
        public ParcelRequestIdSequenceService(ApplicationDbContext context)
        {
            _context = context;   
        }

        private async Task<int> NextNumber()
        {
            var year = DateTime.Now.Year;
            using (var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable))
            {
                var seq = _context.ParcelRequestIdParameter.Where(x => x.Year == year).FirstOrDefault();
                if(seq == null)
                {
                    seq = new ParcelRequestIdParameter { Year = year, LastNumber = 0 };
                    _context.ParcelRequestIdParameter.Add(seq);
                }

                seq.LastNumber++;
                await _context.SaveChangesAsync();
                tx.Commit();

                return seq.LastNumber;
            };
        }
        public async Task<string> GenerateRequestId()
        {
            var year = DateTime.Now.Year;
            var nextNumber = await NextNumber();
            return $"PDR-{year}-{nextNumber.ToString("D7")}";
        }
    }
}
