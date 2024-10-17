using Application.Interfaces;
using Application.Models.Product;
using Application.Shared.Classes;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UserService : Service<User, CreateUserDto, ReadUserDto, UpdateUserDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
        }

        public virtual async Task<ICollection<ReadUserDto>> GetAll(Options? options)
        {
            var users = await base.GetAll(options);

            var userDtos = _mapper.Map<ICollection<ReadUserDto>>(users);
            return userDtos;

        }
        public override async Task<ReadUserDto> Create(CreateUserDto userDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException($"The email '{userDto.Email}' is already in use.");
            }

            var user = _mapper.Map<User>(userDto);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.CreateAsync(user);

            var readUserDto = _mapper.Map<ReadUserDto>(user);
            return readUserDto;
        }

        public override async Task Delete<Tid>(Tid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"The given key '{id}' does not correspond to a user.");
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
                throw new KeyNotFoundException($"The given key '{userDto.Id}' does not correspond to a user.");
            }

            _mapper.Map(userDto, user);
            user.UpdatedAt = DateTime.UtcNow;

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
