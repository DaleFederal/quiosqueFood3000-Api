using Microsoft.AspNetCore.Mvc;
using QuiosqueFood3000.Api.DTOs;
using QuiosqueFood3000.Api.Services.Interfaces;

namespace QuiosqueFood3000.Controllers;

[Route("api/[controller]/")]
[ApiController]
public class RemittanceController(IRemittanceService remittanceService) : ControllerBase
{
    /// <summary>
    /// Retorna uma remessa pelo ID
    /// </summary>
    /// <param name="id">ID da remessa</param>
    /// <returns>Retorna a remessa correspondente ao ID fornecido</returns>
    [HttpGet("getRemittanceById/{id}")]
    public async Task<IActionResult> RemittanceById(int id)
    {
        RemittanceDto? remittanceDto;

        try
        {
            remittanceDto = await remittanceService.GetRemittanceById(id);

            if (remittanceDto is null)
            {
                return NotFound($"Remessa com o id: {id} não encontrado");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(ex.Message);
        }
        return Ok(remittanceDto);
    }

    /// <summary>
    /// Registra o pagamento de uma remessa
    /// </summary>
    /// <param name="id">ID da remessa</param>
    /// <returns>Retorna a remessa com o pagamento registrado</returns>
    // [HttpPost("registerRemittancePayment/")]
    // public async Task<IActionResult> RegisterPayment(int id)
    // {
    //     RemittanceDto? remittanceDto;
    //     try
    //     {
    //         remittanceDto = await remittanceService.RegisterPayment(id);
    //
    //         if (remittanceDto is null)
    //         {
    //             return NotFound("Nenhuma remessa encontrada");
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new InvalidDataException(ex.Message);
    //     }
    //     return Ok(remittanceDto);
    // }

    /// <summary>
    /// Retorna uma remessa pelo ID do pedido
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <returns>Retorna a remessa correspondente ao ID do pedido fornecido</returns>
    [HttpGet("getRemittanceByOrderId/{orderId}")]
    public async Task<IActionResult> RemittanceByOrderId(int orderId)
    {
        RemittanceDto? remittanceDto;

        try
        {
            remittanceDto = await remittanceService.GetRemittanceByOrderId(orderId);

            if (remittanceDto is null)
            {
                return NotFound($"Remessa não encontrada para o pedido com id: {orderId}");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(ex.Message);
        }
        return Ok(remittanceDto);
    }
}