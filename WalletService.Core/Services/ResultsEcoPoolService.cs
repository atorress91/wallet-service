using AutoMapper;
using WalletService.Core.Services.IServices;
using WalletService.Data.Database.Models;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.DTO.ResultsEcoPoolDto;

namespace WalletService.Core.Services;

public class ResultsEcoPoolService:BaseService,IResultsEcoPoolService
{
    private readonly IResultsEcoPoolRepository _resultsEcoPoolRepository;

    public ResultsEcoPoolService(IMapper mapper, IResultsEcoPoolRepository resultsEcoPoolRepository) : base(mapper)
    {
        _resultsEcoPoolRepository = resultsEcoPoolRepository;
    }

    public async Task<IEnumerable<ResultsEcoPoolDto>> GetAllResultsEcoPoolAsync()
    {
        var results = await _resultsEcoPoolRepository.GetAllResultsEcoPool();
        
        return Mapper.Map<IEnumerable<ResultsEcoPoolDto>>(results);
    }
}