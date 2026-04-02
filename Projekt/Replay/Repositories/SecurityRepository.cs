using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Models;

namespace Replay.Repositories;
/// <author>Daniel Feustel</author>
public class SecurityRepository {


    private readonly SecurityDbContext _dbContext;

    public  SecurityRepository(SecurityDbContext ctx) {
        _dbContext = ctx;
    }

    public async void Create(Guid id, string email) {
        SecurityModel model = new (){Id = id, Email = email};
        await _dbContext.SecurityDB.AddAsync(model);
    }
    public async void Create(SecurityModel model) {
        SecurityModel securityModel = new (){Id = model.Id, Email = model.Email};
        await _dbContext.SecurityDB.AddAsync(securityModel);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<SecurityModel> Read(Guid id) {
        return  await _dbContext.SecurityDB.SingleAsync(l => l.Id == id);
    }
    public async Task<SecurityModel> Read(string email) {
        return  await _dbContext.SecurityDB.SingleAsync(l => l.Email == email);
    }
    public async void Delete(string email) {
        var entry = await Read(email);
        
        _dbContext.Entry(entry).State = EntityState.Detached;

        _dbContext.SecurityDB.Attach(entry);
        

        
        _dbContext.SecurityDB.Remove(entry);
        await _dbContext.SaveChangesAsync();
       

        
    }
    
}