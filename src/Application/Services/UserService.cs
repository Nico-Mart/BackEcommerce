using Application.Interfaces;
using Application.Models.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class UserService : Service<User, CreateUserDto, ReadUserDto, UpdateUserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IGenerateVerificationTokenService _verificationTokenService;
        private readonly IEmailService _emailService;
        private readonly ITemporaryUserCacheService _temporaryUserCacheService;
        public UserService(IUserRepository userRepository, IMapper mapper, 
                            IPasswordHasherService passwordHasherService, 
                            IGenerateVerificationTokenService verificationTokenService, 
                            IEmailService emailService,
                            ITemporaryUserCacheService temporaryUserCacheService
            ) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _verificationTokenService = verificationTokenService;
            _emailService = emailService;
            _temporaryUserCacheService = temporaryUserCacheService;
        }

        public override async Task<ReadUserDto> Create(CreateUserDto userDto)
        {
            if (_temporaryUserCacheService.CheckIfUserExists(userDto.Email))
            {
                throw new ArgumentException($"El Email '{userDto.Email}' esta en uso.");
            }

            userDto.Password = _passwordHasherService.HashPassword(userDto.Password);

            var token = _verificationTokenService.GenerateVerificationToken(userDto.Email);

            _temporaryUserCacheService.StoreTemporaryUser(token, userDto, TimeSpan.FromMinutes(30));

            await _emailService.SendEmailAsync(userDto.Email, "Verifique su Cuenta",
                "<h3>Verifique su Cuenta</h3>" +
                "<p>Por favor verifique su cuenta haciendo clic en el enlace a continuación:</p>" +
                $"<a href='https://localhost:7037/api/User/verify?token={token}' style='color:#007BFF; text-decoration:none;'>Verificar cuenta</a>");

            return new ReadUserDto { Email = userDto.Email, FirstName = userDto.FirstName };
        }
        public async Task ActivateAccount(string token)
        {
            var userDto = _temporaryUserCacheService.GetTemporaryUserByToken(token);

            if (userDto == null)
            {
                throw new SecurityTokenException("Token invalido o expirado.");
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.IsActive = true;
            await _userRepository.CreateAsync(userEntity);

            _temporaryUserCacheService.RemoveTemporaryUser(token);
        }


        public async Task<string> GenerateVerificationToken(string email)
        {
            var userEntity = await _userRepository.GetByEmailAsync(email);
            if (userEntity == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            return _verificationTokenService.GenerateVerificationToken(userEntity.Email); 
        }

        public override async Task<ICollection<ReadUserDto>> CreateRange(ICollection<CreateUserDto> userDtos)
        {
            foreach (var userDto in userDtos)
            {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    throw new ArgumentException($"El Email '{userDto.Email}' esta en uso.");
                }
            }

            return await base.CreateRange(userDtos);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
<<<<<<< Updated upstream
                throw new KeyNotFoundException($"The given key '{id}' is not related to a user.");
=======
                throw new KeyNotFoundException($"El id proporcionado: '{id}' no corresponde a ningun usuario.");
>>>>>>> Stashed changes
            }

            await _userRepository.DeleteAsync(user);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {

            var query = _userRepository.GetAll().Where(u => ids.Contains((Tid)(object)u.Id));
            var users = await _userRepository.ToListAsync(query);

            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException($"No se encontraron usuarios con las claves proporcionadas: {string.Join(", ", ids)}.");
            }

            return await _userRepository.DeleteRangeAsync(users);

        }

        public override async Task Update(UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.Id);
            if (user == null)
            {
<<<<<<< Updated upstream
                throw new KeyNotFoundException($"The given key '{userDto.Id}' is not related to a user.");
=======
                throw new KeyNotFoundException($"El id proporcionado: '{userDto.Id}' no corresponde a ningun usuario.");
>>>>>>> Stashed changes
            }

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                userDto.Password = _passwordHasherService.HashPassword(userDto.Password);
            }

              _mapper.Map(userDto, user);
             await _userRepository.UpdateAsync(user);
        }

        public override async Task<int> UpdateRange(ICollection<UpdateUserDto> userDtos)
        {
            var ids = userDtos.Select(u => u.Id).ToList();

            var query = _userRepository.GetAll().Where(u => ids.Contains(u.Id));
            var users = await _userRepository.ToListAsync(query);

            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException($"No se encontraron usuarios con las claves proporcionadas. {string.Join(", ", ids)}.");
            }

            foreach (var user in users)
            {
                var userDto = userDtos.FirstOrDefault(dto => dto.Id == user.Id);
                if (userDto != null)
                {
                    _mapper.Map(userDto, user); 
                    user.UpdatedAt = DateTime.UtcNow; 
                }
            }

            return await _userRepository.UpdateRangeAsync(users);
        }
    }
}
