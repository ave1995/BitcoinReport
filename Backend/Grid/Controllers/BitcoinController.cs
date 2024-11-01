using Grid.Data.Models;
using Grid.Services;
using Grid.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Grid.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BitcoinController : ControllerBase
{

    private readonly IBitcoinStore _bitcoinStore;
    public BitcoinController(IBitcoinStore bitcoinStore)
    {
        _bitcoinStore = bitcoinStore;
    }

    [HttpPost("save-bitcoin")]
    public async Task<IActionResult> SaveBitcoin([FromBody] BitcoinResponse bitcoinResponse)
    {
        if (!TimeOnly.TryParse(bitcoinResponse.Time, out TimeOnly time))
            return BadRequest(new { error = "Invalid time format. Please use HH:mm:ss." });

        var bitcoinDetail = new BitcoinDetailModel()
        {
            Price = bitcoinResponse.Price,
            Time = time
        };

        var result = await _bitcoinStore.InsertAsync(bitcoinDetail);
        if (result is not null)
            return StatusCode(500, "An error occurred while saving the Bitcoin details!");

        return Ok();
    }

    [HttpGet("get-bitcoins")]
    public async Task<IActionResult> GetBitcoins()
    {
        var details = await _bitcoinStore.GetAsync();

        var convertedD = details.Select(d => new BitcoinResponseGet(d.Price, d.Time.ToString("HH:mm:ss"), d.Note ?? string.Empty, d.Guid));

        return Ok(details);
    }

    [HttpPatch("update-note/{guid}")]
    public async Task<IActionResult> UpdateBitcoin(Guid guid, [FromBody] BitcoinUpdateNoteRequest request)
    {
        var bitcoinDetail = await _bitcoinStore.FindAsync(guid);
        if (bitcoinDetail is null)
            return NotFound("Bitcoin detail not found!");

        bitcoinDetail.Note = request.Note;

        var result = await _bitcoinStore.UpdateAsync(bitcoinDetail);
        if (result is not null)
            return StatusCode(500, "An error occurred while updating the Bitcoin details!");

        return Ok();
    }

    [HttpDelete("delete-bitcoin/{guid}")]
    public async Task<IActionResult> DeleteBitcoin(Guid guid)
    {
        var bitcoinDetail = await _bitcoinStore.FindAsync(guid);
        if (bitcoinDetail is null)
            return NotFound("Bitcoin detail not found!");

        var result = await _bitcoinStore.DeleteAsync(bitcoinDetail);
        if (result is not null)
            return StatusCode(500, "An error occurred while deleting the Bitcoin details!");

        return Ok();
    }
}
