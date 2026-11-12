using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.APP
{
    public class PaymentApp : IPaymentApp
    {
        private readonly IPaymentRepository _repo;
        private readonly IAppointmentRepository _appointmentRepository;

        /// <summary>
        /// Recebe por injecao de dependencia os repositorios de pagamento e de agendamento.
        /// </summary>
        public PaymentApp(IPaymentRepository repo, IAppointmentRepository appointmentRepository)
        {
            _repo = repo;
            _appointmentRepository = appointmentRepository;
        }

        /// <summary>
        /// Devolve todos os pagamentos visiveis para a instituicao do usuario.
        /// </summary>
        public async Task<List<Payment>> GetAll() => await _repo.GetAllReadOnly();

        /// <summary>
        /// Busca um pagamento pelo Id; devolve null se nao existir.
        /// </summary>
        public async Task<Payment?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

        /// <summary>
        /// Monta um pagamento a partir do DTO e o devolve em Data com Success = true.
        /// Observacao: o pagamento NAO e gravado no banco (nao chama o repositorio) e o agendamento nao e validado.
        /// </summary>
        public async Task<ApiResponse> Create(CreatePaymentDTO entity)
        {
            var ret = new ApiResponse();
            try {
            var newPayment = new Payment
            {
                AppointmentId = entity.AppointmentId,
                UserId = entity.UserId,
                TotalAmount = entity.TotalAmount,
                PaymentMethod = entity.PaymentMethod,
                PaymentDate = entity.PaymentDate,
                Status = entity.Status,
                ExternalTransactionId = entity.ExternalTransactionId
            };
            ret.Data = newPayment;
            ret.Success = true;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Atualiza os dados de um pagamento existente ("Payment not found." se nao existir).
        /// Se o agendamento mudar, exige que o novo agendamento seja da instituicao do usuario.
        /// </summary>
        public async Task<ApiResponse> Update(UpdatePaymentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingPayment = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existingPayment != null)
                {
                    // O agendamento vem do cliente: precisa ser da instituicao do usuario.
                    if (entity.AppointmentId != existingPayment.AppointmentId &&
                        !await _appointmentRepository.Any(a => a.Id == entity.AppointmentId))
                    {
                        ret.Success = false;
                        ret.ErrorMessage = "Agendamento não encontrado para a instituição do usuário logado.";
                        return ret;
                    }

                    existingPayment.AppointmentId = entity.AppointmentId;
                    existingPayment.UserId = entity.UserId;
                    existingPayment.TotalAmount = entity.TotalAmount;
                    existingPayment.PaymentMethod = entity.PaymentMethod;
                    existingPayment.PaymentDate = entity.PaymentDate;
                    existingPayment.Status = entity.Status;
                    existingPayment.ExternalTransactionId = entity.ExternalTransactionId;
                    await _repo.Update(existingPayment);
                    ret.Success = true;
                }
                else
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Pagamento não encontrado.";
                }

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ApiErrors.From(ex);
            }
            return ret;
        }

        /// <summary>
        /// Exclui o pagamento com o Id informado. Retorna Success = true mesmo se o Id nao existir.
        /// </summary>
        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ApiErrors.From(ex); }
            return ret;
        }
    }
}
