using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.AspNetCore.DataProtection;
namespace TasksManagement_API.ServicesRepositories
{
    public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
    {
        private readonly DailyTasksMigrationsContext dataBaseSqlServerContext;
        private readonly IDataProtectionProvider provider;
        private readonly IJwtTokenService jwtTokenService;

        private readonly IConfiguration configuration;
        private const string Purpose = "my protection purpose"; //On donne une intention pour l'encryptage explire dans 90jours
        public UtilisateurService(DailyTasksMigrationsContext dataBaseSqlServerContext, IJwtTokenService jwtTokenService, IConfiguration configuration, IDataProtectionProvider provider)
        {
            this.dataBaseSqlServerContext = dataBaseSqlServerContext;
            this.jwtTokenService = jwtTokenService;
            this.configuration = configuration;
            this.provider = provider;
        }

        public async Task<TokenResult> GetToken(string email)
        {
            var utilisateurs = await GetUsers(query => query.Where(u => u.Email.ToUpper().Equals(email.ToUpper()) && u.Role.Equals(Utilisateur.Privilege.Administrateur)));
            if (utilisateurs is null || utilisateurs.Count == 0)
            {
                return new TokenResult
                {
                    Success = false,
                    Message = "Droits insuffisants ou adresse mail inexistante !"
                };
            }
            await Task.Delay(500);
            var utilisateur = utilisateurs.First();
            return new TokenResult
            {
                Success = true,
                Token = jwtTokenService.GenerateJwtToken(utilisateur.Email)
            };
        }

        public bool CheckUserSecret(string secretPass)
        {
            // Env.Load("ServicesRepositories/.env");
            string secretUserPass = configuration["ConnectionStrings:SecretApiKey"]; // Dev configuration["JwtSettings:JwtSecretKey"]; // Prod Environment.GetEnvironmentVariable("PasswordSecret")!; //

            if (string.IsNullOrEmpty(secretUserPass))
            {
                throw new NotImplementedException("La clé secrete est inexistante");

            }
            var Pass = BCrypt.Net.BCrypt.HashPassword($"{secretPass}");

            var BCryptResult = BCrypt.Net.BCrypt.Verify(secretUserPass, Pass);
            if (!BCryptResult.Equals(true)) { return false; }
            return true;
        }

        public async Task<ICollection<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null)
        {
            IQueryable<Utilisateur> query = dataBaseSqlServerContext.Utilisateurs
                                   .Include(u => u.LesTaches);
            if (filter != null)
            {
                query = filter(query);
            }

            return await query.ToListAsync();
        }
        public async Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role)
        {
            return (await GetUsers(query => query.Where(user => user.Nom == nom && user.Role == role))).FirstOrDefault();
        }

        public string EncryptUserSecret(string plainText)
        {
            var protector = provider.CreateProtector(Purpose);
            return protector.Protect(plainText);
        }

        public string DecryptUserSecret(string cipherText)
        {
            var protector = provider.CreateProtector(Purpose);
            return protector.Unprotect(cipherText);
        }

        public async Task<string?> CheckExistedUser(Utilisateur utilisateur)
        {
            var listUtilisateurs = dataBaseSqlServerContext.Utilisateurs.ToList();
            var utilisateurExistant = await GetSingleUserByNameRole(utilisateur.Nom, utilisateur.Role);
            if (utilisateurExistant == null)
            {
                return utilisateur.Nom;
            }
            var i = 1;
            var nouveauNomUtilisateur = $"{utilisateur.Nom}_{i}";
            if (utilisateurExistant.Nom == utilisateur.Nom)
            {
                while (listUtilisateurs.Any(item => item.Nom == nouveauNomUtilisateur))
                {
                    i++;
                    nouveauNomUtilisateur = $"{utilisateur.Nom}_{i}";
                }
            }
            utilisateur.Nom = nouveauNomUtilisateur;
            return utilisateur.Nom;
        }
        public async Task<Utilisateur> CreateUser(Utilisateur utilisateur)
        {
            if (utilisateur.CheckHashPassword(utilisateur.Pass) && utilisateur.CheckEmailAdress(utilisateur.Email))
            {
                // Si l'utilisateur a des tâches, on les associe à l'utilisateur avant l'insertion
                if (utilisateur.LesTaches != null && utilisateur.LesTaches.Count > 0)
                {
                    foreach (var tache in utilisateur.LesTaches)
                    {
                        tache.NomUtilisateur = utilisateur.Nom;
                        tache.EmailUtilisateur = utilisateur.Email;
                        utilisateur.LesTaches.Add(tache);
                    }
                }
            }
            await dataBaseSqlServerContext.Utilisateurs.AddAsync(utilisateur);
            await dataBaseSqlServerContext.SaveChangesAsync();
            return utilisateur;
        }
        public async Task SetUserPassword(string nom, string mdp)
        {
            await dataBaseSqlServerContext.SaveChangesAsync();
        }
        public async Task DeleteUserByDetails(string nom, Utilisateur.Privilege role)
        {
            var utilisateur = (await GetUsers(query => query.Where(user => user.Nom == nom && user.Role == role))).FirstOrDefault();
            dataBaseSqlServerContext.Utilisateurs.Remove(utilisateur!);
            await dataBaseSqlServerContext.SaveChangesAsync();

        }
    }
}