using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatformApp.Business.DataProtection;
using OnlineShoppingPlatformApp.Business.Operations.User.Dtos;
using OnlineShoppingPlatformApp.Business.Types;
using OnlineShoppingPlatformApp.Data.Entities;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.Data.Repositories;
using OnlineShoppingPlatformApp.Data.UnitOfWork;

namespace OnlineShoppingPlatformApp.Business.Operations.User
{
    public class UserManager : IUserService
    {

        // bunlar scopedları eklendi.
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IDataProtection _protection;

        public UserManager(IUnitOfWork unitOfWork, IRepository<UserEntity> userRepository, IDataProtection protection)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _protection = protection;
        }

        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            var hasEmail = await _userRepository.GetAll(x => x.Email.ToLower() == user.Email.ToLower()).AnyAsync();
            if (hasEmail)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Email adresi zaten mevcut."
                };
            }

            // AddUserDto'dan UserEntity'e dönüştürme işlemi
            // repository'e eklemek için void Add(TEntity entity); bekliyor.
            var userEntity = new UserEntity
            {
                Email = user.Email,
                Password = _protection.Protect(user.Password),
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Role = UserType.Customer,
                PhoneNumber = user.PhoneNumber
            };

            _userRepository.Add(userEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw new Exception("Kullanıcı eklenirken bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSuccess = true,
                Message = "Kullanıcı başarıyla eklendi."
            };

        }

        public async Task<ServiceMessage> ChangeUserPassword(int id, ChangePasswordDto changePassword)
        {
            var userEntity = _userRepository.Get(x => x.Id == id);
            if (userEntity is null)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            var unprotectedPassword = _protection.UnProtect(userEntity.Password);
            if (unprotectedPassword == changePassword.CurrentPassword)
            {
                userEntity.Password = _protection.Protect(changePassword.NewPassword);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (System.Exception)
                {
                    throw new Exception("Şifre değiştirilirken bir hata oluştu.");
                }

                return new ServiceMessage
                {
                    IsSuccess = true,
                    Message = "Şifre başarıyla değiştirildi."
                };
            }
            else
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Şifre geçersiz."
                };
            }
        }

        public async Task<ServiceMessage> DeleteUser(int id)
        {
            var userEntity = await _userRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (userEntity is null)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }
            _userRepository.Delete(userEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw new Exception("Kullanıcı silinirken bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSuccess = true,
                Message = "Kullanıcı başarıyla silindi."
            };
        }

        public async Task<ServiceMessage<List<UserInfoDto>>> GetAllUsers()
        {
            var users = await _userRepository.GetAll().Select(x => new UserInfoDto
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role
            }).ToListAsync();

            return new ServiceMessage<List<UserInfoDto>>
            {
                IsSuccess = true,
                Data = users
            };
        }

        public async Task<ServiceMessage<UserInfoDto>> GetUserById(int id)
        {
            var userEntity = await _userRepository.GetAll(x => x.Id == id).Select(x => new UserInfoDto
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role
            }).FirstOrDefaultAsync();

            if (userEntity is null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }


            return new ServiceMessage<UserInfoDto>
            {
                IsSuccess = true,
                Data = userEntity
            };
        }

        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {
            var userEntity = _userRepository.Get(u => u.Email.ToLower() == user.Email.ToLower());
            if (userEntity is null)
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSuccess = false,
                    Message = "Kullanıcı adı veya şifre bulunamadı."
                };
            }

            var unprotectedPassword = _protection.UnProtect(userEntity.Password);
            if (unprotectedPassword == user.Password)
            {
                var userInfo = new UserInfoDto
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Role = userEntity.Role
                };
                return new ServiceMessage<UserInfoDto>
                {
                    IsSuccess = true,
                    Data = userInfo
                };
            }
            else
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSuccess = false,
                    Message = "Kullanıcı adı veya şifre bulunamadı."
                };
            }

        }

        public async Task<ServiceMessage> UpdateUser(int id, UpdateUserDto update)
        {
            var userEntity = await _userRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (userEntity is null)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            userEntity.Email = update.Email;
            userEntity.FirstName = update.FirstName;
            userEntity.LastName = update.LastName;
            userEntity.BirthDate = update.BirthDate;
            userEntity.PhoneNumber = update.PhoneNumber;

            _userRepository.Update(userEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw new Exception("Kullanıcı güncellenirken bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSuccess = true,
                Message = "Kullanıcı başarıyla güncellendi."
            };
        }

        public async Task<ServiceMessage> UpdateUserRole(int id, UserType role)
        {
            var userEntity = await _userRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (userEntity is null)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };
            }

            userEntity.Role = role;
            _userRepository.Update(userEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                throw new Exception("Kullanıcı rolü güncellenirken bir hata oluştu.");
            }

            return new ServiceMessage
            {
                IsSuccess = true,
                Message = "Kullanıcı rolü başarıyla güncellendi."
            };
        }
    }
}