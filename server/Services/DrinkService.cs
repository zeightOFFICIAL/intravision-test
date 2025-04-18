using AutoMapper;

using server.Models.DTO;
using server.Repositories;

namespace server.Services
{
    public class DrinkService(IDrinkRepository repository, IMapper mapper) : IDrinkService
    {
        private readonly IDrinkRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<DrinkDto>> GetAllDrinksForDisplayAsync()
        {
            var drinks = await _repository.GetAllDrinksAsync();
            return _mapper.Map<IEnumerable<DrinkDto>>(drinks);
        }
    }
}