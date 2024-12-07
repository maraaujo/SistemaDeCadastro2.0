﻿using SistemaDeCadastro.Domain.DataTransferObject;
using SistemaDeCadastro.Domain.Models.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeCadastro.APP.Interface
{
    public interface IBloodTypeApp
    {
        Task<List<BloodType>> GetBloodTypeById(long id);
        Task<ApiResponse> GetAllBloodIllness();
        Task<ApiResponse> CreateBloodType(BloodTypeDTO bloodType);
        Task<ApiResponse> DeleteIllness(BloodTypeDTO bloodType);

    }
}