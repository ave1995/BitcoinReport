using Grid.Data;
using Grid.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Grid.Stores;

public class BitcoinStore : IBitcoinStore
{
    private readonly ApplicationDbContext _dbContext;
    public BitcoinStore(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }

    public async Task<BitcoinDetailModel?> FindAsync(Guid guid)
    {
        return await _dbContext.BitcoinDetails.FindAsync(guid);
    }

    public async Task<BitcoinDetailModel[]> GetAsync()
    {
        return await _dbContext.BitcoinDetails.OrderBy(x => x.Time).ToArrayAsync();
    }

    public async Task<string?> InsertAsync(BitcoinDetailModel model)
    {
        _dbContext.Add(model);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return ex.Message;
        }
        return null;
    }

    public async Task<string?> UpdateAsync(BitcoinDetailModel model)
    {
        _dbContext.Update(model);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return ex.Message;
        }
        return null;
    }

    public async Task<string?> DeleteAsync(BitcoinDetailModel model)
    {
        _dbContext.Remove(model);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return ex.Message;
        }
        return null;
    }
}
