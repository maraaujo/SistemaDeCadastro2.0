using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using System.Linq;

namespace SistemaDeCadastro.APP.APP
{
    public class SubscriptionPaymentApp : ISubscriptionPaymentApp
    {
        private readonly ISubscriptionPaymentRepository _repo;

        public SubscriptionPaymentApp(ISubscriptionPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse> Create(CreateSubscriptionPaymentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var payment = new SubscriptionPayment
                {
                    SubscriptionId = entity.SubscriptionId,
                    Amount = entity.Amount,
                    PaymentMethod = entity.PaymentMethod,
                    Status = entity.Status,
                    PaymentDate = entity.PaymentDate,
                    ExternalPaymentId = entity.ExternalPaymentId,
                    Observations = entity.Observations
                };

                await _repo.Create(payment);
                ret.Success = true;
                ret.Message = "Pagamento de assinatura criado com sucesso.";
                ret.Data = payment;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdateSubscriptionPaymentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Pagamento não encontrado.";
                    return ret;
                }

                existing.SubscriptionId = entity.SubscriptionId;
                existing.Amount = entity.Amount;
                existing.PaymentMethod = entity.PaymentMethod;
                existing.Status = entity.Status;
                existing.PaymentDate = entity.PaymentDate;
                existing.ExternalPaymentId = entity.ExternalPaymentId;
                existing.Observations = entity.Observations;

                await _repo.Update(existing);
                ret.Success = true;
                ret.Message = "Pagamento atualizado com sucesso.";
                ret.Data = existing;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var existing = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();
                if (existing == null)
                {
                    ret.Success = false;
                    ret.ErrorMessage = "Pagamento não encontrado.";
                    return ret;
                }

                await _repo.Delete(existing);
                ret.Success = true;
                ret.Message = "Pagamento removido com sucesso.";
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> GetById(long id)
        {
            var ret = new ApiResponse();
            try
            {
                var item = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();
                ret.Success = true;
                ret.Data = item;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> GetAll()
        {
            var ret = new ApiResponse();
            try
            {
                var items = await _repo.GetAll();
                ret.Success = true;
                ret.Data = items;
            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            return ret;
        }
    }
}
