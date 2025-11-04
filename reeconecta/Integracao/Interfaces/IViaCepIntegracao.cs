using reeconecta.Integracao.Response;

namespace reeconecta.Integracao.Interfaces
{
    public interface IViaCepIntegracao
    {
        Task<ViaCepResponse> ObterEnderecoViaCep(string cep);
    }
}
