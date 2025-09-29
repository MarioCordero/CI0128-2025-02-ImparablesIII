using backend_lab.Models;
using backend_lab.Repositories;

namespace backend_lab.Services
{
  public class CountryService
  {
    private readonly CountryRepository countryRepository;

    public CountryService()
    {
      countryRepository = new CountryRepository();
    }

    public List<CountryModel> GetCountries()
    {
        return countryRepository.GetCountries();
    }
  }
}