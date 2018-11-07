# EF Core Sqlite - Setups

## Scaffold
	dotnet ef dbcontext scaffold "Datasource=C:\Horsify\Horsify.db" -o Context Microsoft.EntityFrameworkCore.Sqlite -c "HorsifyContext" -f -d

## Scaffold without data annotations
	dotnet ef dbcontext scaffold -o Context -f "Datasource=C:\ProgramData\Horsify\Horsify.db" "Microsoft.EntityFrameworkCore.Sqlite"


#	Views
### Add views manally to Horsify Context
	public virtual DbSet<AllJoinedTable> AllJoinedTables { get; set; }

	modelBuilder.Entity<AllJoinedTable>(entity =>
    {
        entity.HasKey(e => e.Id);
    });

### Searching in Views - Like syntax
	Use FromSQL for stored procs etc.
		ctx.AllJoinedTables.FromSql($"SELECT * FROM dbo.AllJoinedTables WHERE [{searchTable}] Like '%{searchString}%'")