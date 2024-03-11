using HomeWorkOTUS.Infrastructure.Services;
using HomeWorkOTUS.Models.Clients;
using HomeWorkOTUS.Models.Technical;
using RestSharp;

namespace HomeWorkOTUS.Services
{
    public class TechnicalService : ITechnicalService
    {
        private readonly RestClient _restClient;
        private readonly IClientsService _clientsService;
        private readonly Random _rnd;

        private static string[] Interests = ["Плавание", "Чтение", "Музыка", "Велосипед", "Машины", "Путешествия", "Марки"];

        public TechnicalService(IHttpClientFactory httpClientFactory, 
            IClientsService clientsService)
        {
            var httpClient = httpClientFactory.CreateClient("restclient");
            _restClient = new RestClient(httpClient);
            _clientsService = clientsService;
            _rnd = new Random();
        }

        public async Task<GenerateUsersResponse> GenerateUsersAsync(GenerateUsersRequest request)
        {
            GenerateUsersResponse response = new GenerateUsersResponse();

            var restRequest = new RestRequest("https://raw.githubusercontent.com/OtusTeam/highload/master/homework/people.v2.csv", Method.Get);
            var data = await _restClient.ExecuteAsync(restRequest);
            var rows = data.Content.Split("\n");
            int i = 0;
            while (i < rows.Length && i < request.CountUsers)
            {
                try
                {
                    var info = rows[i].Split(',');
                    var registrationRequest = new RegistrationRequest();
                    registrationRequest.Name = info[0].Split(' ')[1];
                    registrationRequest.SerName = info[0].Split(' ')[0];
                    registrationRequest.Age = _rnd.Next(1, 100);
                    registrationRequest.IsMale = _rnd.Next(0, 1) == 1 ? true : false;
                    registrationRequest.City = info[2];
                    registrationRequest.Interests = Interests[_rnd.Next(0, 7)];
                    registrationRequest.Password = _rnd.Next(100000, 1000000).ToString();
                    await _clientsService.RegisterAsync(registrationRequest);
                    response.SuccessCount++;
                }
                catch (Exception ex)
                {
                    response.ErrorsCount++;
                    if (response.Top10Errors.Count < 10 && !response.Top10Errors.Contains(ex.Message))
                        response.Top10Errors.Add(ex.Message);
                    break;
                }
                finally
                {
                    i++;
                }
            }
            return response;
        }
    }
}
