using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.Pageds;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaDeCadastro.Infra.Repository
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        private readonly SistemaDeCadastroContext _context;

        /// <summary>Recebe o contexto do banco por injecao de dependencia e repassa ao BaseRepository.</summary>
        public PatientRepository(SistemaDeCadastroContext context)
            : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Monta a ficha completa do acolhido. Faz uma unica consulta com left joins de Patients com
        /// Responsibles, PatientClinicalConditions, Appointments, BloodTypes, ClinicalConditions,
        /// MedicinePatientClinicalConditions e Medicines; isso gera uma linha por combinacao, entao em
        /// memoria o metodo junta as listas e remove duplicados por id (descartando itens com id 0, que
        /// representam "nao informado"). Devolve null se o acolhido nao existir (ou for de outra instituicao).
        /// Erros sao relancados com a mensagem "Erro ao buscar detalhes do paciente".
        /// </summary>
        public async Task<DetailsPatientDTO?> DetailsPatient(long id)
        {
            try
            {
                var details = await (from pa in _context.Patients.AsNoTracking()
                                     join re in _context.Responsibles.AsNoTracking()
                                         on pa.Id equals re.PatientId into responsibleGroup
                                     from re in responsibleGroup.DefaultIfEmpty()
                                     join pcc in _context.PatientClinicalConditions.AsNoTracking()
                                         on pa.Id equals pcc.PatientId into pccGroup
                                     from pcc in pccGroup.DefaultIfEmpty()
                                     join ap in _context.Appointments.AsNoTracking()
                                         on pa.Id equals ap.PatientId into appGroup
                                     from ap in appGroup.DefaultIfEmpty()         
                                     join b in _context.BloodTypes.AsNoTracking()
                                         on pa.BloodTypeId equals b.Id into bGroup
                                     from b in bGroup.DefaultIfEmpty()                           
                                     join cc in _context.ClinicalConditions.AsNoTracking()
                                         on pcc.ClinicalConditionId equals cc.Id into ccGroup
                                     from cc in ccGroup.DefaultIfEmpty()
                                     join mpcc in _context.MedicinePatientClinicalConditions.AsNoTracking()
                                         on pcc.Id equals mpcc.PatientClinicalConditionId into mpccGroup
                                     from mpcc in mpccGroup.DefaultIfEmpty()
                                     join med in _context.Medicines.AsNoTracking()
                                         on mpcc.MedicineId equals med.Id into medGroup
                                     from med in medGroup.DefaultIfEmpty()
                                     where pa.Id == id
                                     
                                     select new DetailsPatientDTO
                                     {
                                         Patient = new PatientDTO
                                         {
                                             Id = pa.Id,
                                             Name = pa.Name,
                                
                                             Cpf = pa.Cpf,
                                             Observations = pa.Observations,
                                             BirthDate = pa.BirthDate,
                                             Gender = pa.Gender
                                         },
                                         BloodType = new BloodTypeDTO
                                         {
                                             Id = b != null ? b.Id : 0,
                                             Name = b != null ? b.Name : "tipo sanguíneo não informado",
                                         },
                                          // Responsáveis: se não existir, retorna lista vazia
                                          Responsibles = re != null
                                              ? new List<ResponsibleListDTO>
                                              {
                                                  new ResponsibleListDTO
                                                  {
                                                      Id = re.Id,
                                                      Name = re.Name,
                                                      Phone = re.Phone,
                                                      Relationship = re.Relationship,
                                                      Address = re.Address
                                                  }
                                              }
                                              : new List<ResponsibleListDTO>(),

                             
                                       
                                         ClinicalConditions = new List<ClinicalConditionDTO>
                                         {
                                             new ClinicalConditionDTO
                                             {
                                                 Id = cc != null ? cc.Id : 0,
                                                 Name = cc != null ? cc.Name : "condição clínica não informada",
                                                 Type = cc != null ? cc.Type : "tipo não informado",
                                                 Description = cc != null ? cc.Description : "descrição não informada"
                                             }
                                         },
                                         Medicines = new List<MedicineDTO>
                                         {
                                             new MedicineDTO
                                             {
                                                 Id = med != null ? med.Id : 0,
                                                 Name = med != null ? med.Name : "medicamento não informado",
                                                 Description = med != null ? med.Description : "descrição não informada",
                                                 Dosage = mpcc != null ? mpcc.PrescribedDosage : "dosagem não informada",
                                                 AdministrationRoute = med != null ? med.AdministrationRoute : "a",
                                                 StartDate = mpcc != null ? mpcc.StartDate : null,
                                                 EndDate = mpcc != null ? mpcc.EndDate : null
                                             }
                                         },
                                      
                                         Appointments = ap != null ? new List<AppointmentListDTO>
                                         {
                                             new AppointmentListDTO
                                             {
                                                 Id = ap != null ? ap.Id : 0,
                                                 Status = ap != null ? ap.Status : "status não informado",
                                                 // use nullable FK InstitutionId to avoid accessing a null navigation property
                                                 Institution = ap != null ? (ap.InstitutionId ?? 0) : 0,
                                                 DateTime = ap != null ? ap.DateTime : DateTime.MinValue,
                                                 Observations = ap != null ? ap.Observations : "observações não informadas",
                                             }
                                         } : new List<AppointmentListDTO>()
                                         //esse ou (:) é para caso não tenha nenhum agendamento,
                                         //ele vai retornar uma lista vazia, ao invés de retornar null
                                     }).ToListAsync(); // <- materialize the query

                if (!details.Any())
                {
                    return null;
                }
                var result = new DetailsPatientDTO
                {
                    //pega o primeiro paciente da lista, que é o único paciente que vai ser retornado
                    Patient = details.First().Patient,
                    BloodType = details.First().BloodType,
                    //aqui ele vai pegar todos os responsáveis, condições clínicas, medicamentos e agendamentos
                    //porem, ele vai agrupar por id
                    //.Select(g => g.First()) pega o primeiro elemento de cada grupo, ou seja, ele vai eliminar os duplicados


                    Responsibles = details
                    .SelectMany(d => d.Responsibles ?? Enumerable.Empty<ResponsibleListDTO>())
                    .Where(r => r.Id != 0)
                    .GroupBy(r => r.Id)
                    .Select(g => g.First())
                    .ToList(),

                      ClinicalConditions = details
                    .SelectMany(d => d.ClinicalConditions)
                    .Where(c => c.Id != 0)
                    .GroupBy(c => c.Id)
                    .Select(g => g.First())
                    .ToList(),

                      Medicines = details
                    .SelectMany(d => d.Medicines)
                    .Where(m => m.Id != 0)
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    .ToList(),

                     Appointments = details
                    .SelectMany(d => d.Appointments)
                    .Where(a => a.Id != 0)
                    .GroupBy(a => a.Id)
                    .Select(g => g.First())
                    .ToList()
                            };

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar detalhes do paciente. Detalhes: " + ex.Message, ex);
            }
        }

        
        /// <summary>
        /// Lista paginada de acolhidos da instituicao logada. Filtros opcionais: lista de ids de condicoes
        /// clinicas (acolhido que tenha pelo menos uma delas) e id do acolhido. Devolve id, nome, nascimento,
        /// sexo e CPF, ordenando do maior id para o menor, 15 por pagina.
        /// </summary>
        public async Task<PagedPatientDTO> FilterPatient(PatientFilterDTO filter)
        {
            var page = filter.Page <= 0 ? 1 : filter.Page;


            var query =  _context.Patients.AsNoTracking();
                
           
            if(filter.ClinicalConditionIds != null && filter.ClinicalConditionIds.Any())
            {
                //aqui ele vai filtrar os pacientes que possuem alguma condição clínica que esteja na lista de ids de condições clínicas
                //sem precisar fazer join com a tabela de condições clínicas, pois ele vai usar a relação entre paciente e condição clínica
                query = query.Where(p => p.PatientClinicalConditions.Any(c => filter.ClinicalConditionIds.Contains(c.ClinicalConditionId)));
            }

            if (filter.PatientId.HasValue)
            {
                query = query.Where(mp => mp.Id == filter.PatientId.Value);
            }



            var ret = new PagedPatientDTO();

            ret.Page = page;

            ret.Count = await query.CountAsync();

            ret.TotalPages = ret.Count % ret.ItensPerPage > 0
                ? (ret.Count / ret.ItensPerPage) + 1
                : ret.Count / ret.ItensPerPage;

            ret.Patients = await query
                .Select(c => new PatientListDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    Gender = c.Gender,
                    Cpf = c.Cpf,
                })
                .Where(c => c.Id != 0)
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * ret.ItensPerPage)
                .Take(ret.ItensPerPage)
                .ToListAsync();

            return ret;
        }
        
        /// <summary>
        /// Procura um acolhido com o CPF exato dentro da instituicao informada e devolve o primeiro
        /// (ou null). Usado no cadastro para impedir CPF repetido na mesma instituicao.
        /// </summary>
        public async Task<Patient>? FindPatientByCPF(string cpf, long institutionId)
        {
            var existingPatient = await FindBy(c => c.Cpf == cpf && c.InstitutionId == institutionId);
            return existingPatient.FirstOrDefault();

        }

        /// <summary>Busca o acolhido pelo id; devolve lista com 0 ou 1 item (somente da instituicao logada).</summary>
        public async Task<List<Patient>> GetPatientById(long id)
        {
            return await FindBy(c => c.Id == id);
        }

        /// <summary>
        /// Busca a entidade do acolhido pelo id carregando tipo sanguineo, responsaveis, condicoes clinicas
        /// (com a condicao), funcionarios vinculados (com o funcionario), agendamentos e atendimentos.
        /// </summary>
        public async Task<Patient?> GetByIdWithRelations(long id)
        {
            return await _context.Patients
                .Include(p => p.BloodType)
                .Include(p => p.Responsibles)
                .Include(p => p.PatientClinicalConditions)
                    .ThenInclude(pcc => pcc.ClinicalCondition)
                .Include(p => p.PatientEmployees)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.Appointments)
                .Include(p => p.CareServices)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        //vou ter que fazer um job para ficar chamando esse metodo de tempos
        //em tempos para ficar atualizando a tela de lembrete de medicamentos

        

        /// <summary>Cadastra um acolhido (atalho para Create do BaseRepository).</summary>
        public async Task CreatePatient(Patient patient)
        {
            await Create(patient);
        }

        /// <summary>Atualiza um acolhido (atalho para Update do BaseRepository).</summary>
        public async Task UpdatePatient(Patient patient)
        {
            await Update(patient);
        }

        /// <summary>Exclui um acolhido (atalho para Delete do BaseRepository).</summary>
        public async Task DeletePatient(Patient patient)
        {
            await Delete(patient);
        }

        /// <summary>
        /// Consulta se existe acolhido com o nome exato informado, mas descarta o resultado:
        /// o metodo retorna Task sem valor, entao quem chama nao fica sabendo se existe.
        /// </summary>
        public async Task GetPatientByAny(string patient)
        {
            await Any(c => c.Name == patient);
        }
    }
}