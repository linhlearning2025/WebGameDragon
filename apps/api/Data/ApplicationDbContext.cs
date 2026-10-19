using Microsoft.EntityFrameworkCore;

namespace DragonEnglishKids.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options);