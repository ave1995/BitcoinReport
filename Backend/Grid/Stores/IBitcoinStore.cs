using Grid.Data.Models;

namespace Grid.Stores;

public interface IBitcoinStore
{
    Task<string?> InsertAsync(BitcoinDetailModel model);

    Task<BitcoinDetailModel[]> GetAsync();

    Task<BitcoinDetailModel?> FindAsync(Guid guid);

    Task<string?> UpdateAsync(BitcoinDetailModel model);

    Task<string?> DeleteAsync(BitcoinDetailModel model);
}