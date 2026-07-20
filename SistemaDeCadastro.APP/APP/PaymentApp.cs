using SistemaDeCadastro.APP.Interface;
using SistemaDeCadastro.Domain.Models.Stage;
using SistemaDeCadastro.Infra.Interface;
using SistemaDeCadastro.Domain.DataTransferObject;
namespace SistemaDeCadastro.APP.APP
{
    public class PaymentApp : IPaymentApp
    {
        private readonly IPaymentRepository _repo;

        public PaymentApp(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Payment>> GetAll() => await _repo.GetAll();

        public async Task<Payment?> GetById(long id) => (await _repo.FindBy(p => p.Id == id)).FirstOrDefault();

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
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Update(UpdatePaymentDTO entity)
        {
            var ret = new ApiResponse();
            try
            {
                var existingPayment = (await _repo.FindBy(p => p.Id == entity.Id)).FirstOrDefault();
                if (existingPayment != null)
                {
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
                    ret.ErrorMessage = "Payment not found.";
                }

            }
            catch (Exception ex)
            {
                ret.Success = false;
                ret.ErrorMessage = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResponse> Delete(long id)
        {
            var ret = new ApiResponse();
            try { var e = (await _repo.FindBy(p => p.Id == id)).FirstOrDefault(); if (e != null) await _repo.Delete(e); ret.Success = true; } catch (Exception ex) { ret.Success = false; ret.ErrorMessage = ex.Message; }
            return ret;
        }
    }
}
