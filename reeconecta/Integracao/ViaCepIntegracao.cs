using reeconecta.Integracao.Interfaces;
using reeconecta.Integracao.Refit;
using reeconecta.Integracao.Response;

namespace reeconecta.Integracao
{
    public class ViaCepIntegracao : IViaCepIntegracao
    {
        private readonly IViaCepIntegracaoRefit _viaCepIntegracaoRefit;
        public ViaCepIntegracao(IViaCepIntegracaoRefit viaCepIntegracaoRefit)
        {
            _viaCepIntegracaoRefit = viaCepIntegracaoRefit;
        }

        public async Task<ViaCepResponse> ObterEnderecoViaCep(string cep)
        {
            var responseData = await _viaCepIntegracaoRefit.ObterEnderecoViaCep(cep);

            if(responseData != null && responseData.IsSuccessStatusCode)
            {
                return responseData.Content;
            }
            return null;
        }
    }
}
