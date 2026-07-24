using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Filters;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.Infra.Repository
{
    public class MedicineRepository : BaseRepository<Medicine>, IMedicineRepository
    {
        private readonly SistemaDeCadastroContext _context;
        public MedicineRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Medicine>> GetMedicineById(long ind)
        {
            return await this.FindBy( c => c.Id == ind );  
        }
        public async Task GetMedicineByAnyValorString(string medicine)
        {
            await this.Any( c => c.Name == medicine );
           
        }

        public async Task CreateMedicine(Medicine medicine)
        {
            await this.Create(medicine);
        }

        public async Task UpdateMedicine(Medicine medicine)
        {
            await this.Update(medicine);
        }
        public async Task DeleteMedicine(Medicine medicine)
        {
            await this.Delete(medicine);
        }
        public async Task<PagedMedicineDTO> GetMedicineByFilter(MedicineFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;
            var query = from m in _context.Medicines
                        select new MedicineListDTO
                        {
                            Id = m.Id,
                            Name = m.Name,
                            Description = m.Description,
                            Dosage = m.Dosage,
                            AdministrationRoute = m.AdministrationRoute,
                        };
            if (filter.Id.HasValue)
            {
                query = query.Where(c => c.Id == filter.Id.Value);
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(m => m.Name.Contains(filter.Name));
            }
            if(!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(m => m.Description.Contains(filter.Description));
            }
            if(!string.IsNullOrEmpty(filter.Dosage))
            {
                query = query.Where(m => m.Dosage.Contains(filter.Dosage));
            }
            if(!string.IsNullOrEmpty(filter.AdministrationRoute))
            {
                query = query.Where(m => m.AdministrationRoute.Contains(filter.AdministrationRoute));
            }
            var ret = new PagedMedicineDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Medicines = await query
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
    }
}
