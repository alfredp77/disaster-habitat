using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Kastil.Core.Services;
using Kastil.Core.ViewModels;
using Kastil.Shared.Models;
using Moq;
using NUnit.Framework;

namespace Kastil.Core.Tests.ViewModels
{

    [TestFixture]
    public class LoginViewModelTests : BaseTest
    {
        private MockRepository _mockRepository;
        private LoginViewModel _vm;
        private Mock<IUserService> _userService;
        private Mock<IUserDialogs> _userDialog;

        public override void CreateTestableObject()
        {
            base.CreateTestableObject();
            _mockRepository = new MockRepository(MockBehavior.Default);
            _vm = new LoginViewModel();

            _userService = _mockRepository.Create<IUserService>();
            _userDialog = _mockRepository.Create<IUserDialogs>();

            Ioc.RegisterSingleton(_userService.Object);
            Ioc.RegisterSingleton(_userDialog.Object);
        }

        [Test]
        public async Task When_StaffCode_Is_Blank_Should_Prompt_Error()
        {
            _vm.StaffCode = "";
            await _vm.LoginCommand.ExecuteAsync();

            _userDialog.Verify(u => u.PromptAsync(LoginViewModel.PleaseKeyInYourStaffCode, null, null, null, "", InputType.Default, null));
            AssertDispatcherNotCalled<HomeViewModel>();            
        }

        [Test]
        public async Task When_StaffCode_Is_Valid_Should_Go_To_Home()
        {
            var user = new User {Id = "123", StaffCode = "abc"};
            _userService.Setup(s => s.Login(user.StaffCode)).ReturnsAsync(user);

            _vm.StaffCode = user.StaffCode;
            await _vm.LoginCommand.ExecuteAsync();

            AssertDispatcherCalled<HomeViewModel>();
        }

        [Test]
        public async Task When_StaffCode_Is_Invalid_Should_Show_Error()
        {
            var staffCode = "abc";
            _userService.Setup(s => s.Login(staffCode)).ReturnsAsync(null);

            _vm.StaffCode = staffCode;
            await _vm.LoginCommand.ExecuteAsync();

            _userDialog.Verify(u => u.PromptAsync(LoginViewModel.SomethingWentWrongPleaseTryAgain, null, null, null, "", InputType.Default, null));
            AssertDispatcherNotCalled<HomeViewModel>();
        }
    }
}
