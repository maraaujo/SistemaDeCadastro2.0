using Microsoft.EntityFrameworkCore;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Domain.SistemaCadastroContext;
using SistemaDeCadastro.Infra.Interface;

namespace SistemaDeCadastro.Infra.Repository
{
    public class SubscriptionPaymentRepository : BaseRepository<SubscriptionPayment>, ISubscriptionPaymentRepository
    {
        private readonly SistemaDeCadastroContext _context;
        /// <summary>Recebe o contexto do banco; todas as operacoes vem do BaseRepository (sem metodos proprios).</summary>
        public SubscriptionPaymentRepository(SistemaDeCadastroContext context) : base(context)
        {
            _context = context;
        }
    }
}
