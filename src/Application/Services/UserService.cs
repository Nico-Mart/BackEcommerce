using Application.Interfaces;
using Application.Models.User;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class UserService : Service<User, CreateUserDto, ReadUserDto, UpdateUserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IGenerateVerificationTokenService _verificationTokenService;
        public UserService(IUserRepository userRepository, IMapper mapper, 
                            IPasswordHasherService passwordHasherService, 
                            IGenerateVerificationTokenService verificationTokenService) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _passwordHasherService = passwordHasherService;
            _verificationTokenService = verificationTokenService;
        }

        public async Task ActivateUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID '{userId}' not found.");
            }

            if (user.IsActive == 1)
            {
                throw new InvalidOperationException("User is already verified.");
            }

            user.IsActive = 1;
            await _userRepository.UpdateAsync(user);
        }

        public override async Task<ReadUserDto> Create(CreateUserDto userDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException($"The email '{userDto.Email}' is already in use.");
            }

            userDto.Password = _passwordHasherService.HashPassword(userDto.Password);
            var createdUser = await base.Create(userDto);

            return createdUser;
        }

        public async Task<string> GenerateVerificationToken(int userId)
        {
            var userEntity = await _userRepository.GetByIdAsync(userId);
            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return _verificationTokenService.GenerateVerificationToken(userEntity); 
        }

        public override async Task<ICollection<ReadUserDto>> CreateRange(ICollection<CreateUserDto> userDtos)
        {
            foreach (var userDto in userDtos)
            {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    throw new ArgumentException($"The email '{userDto.Email}' is already in use.");
                }
            }

            return await base.CreateRange(userDtos);
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"The given key '{id}' is not related to a user.");
            }

            await _userRepository.DeleteAsync(user);
        }

        public override async Task<int> DeleteRange<Tid>(List<Tid> ids)
        {

            var query = _userRepository.GetAll().Where(u => ids.Contains((Tid)(object)u.Id));
            var users = await _userRepository.ToListAsync(query);

            if (users == null || !users.Any())
            {
                throw new KeyNotFoundException($"No users were found with the provided keys: {string.Join(", ", ids)}.");
            }

            return await _userRepository.DeleteRangeAsync(users);

        }

        public override async Task Update(UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userDto.Id);
            if (user == null)
            {
                throw new KeyNotFoundException($"The given key '{userDto.Id}' is not related to a user.");
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
                throw new KeyNotFoundException("No users were found with the provided keys.");
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
