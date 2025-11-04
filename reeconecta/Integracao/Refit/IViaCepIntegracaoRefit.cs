using reeconecta.Integracao.Response;
using Refit;

namespace reeconecta.Integracao.Refit
{
    public interface IViaCepIntegracaoRefit
    {
        [Get("/ws/{cep}/json")]
        Task<ApiResponse<ViaCepResponse>> ObterEnderecoViaCep(string cep);
    }
}
