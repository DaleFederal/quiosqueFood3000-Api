using Microsoft.EntityFrameworkCore;
using QuiosqueFood3000.Domain.Entities;
using QuiosqueFood3000.Infraestructure.Persistence;
using QuiosqueFood3000.Infraestructure.Repositories.Interfaces;

namespace QuiosqueFood3000.Infraestructure.Repositories;

public class RemittanceRepository(ApplicationDbContext context) : IRemittanceRepository
{
    public async Task<Remittance?> GetRemittancebyId(int id) => await context.Remittance.Include(o => o.Order).FirstOrDefaultAsync(r => r.Id == id);
    public async Task<Remittance?> GetRemittancebyOrderId(int orderId) => await context.Remittance
        .Include(o => o.Order)
        .FirstOrDefaultAsync(r => r.Order != null && r.Order.Id == orderId);
    public Remittance RegisterRemittance(Remittance remittance)
    {
        if (remittance.Order != null)
        {
            var existingOrder = context.Order.Find(remittance.Order.Id);
            if (existingOrder != null)
            {
                context.Entry(existingOrder).State = EntityState.Detached;
                context.Order.Attach(remittance.Order);
            }
        }

        context.Remittance.Add(remittance);
        context.SaveChanges();
        return remittance;
    }
    public void RemoveRemittance(Remittance remittance)
    {
        var existingRemittance = context.Remittance.Find(remittance.Id);
        if (existingRemittance != null)
        {
            context.Entry(existingRemittance).State = EntityState.Detached;

            context.Remittance.Remove(remittance);
            context.SaveChanges();
        }

    }
    public Remittance UpdateRemittance(Remittance remittance)
    {
        var existingRemittance = context.Remittance.Find(remittance.Id);
        if (existingRemittance != null)
        {
            context.Entry(existingRemittance).State = EntityState.Detached;

            context.Remittance.Update(remittance);
            context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("Remessa não encontrada para realizar alteração");
        }
        return remittance;
    }
}
