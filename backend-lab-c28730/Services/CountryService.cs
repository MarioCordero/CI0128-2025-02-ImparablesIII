using backend_lab_c28730.Models;
using backend_lab_c28730.Repositories;

namespace backend_lab_c28730.Services
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