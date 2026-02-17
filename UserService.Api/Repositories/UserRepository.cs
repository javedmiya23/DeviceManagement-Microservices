using Microsoft.Data.SqlClient;
using UserService.Api.Models.Entities;

namespace UserService.Api.Repositories;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task AddUserAsync(User user)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"INSERT INTO Users (UserId, PasswordHash, Name, Permissions) 
                      VALUES (@UserId, @PasswordHash, @Name, @Permissions)";

        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = user.UserId;
        cmd.Parameters.Add("@PasswordHash", System.Data.SqlDbType.NVarChar).Value = user.PasswordHash;
        cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = user.Name;
        cmd.Parameters.Add("@Permissions", System.Data.SqlDbType.NVarChar).Value = user.Permissions;

        try
        {
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex) when (ex.Number == 2627) 
        {
            throw new InvalidOperationException("UserId already exists.");
        }
    }

    public async Task UpdateUserAsync(string userId, string name, string permissions)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"UPDATE Users 
                      SET Name=@Name, Permissions=@Permissions 
                      WHERE UserId=@UserId";

        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = userId;
        cmd.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = name;
        cmd.Parameters.Add("@Permissions", System.Data.SqlDbType.NVarChar).Value = permissions;

        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0)
            throw new KeyNotFoundException("User not found.");
    }

   
    public async Task DeleteUserAsync(string userId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"DELETE FROM Users WHERE UserId=@UserId";

        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = userId;

        var rows = await cmd.ExecuteNonQueryAsync();

        if (rows == 0)
            throw new KeyNotFoundException("User not found.");
    }

 
    public async Task<User?> GetByUserIdAsync(string userId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"SELECT UserId, PasswordHash, Name, Permissions 
                      FROM Users 
                      WHERE UserId=@UserId";

        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add("@UserId", System.Data.SqlDbType.NVarChar).Value = userId;

        using var reader = await cmd.ExecuteReaderAsync();

        if (!reader.Read())
            return null;

        return new User
        {
            UserId = reader.GetString(0),
            PasswordHash = reader.GetString(1),
            Name = reader.GetString(2),
            Permissions = reader.GetString(3)
        };
    }


    public async Task<List<User>> GetAllAsync()
    {
        var list = new List<User>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = @"SELECT UserId, PasswordHash, Name, Permissions FROM Users";

        using var cmd = new SqlCommand(query, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new User
            {
                UserId = reader.GetString(0),
                PasswordHash = reader.GetString(1),
                Name = reader.GetString(2),
                Permissions = reader.GetString(3)
            });
        }

        return list;
    }
}
