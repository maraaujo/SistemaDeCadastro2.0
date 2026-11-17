namespace SistemaDeCadastro.Domain.DataTransferObject
{
    /// <summary>
    /// Quantidade de registros por tabela nas operacoes de manutencao do administrador da plataforma.
    /// Usado tanto para mostrar o que existe hoje (resumo) quanto o que foi apagado por uma limpeza.
    /// Os numeros sao globais: somam todas as instituicoes.
    /// </summary>
    public class MaintenanceCountsDTO
    {
        /// <summary>Acolhidos.</summary>
        public int Patients { get; set; }

        /// <summary>Responsaveis (familiares/contatos dos acolhidos).</summary>
        public int Responsibles { get; set; }

        /// <summary>Condicoes clinicas vinculadas a acolhidos.</summary>
        public int PatientClinicalConditions { get; set; }

        /// <summary>Medicamentos programados (prescricoes).</summary>
        public int ScheduledMedicines { get; set; }

        /// <summary>Administracoes de medicamento (historico de doses).</summary>
        public int MedicationAdministrations { get; set; }

        /// <summary>Agendamentos.</summary>
        public int Appointments { get; set; }

        /// <summary>Pagamentos ligados a agendamentos.</summary>
        public int Payments { get; set; }

        /// <summary>Vinculos acolhido-funcionario.</summary>
        public int PatientEmployees { get; set; }

        /// <summary>Funcionarios.</summary>
        public int Employees { get; set; }

        /// <summary>Departamentos.</summary>
        public int Departments { get; set; }

        /// <summary>Medicamentos (cadastro de cada instituicao).</summary>
        public int Medicines { get; set; }

        /// <summary>Soma de todos os contadores.</summary>
        public int Total =>
            Patients + Responsibles + PatientClinicalConditions + ScheduledMedicines +
            MedicationAdministrations + Appointments + Payments + PatientEmployees +
            Employees + Departments + Medicines;
    }
}
