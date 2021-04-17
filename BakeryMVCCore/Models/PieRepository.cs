using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BakeryMVCCore.Models
{
    public class PieRepository : IPieRepository
    {
        private readonly BakeryDBContext _ctx;

        public PieRepository(BakeryDBContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Pie> AllPies
        {
            get
            {
                return _ctx.Pies.Include(p => p.Category);
            }
        }

        public IEnumerable<Pie> PiesOfTheWeek
        {
            get
            {
                return _ctx.Pies.Include(p => p.Category).Where(p => p.IsPieOfTheWeek);
            }
        }

        public Pie GetPieById(int pieId)
        {
            var result = _ctx.Pies.Include(p => p.Category).FirstOrDefault(p => p.PieId == pieId);
            return result;
        }
    }
}
