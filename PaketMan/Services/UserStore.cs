using Dapper;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using PaketMan.Context;
using PaketMan.Models;

namespace PaketMan.Services
{
    public class UserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>,
     IUserTwoFactorStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>,IUserRoleStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>
    {
        private readonly string _connectionString;
        private readonly DapperContext _context;

       

        public UserStore(DapperContext context)
        {
            _context = context;
        }

       

        public UserStore(IConfiguration configuration, DapperContext context)
        {
            _connectionString = configuration.GetConnectionString("SqlConnection");
            _context = context;
        }

        IQueryable<ApplicationUser> IQueryableUserStore<ApplicationUser>.Users
        {
            get
            {
                var query = $"SELECT * FROM \"ApplicationUser\";";
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var users = connection.Query<ApplicationUser>(query);
                    return users.AsQueryable();
                }
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
        
                //""NormalizedEmail"", ""PasswordHash"", ""PhoneNumber"", ""TwoFactorEnabled"")
                //VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)}) RETURNING ""Id"";"
                //, user);
                user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO ""ApplicationUser"" (""UserName"", ""NormalizedUserName"", ""Email"",
                    ""NormalizedEmail"", ""PasswordHash"", ""PhoneNumber"")
                    VALUES (@{nameof(ApplicationUser.UserName)}, @{nameof(ApplicationUser.NormalizedUserName)}, @{nameof(ApplicationUser.Email)},
                    @{nameof(ApplicationUser.NormalizedEmail)}, @{nameof(ApplicationUser.PasswordHash)},
                    @{nameof(ApplicationUser.PhoneNumber)}) RETURNING ""Id"";", user);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM \"ApplicationUser\" WHERE \"Id\" = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ""ApplicationUser""
                WHERE ""Id"" = @{nameof(userId)}", new { userId });
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ""ApplicationUser""
                WHERE ""NormalizedUserName"" = @{nameof(normalizedUserName)}", new { normalizedUserName });
            }
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE ""ApplicationUser"" SET
                ""UserName"" = @{nameof(ApplicationUser.UserName)},
                ""NormalizedUserName"" = @{nameof(ApplicationUser.NormalizedUserName)},
                ""Email"" = @{nameof(ApplicationUser.Email)},
                ""NormalizedEmail"" = @{nameof(ApplicationUser.NormalizedEmail)},
                ""PasswordHash"" = @{nameof(ApplicationUser.PasswordHash)},
                ""PhoneNumber"" = @{nameof(ApplicationUser.PhoneNumber)}
                WHERE ""Id"" = @{nameof(ApplicationUser.Id)}", user);
            }

            return IdentityResult.Success;
        }

        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

      

      

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ""ApplicationUser""
                WHERE ""NormalizedEmail"" = @{nameof(normalizedEmail)}", new { normalizedEmail });
            }
        }

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

      

        

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            //user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            //return Task.FromResult(user.TwoFactorEnabled);
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            //Return Roles From Db
            var res = new List<string> { "", "" };
            return Task.FromResult<IList<string>>(res);
            //throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
