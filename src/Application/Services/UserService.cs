using Application.Interfaces;
using Application.Models.Password;
using Application.Models.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class UserService : Service<User, CreateUserDto, ReadUserDto, UpdateUserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IGenerateVerificationTokenService _verificationTokenService;
        private readonly IEmailService _emailService;
        private readonly ITemporaryUserCacheService _temporaryUserCacheService;
        public UserService(IUserRepository userRepository, IMapper mapper,
                            IPasswordHasher<User> passwordHasher,
                            IGenerateVerificationTokenService verificationTokenService,
                            IEmailService emailService,
                            ITemporaryUserCacheService temporaryUserCacheService
            ) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _verificationTokenService = verificationTokenService;
            _emailService = emailService;
            _temporaryUserCacheService = temporaryUserCacheService;
        }

        public override async Task<ReadUserDto> Create(CreateUserDto userDto)
        {
            if (_temporaryUserCacheService.CheckIfUserExists(userDto.Email))
            {
                throw new ArgumentException($"El Email '{userDto.Email}' está en uso.");
            }

            var passwordValidation = new PasswordValidationAttribute();
            var validationResult = passwordValidation.GetValidationResult(userDto.Password, new ValidationContext(userDto));

            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var userEntity = _mapper.Map<User>(userDto);
            userDto.Password = _passwordHasher.HashPassword(userEntity, userDto.Password);



            var token = _verificationTokenService.GenerateVerificationToken(userDto.Email);
            _temporaryUserCacheService.StoreTemporaryUser(token, userDto, TimeSpan.FromMinutes(30));

            await _emailService.SendEmailAsync(userDto.Email, "Verifique su Cuenta",
                "<h3>Verifique su Cuenta</h3>" +
                "<p>Por favor verifique su cuenta haciendo clic en el enlace a continuación:</p>" +
                $"<a href='https://localhost:7037/confirmAccount/{token}' style='color:#007BFF; text-decoration:none;'>Verificar cuenta</a>");

            return new ReadUserDto { Email = userDto.Email, FirstName = userDto.FirstName };
        }

        public async Task<ReadUserDto> CreateWithoutEmailVerification(CreateUserDto userDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException($"El Email '{userDto.Email}' está en uso.");
            }

            var passwordValidation = new PasswordValidationAttribute();
            var validationResult = passwordValidation.GetValidationResult(userDto.Password, new ValidationContext(userDto));

            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.Password = _passwordHasher.HashPassword(userEntity, userDto.Password); 

            var entity = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<ReadUserDto>(entity);
        }



        public async Task ActivateAccount(string token)
        {
            var userDto = _temporaryUserCacheService.GetTemporaryUserByToken(token) ?? throw new SecurityTokenException("Token invalido o expirado.");
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.IsActive = 1;
            await _userRepository.CreateAsync(userEntity);

            _temporaryUserCacheService.RemoveTemporaryUser(token);
        }
        public async Task RequestPasswordReset(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email) ?? throw new ArgumentException("Email not found");
            var token = _verificationTokenService.GenerateVerificationToken(email);
            var resetLink = $"http://localhost:5173/newresetpassword?token={token}";
            var body = $"Click aqui para resetear su contraseña : <a href='{resetLink}'>Reset Password</a>";
            await _emailService.SendEmailAsync(email, "Password Reset Request", body);

            var userDto = new CreateUserDto
            {
                Email = user.Email,
                Password = user.Password
            };

            _temporaryUserCacheService.StoreTemporaryUser(token, (email, userDto), TimeSpan.FromMinutes(30));
        }

        public async Task ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var tempData = _temporaryUserCacheService.GetTemporaryUserDataByToken(resetPasswordDto.Token)
                ?? throw new SecurityTokenException("Invalid or expired token.");

            var (userEmail, userDto) = ((string, CreateUserDto))tempData;

            var user = await _userRepository.GetByEmailAsync(userEmail)
                ?? throw new ArgumentException("Email no encontrado");

            var passwordValidation = new PasswordValidationAttribute();
            var validationResult = passwordValidation.GetValidationResult(resetPasswordDto.NewPassword, new ValidationContext(resetPasswordDto));

            if (validationResult != ValidationResult.Success)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, resetPasswordDto.NewPassword);
            if (verificationResult == PasswordVerificationResult.Success)
            {
                throw new ArgumentException("La nueva contraseña no puede ser la misma que la contraseña anterior.");
            }

            user.Password = _passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
            _temporaryUserCacheService.RemoveTemporaryUser(resetPasswordDto.Token);
        }



        public async Task<string> GenerateVerificationToken(string email)
        {
            var userEntity = await _userRepository.GetByEmailAsync(email) ?? throw new KeyNotFoundException("Usuario no encontrado");
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
            var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"El id proporcionado: '{id}' no corresponde a ningun usuario.");
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
            var user = await _userRepository.GetByIdAsync(userDto.Id) ?? throw new KeyNotFoundException($"El id proporcionado: '{userDto.Id}' no corresponde a ningún usuario.");

            if (!string.IsNullOrEmpty(userDto.FirstName))
            {
                user.FirstName = userDto.FirstName;
            }

            if (!string.IsNullOrEmpty(userDto.LastName))
            {
                user.LastName = userDto.LastName;
            }
            var role = user.Role;
            var isActive = user.IsActive;
            var password = user.Password;
            var email = user.Email;

            _mapper.Map(userDto, user);
            user.Role = role;
            user.IsActive = isActive;
            user.Password = password;
            user.Email = email;
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

    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("La contraseña es obligatoria.");
            }

            if (password.Length < 8)
            {
                return new ValidationResult("La contraseña debe tener al menos 8 caracteres.");
            }

            if (!password.Any(char.IsUpper))
            {
                return new ValidationResult("La contraseña debe contener al menos una letra mayúscula.");
            }

            if (!password.Any(char.IsLower))
            {
                return new ValidationResult("La contraseña debe contener al menos una letra minúscula.");
            }

            if (!password.Any(char.IsDigit))
            {
                return new ValidationResult("La contraseña debe contener al menos un número.");
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return new ValidationResult("La contraseña debe contener al menos un carácter especial.");
            }

            return ValidationResult.Success;
        }
    }


}
